using AutoMapper;
using Evaluation.Application.Features.DTOs;
using Evaluation.Domain.Entities;
using Evaluation.Domain.ValueObjects;

namespace Evaluation.API.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Domain to DTO mappings
            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber.Value));

            // ============================
            // CreateClientRequest → Client
            // ============================
            CreateMap<CreateClientRequest, Client>()
                .ConstructUsing(src =>
                    new Client(
                        new IdentificationNumber(src.IdentificationNumber),
                        src.Name,
                        src.Email,
                        src.PhoneNumber,
                        src.Address,
                        null // UserId is set to null on creation
                    ));

            // ============================
            // UpdateClientRequest → Client
            // Only map non-null properties
            // ============================
            CreateMap<UpdateClientRequest, Client>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null)
                );

            // Policy mapping
            CreateMap<Policy, PolicyDto>();

            // CreatePolicyRequest → Policy
            CreateMap<CreatePolicyRequest, Policy>()
                .ConstructUsing(src =>
                    new Policy(
                        src.PolicyNumber,
                        src.ClientId,
                        src.Type,
                        src.StartDate,
                        src.EndDate,
                        src.InsuredAmount,
                        src.Status
                    ));

            // Policy → PolicySearchResultDto
            CreateMap<Policy, PolicySearchResultDto>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId));
        }
    }
}
