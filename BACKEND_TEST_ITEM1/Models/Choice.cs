using System;

namespace QuizIslandAPI.Models
{
    public class Choice
    {
        public Guid ChoiceId { get; set; }
        public Guid QuestionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsAnswer { get; set; }
    }
}
