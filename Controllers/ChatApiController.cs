using Microsoft.AspNetCore.Mvc;
using MunicipalServicesMVC.Services.Chatbot;

namespace MunicipalServicesMVC.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public sealed class ChatApiController : ControllerBase
    {
        private static readonly Bot _bot = new();

        public sealed class ChatRequest { public string? Text { get; set; } }
        public sealed class ChatResponse
        {
            public string ReplyHtml { get; set; } = "";
            public string? NavigateUrl { get; set; }
        }

        [HttpPost]
        public ActionResult<ChatResponse> Post([FromBody] ChatRequest req)
        {
            var (replyHtml, nav) = _bot.Respond(req.Text ?? "");
            return Ok(new ChatResponse { ReplyHtml = replyHtml, NavigateUrl = nav });
        }
    }
}
