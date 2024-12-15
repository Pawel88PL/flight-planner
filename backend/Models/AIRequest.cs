namespace backend.Models
{
    public class AIRequest
    {
        public string? model { get; set; }
        public decimal? temperature { get; set; }
        public int? max_tokens { get; set; }
        public List<AIMessages>? messages { get; set; }
    }

    public class AIMessages
    {
        public string? role { get; set; }
        public string? content { get; set; }
    }
}