using Evaluation.Application.Interfaces;
using Evaluation.Domain.Entities;
using Evaluation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Evaluation.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Client client, CancellationToken cancellationToken = default)
        {
            await _dbContext.Clients.AddAsync(client, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Client>> GetAllAsync(int page, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Client?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients.FirstOrDefaultAsync(x => x.UserId == userId.ToString(), cancellationToken);
        }

        public async Task<Client?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentificationNumber.Value == identificationNumber, cancellationToken);
        }

        public void Remove(Client client) => _dbContext.Clients.Remove(client);

        public async Task<IEnumerable<Client>> SearchAsync(string? name = null, string? email = null, string? identificationNumber = null, string? phoneNumber = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalized = name.Trim();
                query = query.Where(c => EF.Functions.ILike(c.Name, $"%{normalized}%"));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var normalized = email.Trim();
                query = query.Where(c => EF.Functions.ILike(c.Email, $"%{normalized}%"));
            }

            if (!string.IsNullOrWhiteSpace(identificationNumber))
            {
                var idn = identificationNumber.Trim();
                // Use shadow property or owned property access depending on your EF mapping:
                // If you used OwnsOne with property Value, use EF.Property<string>(c, "IdentificationNumber")
                query = query.Where(c => c.IdentificationNumber.Value == idn);
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var phone = phoneNumber.Trim();
                query = query.Where(c => EF.Functions.ILike(c.PhoneNumber, $"%{phone}%"));
            }

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            return await query
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public void Update(Client client) => _dbContext.Clients.Update(client);
    }
}
