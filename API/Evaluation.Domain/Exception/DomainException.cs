using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluation.Domain.Exception
{
    /// <summary>
    /// Represents errors that occur during application domain operations.
    /// </summary>
    /// <remarks>Use this exception to indicate a violation of business rules or domain logic within the
    /// application. DomainException is intended to be thrown when an operation cannot be completed due to
    /// domain-specific constraints, rather than technical or infrastructure errors.</remarks>
    public class DomainException : System.Exception
    {
        public DomainException() { }
        public DomainException(string message) : base(message) { }
        public DomainException(string message, System.Exception inner) : base(message, inner) { }
    }
}