using System;

namespace QuizIslandAPI.Models
{
    public class Answer
    {
        public Guid AnswerId { get; set; }
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime AnsweredAt { get; set; }
    }
}
