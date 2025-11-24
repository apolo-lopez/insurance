using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Evaluation.Domain.ValueObjects
{
    /// <summary>
    /// Represents a validated identification number consisting of exactly 10 digits.
    /// </summary>
    /// <remarks>An IdentificationNumber instance is immutable and guarantees that the value contains only
    /// digits and is exactly 10 characters long. Use this type to ensure identification numbers are consistently
    /// validated and formatted throughout your application.</remarks>
    public sealed class IdentificationNumber
    {
        public string Value { get; } = string.Empty;

        public IdentificationNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception.DomainException("Identification number cannot be null or empty.");
            }

            var digits = Regex.Replace(value, @"[^\d]", "");
            if (digits.Length != 10)
            {
                throw new Exception.DomainException("Identification number must have exactly 10 digits.");
            }

            Value = digits;
        }

        public override string ToString() => Value;
    }
}