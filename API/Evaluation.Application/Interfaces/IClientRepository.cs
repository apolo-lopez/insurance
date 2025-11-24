using Evaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Client?> GetByIdentificationNumberAsync(string identificationNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<Client>> GetAllAsync(int page, int pageSize = 20, CancellationToken cancellationToken = default);
        Task AddAsync(Client client, CancellationToken cancellationToken = default);
        void Update(Client client);
        void Remove(Client client);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Client>> SearchAsync(
            string? name = null,
            string? email = null,
            string? identificationNumber = null,
            string? phoneNumber = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);
    }
}
