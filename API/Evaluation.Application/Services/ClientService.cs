using AutoMapper;
using Evaluation.Application.Features.DTOs;
using Evaluation.Application.Interfaces;
using Evaluation.Domain.Entities;
using Evaluation.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IClientRepository clientRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ClientExistAsync(Guid id)
        {
            return await _clientRepository.ExistsAsync(id);
        }

        public async Task<ClientDto> CreateAsync(CreateClientRequest request)
        {
            var identification = new IdentificationNumber(request.IdentificationNumber);

            var client = new Client(
                identification,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Address,
                null
                );

            await _clientRepository.AddAsync(client);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client is null)
                return false;

            _clientRepository.Remove(client);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync(int page, int pageSize = 20)
        {
            var clients = await _clientRepository.GetAllAsync(page, pageSize);
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto?> GetByIdAsync(Guid id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client is null ? null : _mapper.Map<ClientDto>(client);
        }

        public async Task<IEnumerable<ClientDto>> SearchAsync(string? name = null, string? email = null, string? identificationNumber = null, string? phoneNumber = null)
        {
            var clients = await _clientRepository.SearchAsync(name, email, identificationNumber, phoneNumber);
            return _mapper.Map<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ClientDto> UpdateAsync(Guid id, UpdateClientRequest request)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if(client is null)
            {
                throw new KeyNotFoundException($"Client not found.");
            }

            if (request.IdentificationNumber != null)
            {
                client.IdentificationNumber = new IdentificationNumber(request.IdentificationNumber);
            }

            if(request.Name != null)
            {
                client.SetFullName(request.Name);
            }

            if(request.Email != null)
            {
                client.SetEmail(request.Email);
            }

            if(request.PhoneNumber != null)
            {
                client.SetPhoneNumber(request.PhoneNumber);
            }

            if(request.Address != null)
            {
                client.SetAddress(request.Address);
            }

            _clientRepository.Update(client);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ClientDto>(client);
        }
    }
}
