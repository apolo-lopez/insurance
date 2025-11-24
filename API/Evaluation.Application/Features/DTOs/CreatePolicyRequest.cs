using Evaluation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Features.DTOs
{
    public class CreatePolicyRequest
    {
        public Guid ClientId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public PolicyType Type { get; set; }
        public PolicyStatus Status { get; set; }
        public DateTime StartDate { get; set; }        
        public DateTime EndDate { get; set; }        
        public decimal InsuredAmount { get; set; }
    }
}
