using BoardGameFontier.Repostiory.DTOS;
using BoardGameFontier.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameFontier.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminApiController : ControllerBase
    {
        private readonly IAdminService _service;
        private readonly ILogger<AdminApiController> _logger;

        public AdminApiController(IAdminService service, ILogger<AdminApiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("allusers")]
        public IActionResult GetAllUsers([FromBody] GetAllUsersDto dto)
        {
            try
            {
                var data = _service.GetAllUsers(dto.SearchText ?? "", dto.Skip, dto.Take);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetAllUsers] Error: {ex}");
                return BadRequest("無法取得使用者資料");
            }
        }

        [HttpPost("updateLockout")] 
        public IActionResult UpdateLockout([FromBody] UpdateLockoutDto dto)
        {
            try
            {
                var result = _service.UpdateUserLockout(dto.Id, dto.LockoutEnabled);
                if (result)
                    return Ok(new { success = true });
                else
                    return NotFound(new { success = false, message = "使用者不存在" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[UpdateLockout] Error: {ex}");
                return StatusCode(500, new { success = false, message = "內部錯誤" });
            }
        }

        [HttpPost("updateRole")]
        public IActionResult UpdateRole([FromBody] UpdateRoleDto dto)
        {
            try
            {
                var result = _service.UpdateUserRole(dto.Id, dto.Role);
                if (result)
                    return Ok(new { success = true });
                else
                    return NotFound(new { success = false, message = "使用者不存在" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[UpdateRole] Error: {ex}");
                return StatusCode(500, new { success = false, message = "內部錯誤" });
            }
        }
    }
}
