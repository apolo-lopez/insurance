using Evaluation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluation.Application.Features.DTOs
{
    public class PolicyDto
    {       
        public Guid Id { get; set; }
        public string PolicyNumber { get; set; } = default!;
        public Guid ClientId { get; set; }
        public PolicyType PolicyType { get; set; }
        public PolicyStatus PolicyStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal InsuredAmount { get; set; }
    }
}