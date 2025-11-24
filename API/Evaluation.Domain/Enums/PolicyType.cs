using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluation.Domain.Enums
{
    /// <summary>
    /// Specifies the types of insurance policies supported by the system.
    /// </summary>
    public enum PolicyType
    {
        LifeInsurance = 1,
        HealthInsurance = 2,
        AutoInsurance = 3,
        HomeInsurance = 4,
    }
}