using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluation.Application.Features.DTOs
{    
    public class ClientDto
    {
        public Guid Id { get; set; }
        public required string IdentificationNumber { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? UserId { get; set; }
    }
}