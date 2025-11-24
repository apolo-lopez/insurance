using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluation.Domain.Enums
{
    /// <summary>
    /// Specifies the status of a policy.
    /// </summary>
    /// <remarks>Use this enumeration to represent the current state of a policy, such as whether it is
    /// active, inactive, pending, or cancelled.</remarks>
    public enum PolicyStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3,
        Cancelled = 4,
    }
}