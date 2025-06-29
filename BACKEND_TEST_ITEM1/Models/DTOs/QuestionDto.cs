using System;
using System.Collections.Generic;

namespace QuizIslandAPI.Models.DTOs
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Title { get; set; }
        public List<ChoiceDto> Choices { get; set; }
    }
}
