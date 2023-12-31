﻿using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services;

/// <summary>
/// Helper class for validating business logic on the domain models.
/// </summary>
internal static class BusinessRules
{
    private const int MinConnectorsPerStation = 1;
    private const int MaxConnectorsPerStation = 5;

    /// <summary>
    /// Validates that <see cref="Group"/> entity to be added or modified complies with all the business rules.
    /// </summary>
    internal static ValidationMessageList ValidateGroupUpdate(Group group)
    {
        var maxCurrentSum = group.ChargeStations.Sum(cs => cs.MaxCurrentInAmpsSum);
        
        return new ValidationMessageList()
            .AddIf(group.CapacityInAmps <= 0, $"{nameof(Group.CapacityInAmps)} must be greater than 0.")
            .AddIf(maxCurrentSum > group.CapacityInAmps, $"{nameof(Group.CapacityInAmps)} cannot be less than the sum of the {nameof(Connector.MaxCurrentInAmps)} of all {nameof(Connector)}s indirectly belonging to the {nameof(Group)}. Group's Capacity in Amps: {group.CapacityInAmps}. Sum of all its connectors' Max Current: {maxCurrentSum}.");
    }

    /// <summary>
    /// Validates that <see cref="Connector"/> entity to be added or modified complies with all the business rules.
    /// </summary>
    internal static ValidationMessageList ValidateConnectorUpdate(Connector connector, Group parentGroup, ChargeStation parentStation)
    {
        var maxCurrentSum = parentGroup.ChargeStations.Sum(cs => cs.Connectors.Where(c => cs.Id != parentStation.Id || c.Id != connector.Id).Sum(c => c.MaxCurrentInAmps)) + connector.MaxCurrentInAmps;
        
        return new ValidationMessageList()
            .AddIf(parentStation.Connectors.Count > MaxConnectorsPerStation, $"A station cannot have more than {MaxConnectorsPerStation} connectors.")
            .AddIf(parentStation.Connectors.Count(c => c.Id == connector.Id) > 1, $"Connector Id {connector.Id} already exists within this Charge Station.")
            .AddIf(connector.MaxCurrentInAmps <= 0, $"{nameof(Connector.MaxCurrentInAmps)} must be greater than 0.")
            .AddIf(maxCurrentSum > parentGroup.CapacityInAmps, $"Adding this connector's {nameof(Connector.MaxCurrentInAmps)} exceeds the Group's {nameof(Group.CapacityInAmps)}. Group's Capacity in Amps: {parentGroup.CapacityInAmps}. Sum of all other connectors' Max Current: {maxCurrentSum}.");
    }

    /// <summary>
    /// Validates that <see cref="ChargeStation"/> entity to be added or modified complies with all the business rules.
    /// </summary>
    internal static ValidationMessageList ValidateChargeStationUpdate(ChargeStation station, Group parentGroup)
    {
        var maxCurrentSum = parentGroup.ChargeStations.Where(cs => cs.Id != station.Id).Sum(cs => cs.MaxCurrentInAmpsSum) + station.MaxCurrentInAmpsSum;
        
        return new ValidationMessageList()
            .AddIf(station.Connectors.Count < MinConnectorsPerStation, $"A station must have at least {MinConnectorsPerStation} connector.")
            .AddIf(station.Connectors.Count > MaxConnectorsPerStation, $"A station cannot have more than {MaxConnectorsPerStation} connectors.")
            .AddIf(station.Connectors.DistinctBy(c => c.Id).Count() != station.Connectors.Count, "Connector Ids must be unique.")
            .AddIf(station.Connectors.Any(c => c.MaxCurrentInAmps <= 0), $"{nameof(Connector.MaxCurrentInAmps)} must be greater than 0.")
            .AddIf(maxCurrentSum > parentGroup.CapacityInAmps, $"The sum of the connectors' {nameof(Connector.MaxCurrentInAmps)} exceeds the Group's {nameof(Group.CapacityInAmps)}. Group's Capacity in Amps: {parentGroup.CapacityInAmps}. Sum of all other connectors' Max Current: {maxCurrentSum}.");
    }

    /// <summary>
    /// A list of errors that spawned during the process of validating business rules.
    /// </summary>
    internal class ValidationMessageList : List<string>
    {
        public bool IsValid => Count == 0;

        /// <summary>
        /// Adds a message to the list only if the <paramref name="condition"/> is met.
        /// </summary>
        public ValidationMessageList AddIf(bool condition, string message)
        {
            if (condition) Add(message);
            return this;
        }

        /// <summary>
        /// Throws a <see cref="BusinessRulesValidationException"/> if the list contains one or more error messages.
        /// </summary>
        /// <exception cref="BusinessRulesValidationException"></exception>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                throw new BusinessRulesValidationException(this);
            }
        }
    }
}
