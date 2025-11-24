using Evaluation.Application.Interfaces;
using Evaluation.Domain.Entities;
using Evaluation.Domain.Enums;
using Evaluation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Evaluation.Infrastructure.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly AppDbContext _dbContext;
        public PolicyRepository(AppDbContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Policy policy, CancellationToken cancellationToken = default)
        {
            await _dbContext.Policies.AddAsync(policy, cancellationToken);
        }

        public async Task<IEnumerable<Policy>> GetAllPoliciesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Policies
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Policy?> GetPolicyByIdAsync(Guid policyId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Policies
                .FindAsync(new object[] { policyId }, cancellationToken);
        }

        public async Task<Policy?> GetPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Policies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber, cancellationToken);
        }

        public void Update(Policy policy) => _dbContext.Policies.Update(policy);

        public void Remove(Policy policy) => _dbContext.Policies.Remove(policy);

        public async Task<IEnumerable<Policy>> FindAsync(
            Guid? clientId = null,
            PolicyType? type = null,
            PolicyStatus? status = null,
            DateTime? from = null,
            DateTime? to = null,
            string? policyNumber = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default
            )
        {
            var query = _dbContext.Policies.AsQueryable();

            if (clientId.HasValue)
                query = query.Where(p => p.ClientId == clientId.Value);

            if (type.HasValue)
                query = query.Where(p => p.PolicyType == type.Value);

            if (status.HasValue)
                query = query.Where(p => p.PolicyStatus == status.Value);

            if (from.HasValue)
                query = query.Where(p => p.StartDate >= from.Value);

            if (to.HasValue)
                query = query.Where(p => p.EndDate <= to.Value);

            if (!string.IsNullOrWhiteSpace(policyNumber))
                query = query.Where(p => p.PolicyNumber.Contains(policyNumber));

            return await query
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Policies.AnyAsync(p => p.Id == id, cancellationToken);
        }
    }
}
