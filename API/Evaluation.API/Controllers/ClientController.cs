using Evaluation.Application.Features.DTOs;
using Evaluation.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
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
            var created = await _clientService.CreateAsync(request);
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
