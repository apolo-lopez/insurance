using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Application.Features.DTOs.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }
        public string Email { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public IEnumerable<string> Roles { get; set; } = default!;
    }
}
