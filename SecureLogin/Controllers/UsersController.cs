using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureLogin.Model;

namespace SecureLogin.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserDbContext db, ILogger<UsersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // 1. Adding a new user (Create)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            user.Id = Guid.NewGuid();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            LogAction("Create", user.UserName);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        // 2. Updating user’s data (Update)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] User updatedUser)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.FullName = updatedUser.FullName;
            user.MobileNumber = updatedUser.MobileNumber;
            user.Language = updatedUser.Language;
            user.Culture = updatedUser.Culture;
            // Ažurirati samo polja koja su dozvoljena

            await _db.SaveChangesAsync();
            LogAction("Update", id.ToString());
            return NoContent();
        }

        // 3. Deleting user (Delete)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            LogAction("Delete", id.ToString());
            return Ok();
        }

        // 4. Retrieving user’s data (Read)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            LogAction("Get", id.ToString());

            if (user == null) return NotFound();
            return Ok(user);
        }

        // 5. Validating user’s password (Validate)
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] LoginDto login)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.UserName == login.UserName);

            LogAction("Validate", login.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { Status = "Valid", UserId = user.Id });
        }

        private void LogAction(string method, string @params)
        {
            var clientName = HttpContext.Items["ClientName"] as string ?? "Unknown";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Method: {Method}, Params: {Params}, Client: {Client}, IP: {IP}",
                method, @params, clientName, ip);
        }

    }
}
