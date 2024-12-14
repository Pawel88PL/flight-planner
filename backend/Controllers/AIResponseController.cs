using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/ai-response")]
    public class AIResponseController: ControllerBase
    {
        private readonly IAIService _aiService;

        public AIResponseController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetAIResponseByFlightPlanId(int flightPlanId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var aiResponse = await _aiService.GetAIResponseByFlightPlanId(flightPlanId);

                if (aiResponse == null)
                {
                    return NotFound();
                }

                return Ok(aiResponse);
            }
            catch (Exception ex)
            {
                var message = "Wystąpił błąd podczas pobierania odpowiedzi z AI. " + ex.Message;
                Log.Error(message);
                return BadRequest(new { message });
            }
        }
    }
}