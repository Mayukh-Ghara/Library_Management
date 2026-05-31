using LibraryWebAPI.DTOs;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebAPI.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingsController : ControllerBase
    {
        private readonly BorrowingService _borrowingService;
        private readonly IUsrTokenContext usrTokenContext;

        public BorrowingsController(BorrowingService borrowingService, IUsrTokenContext usrTokenContext)
        {
            _borrowingService = borrowingService;
            this.usrTokenContext = usrTokenContext;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowRequestDto request)
        {
            var (success, message, data) = await _borrowingService.BorrowBookAsync(request);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, data });
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnRequestDto request)
        {
            var (success, message, data) = await _borrowingService.ReturnBookAsync(request);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, data });
        }

        [HttpGet()]
        public async Task<IActionResult> GetUserBorrowings()
        {
            var borrowings = await _borrowingService.GetUserBorrowingsAsync(usrTokenContext.GetUserId());
            return Ok(borrowings);
        }
    }
}