using Evaluation.Application.Features.DTOs;
using Evaluation.Application.Interfaces;
using Evaluation.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Evaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PoliciesController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        // ---------------------------------------------------------
        // GET ALL POLICIES (Admin only)
        // ---------------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var policies = await _policyService.GetAllAsync(page, pageSize);
            return Ok(policies);
        }

        // ---------------------------------------------------------
        // GET POLICY BY ID
        // - Admin can see any policy
        // - Client can see only their own policies
        // ---------------------------------------------------------
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var policy = await _policyService.GetByIdAsync(id);
            if (policy == null)
                return NotFound("Policy not found.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            if (!roles.Contains("Admin"))
            {
                if (!await _policyService.ClientOwnsPolicyAsync(userId!, id))
                    return Forbid();
            }

            return Ok(policy);
        }

        // ---------------------------------------------------------
        // GET POLICIES FOR CLIENT LOGGED IN (Client only)
        // ---------------------------------------------------------
        [HttpGet("mine")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetMyPolicies()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _policyService.GetPoliciesForUserAsync(userId!);

            return Ok(result);
        }

        // ---------------------------------------------------------
        // CREATE POLICY (Admin only)
        // ---------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreatePolicyRequest request)
        {
            var created = await _policyService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ---------------------------------------------------------
        // UPDATE POLICY (Admin only)
        // ---------------------------------------------------------
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePolicyRequest request)
        {
            var updated = await _policyService.UpdateAsync(id, request);

            if (updated == null)
                return NotFound("Policy not found.");

            return Ok(updated);
        }

        // ---------------------------------------------------------
        // DELETE POLICY (Admin only)
        // ---------------------------------------------------------
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _policyService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Policy not found.");

            return NoContent();
        }

        // ---------------------------------------------------------
        // SEARCH POLICIES (Admin only)
        // ---------------------------------------------------------
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(
            [FromQuery] Guid? clientId, 
            [FromQuery] PolicyType? type,
            [FromQuery] PolicyStatus? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? policyNumber,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var results = await _policyService.SearchAsync(clientId: clientId,
                type: type,
                status: status,
                from: from,
                to: to,
                policyNumber: policyNumber,
                page: page,
                pageSize: pageSize);
            return Ok(results);
        }
    }
}
