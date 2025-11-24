using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evaluation.Domain.Common;
using Evaluation.Domain.Enums;

namespace Evaluation.Domain.Entities
{
    /// <summary>
    /// Represents an insurance policy, including details such as policy number, client, type, coverage period, insured
    /// amount, and status.
    /// </summary>
    /// <remarks>A Policy instance encapsulates the core information required to manage an insurance contract.
    /// The policy's state, coverage dates, and insured amount are immutable except through provided methods, ensuring
    /// consistency and validation of business rules. Use the available methods to update policy details or to cancel
    /// the policy as needed.</remarks>
    public class Policy : BaseEntity
    {
        public string PolicyNumber { get; private set; } = string.Empty;
        public Guid ClientId { get; private set; }
        public PolicyType PolicyType { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public decimal InsuredAmount { get; private set; }
        public PolicyStatus PolicyStatus { get; private set; }

        private Policy() { }

        public Policy(string policyNumber, Guid clientId, PolicyType policyType, DateTime startDate, DateTime endDate, decimal insuredAmount, PolicyStatus status)
        {
            if(string.IsNullOrWhiteSpace(policyNumber))
                throw new ArgumentException("Policy number cannot be null or empty.", nameof(policyNumber));

            if(endDate <= startDate)
                throw new ArgumentException("End date must be later than start date.", nameof(endDate));

            if(insuredAmount <= 0)
                throw new ArgumentException("Insured amount must be greater than zero.", nameof(insuredAmount));

            PolicyNumber = policyNumber;
            ClientId = clientId;
            PolicyType = policyType;
            StartDate = startDate;
            EndDate = endDate;
            InsuredAmount = insuredAmount;
            PolicyStatus = PolicyStatus.Active;
        }

        /// <summary>
        /// Cancels the policy if it is currently active.
        /// </summary>
        /// <remarks>After cancellation, the policy status is set to cancelled and cannot be reverted.
        /// This operation updates the policy's last modified timestamp.</remarks>
        public void Cancel()
        {
            if(PolicyStatus != PolicyStatus.Active)
                throw new InvalidOperationException("Only active policies can be cancelled.");

            PolicyStatus = PolicyStatus.Cancelled;
            UpdateTimestamp();
        }

        /// <summary>
        /// Updates the start and end dates for the current instance.
        /// </summary>
        /// <param name="start">The new start date to set. Must be earlier than <paramref name="end"/>.</param>
        /// <param name="end">The new end date to set. Must be later than <paramref name="start"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="end"/> is less than or equal to <paramref name="start"/>.</exception>
        public void UpdateDates(DateTime start, DateTime end)
        {
            if(end <= start)
                throw new ArgumentException("End date must be later than start date.", nameof(end));

            StartDate = start;
            EndDate = end;
            UpdateTimestamp();
        }

        /// <summary>
        /// Updates the insured amount to the specified value.
        /// </summary>
        /// <param name="amount">The new insured amount. Must be greater than zero.</param>
        /// <exception cref="ArgumentException">Thrown if the specified amount is less than or equal to zero.</exception>
        public void UpdateInsuredAmount(decimal amount)
        {
            if(amount <= 0)
                throw new ArgumentException("Insured amount must be greater than zero.", nameof(amount));

            InsuredAmount = amount;
            UpdateTimestamp();
        }

        public void SetPolicyNumber(string policyNumber)
        {
            if (string.IsNullOrWhiteSpace(policyNumber))
                throw new ArgumentException("Policy number cannot be null or empty.", nameof(policyNumber));

            PolicyNumber = policyNumber;
            UpdateTimestamp();
        }

        public void SetPolicyType(PolicyType policyType)
        {
            if (PolicyStatus == PolicyStatus.Cancelled)
                throw new InvalidOperationException("Cannot change type of a cancelled policy.");

            PolicyType = policyType;
            UpdateTimestamp();
        }

        public void SetPolicyStatus(PolicyStatus status)
        {
            if (PolicyStatus == PolicyStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a cancelled policy.");

            PolicyStatus = status;
            UpdateTimestamp();
        }

        public void SetStartDate(DateTime startDate)
        {
            if (startDate >= EndDate)
                throw new ArgumentException("Start date must be earlier than end date.", nameof(startDate));

            StartDate = startDate;
            UpdateTimestamp();
        }

        public void SetEndDate(DateTime endDate)
        {
            if (endDate <= StartDate)
                throw new ArgumentException("End date must be later than start date.", nameof(endDate));

            EndDate = endDate;
            UpdateTimestamp();
        }

        public void SetInsuredAmount(decimal insuredAmount)
        {
            if(insuredAmount <= 0)
                throw new ArgumentException("Insured amount must be greater than zero.", nameof(insuredAmount));

            InsuredAmount = insuredAmount;
            UpdateTimestamp();
        }
    }
}