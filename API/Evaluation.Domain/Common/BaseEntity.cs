using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evaluation.Domain.Common
{
    /// <summary>
    /// Represents a base entity with common properties for identification and auditing.
    /// </summary>
    /// <remarks>This class is intended to be used as a base type for entities that require a unique
    /// identifier and basic timestamp tracking. Derived classes can extend this type to include additional
    /// domain-specific properties.</remarks>
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
    }
}