using ChattingApp.Application.DTOs;
using ChattingApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattingApp.API.Controllers
{
    /// <summary>
    /// REST API controller for chat operations.
    /// Provides HTTP endpoints as an alternative/fallback to SignalR for:
    ///   - Loading chat history on initial page render
    ///   - Sending messages from non-WebSocket clients
    ///
    /// All real-time interactions are handled by ChatHub;
    /// this controller supports REST-based workflows.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChatHistory(
            [FromQuery] string userA,
            [FromQuery] string userB)
        {
            if (string.IsNullOrWhiteSpace(userA) || string.IsNullOrWhiteSpace(userB))
                return BadRequest(new { error = "Both 'userA' and 'userB' query parameters are required." });

            try
            {
                var history = await _chatService.GetChatHistoryAsync(userA, userB);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching chat history.");
                return StatusCode(500, new { error = "Internal server error. Please try again." });
            }
        }

        /// <summary>
        /// POST /api/chat/send
        /// Saves a message to the database.
        /// NOTE: This does NOT broadcast via SignalR — for real-time, use the Hub.
        /// Use this endpoint for REST clients or retry scenarios.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var savedMessage = await _chatService.SendMessageAsync(sendMessageDto);
                return StatusCode(StatusCodes.Status201Created, savedMessage);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message via REST.");
                return StatusCode(500, new { error = "Internal server error." });
            }
        }

        /// <summary>
        /// DELETE /api/chat/cleanup
        /// Manually triggers cleanup of messages older than 12 hours.
        /// Useful for testing or on-demand admin use.
        /// </summary>
        [HttpDelete("cleanup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CleanupOldMessages()
        {
            try
            {
                await _chatService.CleanupOldMessagesAsync();
                return Ok(new { message = "Old messages cleaned up successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual cleanup.");
                return StatusCode(500, new { error = "Cleanup failed." });
            }
        }
    }
}
