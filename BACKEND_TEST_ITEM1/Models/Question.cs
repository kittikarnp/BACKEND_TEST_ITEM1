using System;

namespace QuizIslandAPI.Models
{
    public class Question
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}