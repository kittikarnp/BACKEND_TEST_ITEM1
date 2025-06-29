using System;

namespace QuizIslandAPI.Models.DTOs
{
    public class SubmitAnswerDto
    {
        public Guid SessionId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }
    }
}
