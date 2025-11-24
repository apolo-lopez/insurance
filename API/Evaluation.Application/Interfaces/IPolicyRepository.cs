using Evaluation.Domain.Entities;
using Evaluation.Domain.Enums;

namespace Evaluation.Application.Interfaces
{
    public interface IPolicyRepository
    {
        Task<Policy?> GetPolicyByIdAsync(Guid policyId, CancellationToken cancellationToken = default!);
        Task<Policy?> GetPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default!);
        Task<IEnumerable<Policy>> GetAllPoliciesAsync(int page = 1, int pageSize = 20,CancellationToken cancellationToken = default!);
        Task AddAsync(Policy policy, CancellationToken cancellationToken = default!);
        void Update(Policy policy);
        void Remove(Policy policy);
        Task<bool> ExistsAsync(Guid policyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Policy>> FindAsync(
            Guid? clientId = null,
            PolicyType? type = null,
            PolicyStatus? status = null,
            DateTime? from = null,
            DateTime? to = null,
            string? policyNumber = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default
            );
    }
}
