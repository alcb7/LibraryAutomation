using LibraryAutomation.Core.DTO_s;
using LibraryAutomation.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAutomation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   [Authorize(Roles = Roles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Tüm kullanıcıları listele
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync(); // Kullanıcıları çek

            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Kullanıcının rollerini çek
                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.IsApproved,
                    Roles = roles
                });
            }

            return Ok(userList);
        }


        // Kullanıcıyı onayla (IsApproved = true yap)
        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı." });
            }

            user.IsApproved = true;
            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "Kullanıcı onaylandı." });
        }
        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Kullanıcı silinemedi.", Errors = result.Errors });
            }

            return Ok(new { Message = "Kullanıcı başarıyla silindi." });
        }
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new { Message = "Kullanıcı bulunamadı." });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Rol güncellenemedi.", Errors = result.Errors });
            }

            return Ok(new { Message = "Kullanıcı rolü başarıyla güncellendi." });
        }
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsApproved = true 
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Message = "Kullanıcı oluşturulamadı.", Errors = result.Errors });
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { Message = "Kullanıcı başarıyla oluşturuldu." });
        }

    }
}
