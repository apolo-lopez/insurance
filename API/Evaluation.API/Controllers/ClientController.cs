using Evaluation.Application.Features.DTOs;
using Evaluation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientController(IClientService clientService, UserManager<IdentityUser> userManager)
        {
            _clientService = clientService;
            _userManager = userManager;
        }

        // ----------------------------------------------
        // GET ALL CLIENTS (Only admin)
        // ----------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllClients([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var clients = await _clientService.GetAllClientsAsync(page, pageSize);
            return Ok(clients);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetClientById(Guid id)
        {
            var userId = User.FindFirst("sub")?.Value;
            var roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

            var client = await _clientService.GetByIdAsync(id);

            if (client == null)
                return NotFound("Client not found.");

            // If not admin, can only access own client info
            if (!roles.Contains("Admin") && client.UserId != userId)
                return Forbid();

            return Ok(client);
        }

        // ---------------------------------------------------------
        // CREATE CLIENT (Admin only)
        // ---------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email y password son obligatorios.");
            }

            var newUser = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(newUser, request.Password);

            if (!createUserResult.Succeeded)
            {
                // Concatenamos las descripciones de los errores y devolvemos en una propiedad 'error'
                var friendlyError = string.Join(" ", createUserResult.Errors.Select(e => e.Description));
                var response = new
                {
                    success = false,
                    error = friendlyError
                };
                return BadRequest(response);
            }

            await _userManager.AddToRoleAsync(newUser, "Client");

            var clientDto = new CreateClientRequest
            {
                IdentificationNumber = request.IdentificationNumber,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                UserId = newUser.Id
            };

            var created = await _clientService.CreateAsync(clientDto);
            return CreatedAtAction(nameof(GetClientById), new { id = created.Id }, created);
        }

        // ---------------------------------------------------------
        // UPDATE CLIENT
        // - Admin can update any client
        // - Client can update their own info
        // ---------------------------------------------------------
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequest request)
        {
            var existing = await _clientService.GetByIdAsync(id);
            if (existing == null)
                return NotFound("Client not found.");

            var userId = User.FindFirst("sub")?.Value;
            var roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

            if (!roles.Contains("Admin") && existing.UserId != userId)
                return Forbid();

            var updated = await _clientService.UpdateAsync(id, request);
            return Ok(updated);
        }

        // ---------------------------------------------------------
        // DELETE CLIENT
        // Admin only
        // ---------------------------------------------------------
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _clientService.DeleteAsync(id);
            if (result == null)
                return NotFound("Client not found.");

            return NoContent();
        }

        // ---------------------------------------------------------
        // SEARCH CLIENTS (Admin only)
        // ---------------------------------------------------------
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] string? name = null, [FromQuery] string? email = null, [FromQuery] string? identificationNumber = null, [FromQuery] string? phoneNumber = null)
        {
            var results = await _clientService.SearchAsync(name, email, identificationNumber, phoneNumber);
            return Ok(results);
        }
    }
}
