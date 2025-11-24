using Evaluation.Application.Features.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Interfaces
{
    public interface IPolicyService
    {
        Task<IEnumerable<PolicyDto>> GetAllAsync(int page = 1, int pageSize = 20);

        Task<PolicyDto?> GetByIdAsync(Guid id);

        Task<PolicyDto> CreateAsync(CreatePolicyRequest request);

        Task<PolicyDto?> UpdateAsync(Guid id, UpdatePolicyRequest request);

        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<PolicySearchResultDto>> SearchAsync(string? policyNumber, string? clientName);

        // Validate if policy belongs to the client
        Task<bool> ClientOwnsPolicyAsync(string userId, Guid policyId);

        // Get all policies for a specific user
        Task<IEnumerable<PolicyDto>> GetPoliciesForUserAsync(string userId);
    }
}
