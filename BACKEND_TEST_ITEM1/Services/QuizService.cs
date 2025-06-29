using QuizIslandAPI.Models.DTOs;
using Microsoft.Data.SqlClient; // ✅ ใช้อันเดียวพอ ไม่ใช้ System.Data.SqlClient

namespace QuizIslandAPI.Services
{
    public class QuizService : IQuizService
    {
        private readonly string _connectionString;

        public QuizService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<Guid> CreateSessionAsync()
        {
            var sessionId = Guid.NewGuid();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("INSERT INTO Sessions (SessionId, CreatedAt, IsEnded) VALUES (@id, GETDATE(), 0)", conn);
            cmd.Parameters.AddWithValue("@id", sessionId);
            await cmd.ExecuteNonQueryAsync();
            return sessionId;
        }

        public async Task<QuestionDto?> GetNextQuestionAsync(Guid sessionId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT TOP 1 q.QuestionId, q.Title
                FROM Questions q
                WHERE NOT EXISTS (
                    SELECT 1 FROM Answers a
                    WHERE a.SessionId = @sessionId AND a.QuestionId = q.QuestionId
                )", conn);

            cmd.Parameters.AddWithValue("@sessionId", sessionId);
            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read()) return null;
            var questionId = reader.GetGuid(0);
            var title = reader.GetString(1);
            reader.Close();

            var choices = new List<ChoiceDto>();
            var cmdChoices = new SqlCommand("SELECT ChoiceId, Title FROM Choices WHERE QuestionId = @qid", conn);
            cmdChoices.Parameters.AddWithValue("@qid", questionId);
            using var choiceReader = await cmdChoices.ExecuteReaderAsync();
            while (await choiceReader.ReadAsync())
            {
                choices.Add(new ChoiceDto
                {
                    ChoiceId = choiceReader.GetGuid(0),
                    Title = choiceReader.GetString(1)
                });
            }

            return new QuestionDto
            {
                QuestionId = questionId,
                Title = title,
                Choices = choices
            };
        }

        public async Task<bool> SubmitAnswerAsync(SubmitAnswerDto dto)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                INSERT INTO Answers (AnswerId, SessionId, QuestionId, ChoiceId, AnsweredAt)
                VALUES (@id, @session, @question, @choice, GETDATE())", conn);

            cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("@session", dto.SessionId);
            cmd.Parameters.AddWithValue("@question", dto.QuestionId);
            cmd.Parameters.AddWithValue("@choice", dto.ChoiceId);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<SummaryDto> GetSummaryAsync(Guid sessionId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                SELECT COUNT(*) AS Total,
                       SUM(CASE WHEN c.IsCorrect = 1 THEN 1 ELSE 0 END) AS Correct
                FROM Answers a
                INNER JOIN Choices c ON a.ChoiceId = c.ChoiceId
                WHERE a.SessionId = @sessionId", conn);

            cmd.Parameters.AddWithValue("@sessionId", sessionId);
            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.Read()) return new SummaryDto { Total = 0, Correct = 0 };

            return new SummaryDto
            {
                Total = reader.GetInt32(0),
                Correct = reader.GetInt32(1)
            };
        }

        public async Task EndSessionAsync(Guid sessionId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("UPDATE Sessions SET IsEnded = 1 WHERE SessionId = @id", conn);
            cmd.Parameters.AddWithValue("@id", sessionId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
