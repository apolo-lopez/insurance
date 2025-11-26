using Evaluation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Features.DTOs
{
    public class PolicySearchResultDto
    {
        public Guid Id { get; set; }

        public string PolicyNumber { get; set; } = default!;

        public PolicyType Type { get; set; }

        public PolicyStatus Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal InsuredAmount { get; set; }

        public Guid ClientId { get; set; }
    }
}
