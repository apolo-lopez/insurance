using Evaluation.Application.Interfaces;
using Evaluation.Infrastructure.Data;

namespace Evaluation.Infrastructure.UOF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public IClientRepository Clients { get; }
        public IPolicyRepository Policies { get; }

        public UnitOfWork(AppDbContext dbContext, IClientRepository clients, IPolicyRepository policies)
        {
            _dbContext = dbContext;
            Clients = clients;
            Policies = policies;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
