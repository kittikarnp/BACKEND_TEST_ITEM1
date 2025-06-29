using System;

namespace QuizIslandAPI.Models
{
    public class AnswerDto
    {
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }
    }
}
