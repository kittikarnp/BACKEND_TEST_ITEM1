using System;

namespace QuizIslandAPI.Models
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
