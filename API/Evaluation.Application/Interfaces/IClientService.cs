using Evaluation.Application.Features.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllClientsAsync(int page, int pageSize = 20);
        Task<ClientDto?> GetByIdAsync(Guid id);
        Task<ClientDto> CreateAsync(CreateClientRequest request);
        Task<ClientDto> UpdateAsync(Guid id, UpdateClientRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ClientDto>> SearchAsync(string? name = null, string? email = null, string? identificationNumber = null, string? phoneNumber = null);
        Task<bool> ClientExistAsync(Guid id);
    }
}
