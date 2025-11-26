using Evaluation.Application.Features.DTOs;
using Evaluation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IClientService _clientService;

        public ProfileController(UserManager<IdentityUser> userManager, IClientService clientService)
        {
            _clientService = clientService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Busca el cliente por UserId
            var client = await _clientService.GetByUserIdAsync(userId);
            if (client == null) return NotFound();

            return Ok(client);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateClientRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Busca el cliente por UserId
            var client = await _clientService.GetByUserIdAsync(userId);
            if (client == null) return NotFound();

            // Usa tu método existente para actualizar
            var updated = await _clientService.UpdateAsync(client.Id, request);
            return Ok(updated);
        }
    }
}
