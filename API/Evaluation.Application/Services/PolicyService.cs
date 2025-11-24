using AutoMapper;
using Evaluation.Application.Features.DTOs;
using Evaluation.Application.Interfaces;
using Evaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evaluation.Application.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PolicyService(IPolicyRepository policyRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _policyRepository = policyRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ClientOwnsPolicyAsync(string userId, Guid policyId)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId);
            return policy != null && policy.ClientId.ToString() == userId;
        }

        public async Task<PolicyDto> CreateAsync(CreatePolicyRequest request)
        {
            var policy = new Policy(
                request.PolicyNumber,
                request.ClientId,
                request.Type,
                request.StartDate.ToUniversalTime(),
                request.EndDate.ToUniversalTime(),
                request.InsuredAmount,
                request.Status
            );

            await _policyRepository.AddAsync(policy);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PolicyDto>(policy);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(id);
            if (policy == null) return false;

            _policyRepository.Remove(policy);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PolicyDto>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            var policies = await _policyRepository.GetAllPoliciesAsync(page, pageSize);
            return _mapper.Map<IEnumerable<PolicyDto>>(policies);
        }

        public async Task<PolicyDto?> GetByIdAsync(Guid id)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(id);
            return policy == null ? null : _mapper.Map<PolicyDto>(policy);
        }

        public async Task<IEnumerable<PolicyDto>> GetPoliciesForUserAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var clientGuid))
                throw new ArgumentException("Invalid user ID format.", nameof(userId));

            var policies = await _policyRepository.FindAsync(clientId: clientGuid);
            return _mapper.Map<IEnumerable<PolicyDto>>(policies);
        }

        public async Task<IEnumerable<PolicySearchResultDto>> SearchAsync(string? policyNumber, string? clientName)
        {
            // Aquí usamos FindAsync para buscar por número de póliza
            var policies = await _policyRepository.FindAsync(policyNumber: policyNumber);
            // Si quieres filtrar por nombre del cliente, tendrías que hacer join con la tabla Clients
            return _mapper.Map<IEnumerable<PolicySearchResultDto>>(policies);
        }

        public async Task<PolicyDto?> UpdateAsync(Guid id, UpdatePolicyRequest request)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(id);
            if (policy == null) throw new KeyNotFoundException("Policy not found.");

            if (!string.IsNullOrEmpty(request.PolicyNumber))
                policy.SetPolicyNumber(request.PolicyNumber);

            if (request.StartDate.HasValue)
                policy.SetStartDate(request.StartDate.Value.ToUniversalTime());

            if (request.EndDate.HasValue)
                policy.SetEndDate(request.EndDate.Value.ToUniversalTime());

            if (request.InsuredAmount.HasValue)
                policy.SetInsuredAmount(request.InsuredAmount.Value);

            if(request.Status.HasValue)
                policy.SetPolicyStatus(request.Status.Value);

            if (request.Type.HasValue)
                policy.SetPolicyType(request.Type.Value);

            _policyRepository.Update(policy);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PolicyDto>(policy);
        }
    }
}
