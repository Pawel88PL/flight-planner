using backend.Models;

namespace backend.Interfaces
{
    public interface IOpenAIHelper
    {
        Task<string> SentRequestToOpenAI(AIRequest request);
    }
}