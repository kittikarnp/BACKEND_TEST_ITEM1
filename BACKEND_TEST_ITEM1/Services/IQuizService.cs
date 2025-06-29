using QuizIslandAPI.Models.DTOs;

namespace QuizIslandAPI.Services
{
    public interface IQuizService
    {
        Task<Guid> CreateSessionAsync();
        Task<QuestionDto?> GetNextQuestionAsync(Guid sessionId);
        Task<bool> SubmitAnswerAsync(SubmitAnswerDto dto);
        Task<SummaryDto> GetSummaryAsync(Guid sessionId);
        Task EndSessionAsync(Guid sessionId);
    }
}