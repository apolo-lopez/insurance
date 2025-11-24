using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Domain.Common;
using Evaluation.Domain.ValueObjects;

namespace Evaluation.Domain.Entities
{
    /// <summary>
    /// Represents a client entity with identification, contact information, and optional user association.
    /// </summary>
    /// <remarks>The Client class encapsulates the core details of a client, including identification number,
    /// name, email, phone number, and optional address and user ID. It provides methods to update individual fields
    /// while enforcing basic validation rules. Instances of Client are typically created and managed within the
    /// application's domain layer.</remarks>
    public class Client : BaseEntity
    {
        public IdentificationNumber IdentificationNumber { get; set; } = null!;
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string? Address { get; private set; } = string.Empty;
        public string? UserId { get; private set; } = null;

        private Client() { }

        public Client(
            IdentificationNumber identificationNumber,
            string name,
            string email,
            string phoneNumber,
            string? address,
            string? userId)
        {
            IdentificationNumber = identificationNumber;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            UserId = userId;
        }


        public void SetFullName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new Exception.DomainException("Name cannot be null or empty.");
            }

            if(name.Any(char.IsDigit))
            {
                throw new Exception.DomainException("Name cannot contain numbers.");
            }

            Name = name.Trim();
            UpdateTimestamp();
        }

        public void SetEmail(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                throw new Exception.DomainException("Email cannot be null or empty.");
            }

            if(!email.Contains("@"))
            {
                throw new Exception.DomainException("Email is not valid.");
            }

            Email = email.Trim();
            UpdateTimestamp();
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            if(string.IsNullOrEmpty(phoneNumber))
            {
                throw new Exception.DomainException("Phone number cannot be null or empty.");
            }

            PhoneNumber = phoneNumber.Trim();
            UpdateTimestamp();
        }

        public void SetAddress(string? address)
        {
            Address = address?.Trim();
            UpdateTimestamp();
        }

        public void SetUserId(string? userId)
        {
            UserId = userId;
            UpdateTimestamp();
        }       
    }
}