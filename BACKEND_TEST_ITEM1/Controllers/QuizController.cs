using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuizIslandAPI.Models;

namespace QuizIslandAPI.Controllers
{
    [ApiController]
    [Route("api/quiz")]
    public class QuizController : ControllerBase
    {
        private readonly string _connectionString;

        public QuizController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        [HttpPost("session")]
        public IActionResult CreateSession()
        {
            var sessionId = Guid.NewGuid();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("INSERT INTO SessionsQuiz (SessionId, CreatedAt) VALUES (@id, GETUTCDATE())", conn);
            cmd.Parameters.AddWithValue("@id", sessionId);
            conn.Open();
            cmd.ExecuteNonQuery();
            return Ok(new { sessionId });
        }

        [HttpGet("questions/{sessionId}")]
        public IActionResult GetNextQuestion(Guid sessionId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM SessionsQuiz WHERE SessionId = @sid", conn))
            {
                checkCmd.Parameters.AddWithValue("@sid", sessionId);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists == 0)
                    return BadRequest("Invalid SessionId: session not found.");
            }

            Guid questionId;
            string title;

            using (var cmd = new SqlCommand(@"SELECT TOP 1 q.QuestionId, q.Title FROM QuestionsQuiz q
                WHERE q.QuestionId NOT IN (SELECT QuestionId FROM AnswersQuiz WHERE SessionId = @sessionId)", conn))
            {
                cmd.Parameters.AddWithValue("@sessionId", sessionId);
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return Ok(null);

                questionId = reader.GetGuid(0);
                title = reader.GetString(1);
                reader.Close();
            }

            var choices = new List<object>();
            using (var choiceCmd = new SqlCommand("SELECT ChoiceId, Title FROM ChoicesQuiz WHERE QuestionId = @qid", conn))
            {
                choiceCmd.Parameters.AddWithValue("@qid", questionId);
                using var cr = choiceCmd.ExecuteReader();
                while (cr.Read())
                {
                    choices.Add(new
                    {
                        choiceId = cr.GetGuid(0),
                        title = cr.GetString(1)
                    });
                }
            }

            return Ok(new { questionId, title, choices });
        }

        [HttpPost("answer")]
        public IActionResult SubmitAnswer([FromBody] AnswerDto dto)
        {
            if (dto.SessionId == Guid.Empty || dto.QuestionId == Guid.Empty || dto.ChoiceId == Guid.Empty)
                return BadRequest("Invalid input: all fields are required.");

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM SessionsQuiz WHERE SessionId = @sid", conn))
            {
                checkCmd.Parameters.AddWithValue("@sid", dto.SessionId);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists == 0)
                    return BadRequest("Invalid SessionId: session not found.");
            }

            bool isCorrect = false;
            using (var correctCmd = new SqlCommand(@"SELECT IsAnswer FROM ChoicesQuiz WHERE ChoiceId = @cid AND QuestionId = @qid", conn))
            {
                correctCmd.Parameters.AddWithValue("@cid", dto.ChoiceId);
                correctCmd.Parameters.AddWithValue("@qid", dto.QuestionId);
                var result = correctCmd.ExecuteScalar();
                if (result != null)
                    isCorrect = Convert.ToBoolean(result);
            }

            using var cmd = new SqlCommand(@"INSERT INTO AnswersQuiz (AnswerId, SessionId, QuestionId, ChoiceId, AnsweredAt, IsCorrect)
                VALUES (NEWID(), @sid, @qid, @cid, GETUTCDATE(), @iscorrect)", conn);

            cmd.Parameters.AddWithValue("@sid", dto.SessionId);
            cmd.Parameters.AddWithValue("@qid", dto.QuestionId);
            cmd.Parameters.AddWithValue("@cid", dto.ChoiceId);
            cmd.Parameters.AddWithValue("@iscorrect", isCorrect);
            cmd.ExecuteNonQuery();

            return Ok(new { status = "success", isCorrect });
        }

        [HttpGet("summary/{sessionId}")]
        public IActionResult GetSummary(Guid sessionId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM SessionsQuiz WHERE SessionId = @sid", conn))
            {
                checkCmd.Parameters.AddWithValue("@sid", sessionId);
                int exists = (int)checkCmd.ExecuteScalar();
                if (exists == 0)
                    return BadRequest("Invalid SessionId: session not found.");
            }

            using var cmd = new SqlCommand(@"SELECT q.Title AS Question, c.Title AS Selected, a.IsCorrect FROM AnswersQuiz a
                JOIN QuestionsQuiz q ON a.QuestionId = q.QuestionId
                JOIN ChoicesQuiz c ON a.ChoiceId = c.ChoiceId
                WHERE a.SessionId = @sid", conn);

            cmd.Parameters.AddWithValue("@sid", sessionId);
            var result = new List<object>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new
                {
                    question = reader.GetString(0),
                    selected = reader.GetString(1),
                    isCorrect = reader.GetBoolean(2)
                });
            }

            return Ok(result);
        }
    }
}