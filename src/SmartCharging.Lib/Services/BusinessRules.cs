using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services;

internal static class BusinessRules
{
    private const int MinConnectorsPerStation = 1;
    private const int MaxConnectorsPerStation = 5;

    internal static ValidationMessageList ValidateGroupUpdate(Group group)
    {
        var errorList = new ValidationMessageList();

        if (group.CapacityInAmps <= 0)
        {
            errorList.Add($"{nameof(Group.CapacityInAmps)} must be greater than 0.");
        }

        var maxCurrentSum = group.ChargeStations.Sum(cs => cs.Connectors.Sum(c => c.MaxCurrentInAmps));
        if (maxCurrentSum > group.CapacityInAmps)
        {
            errorList.Add($"{nameof(Group.CapacityInAmps)} cannot be less than the sum of the {nameof(Connector.MaxCurrentInAmps)} of all {nameof(Connector)}s indirectly belonging to the {nameof(Group)}. Group's Capacity in Amps: {group.CapacityInAmps}. Sum of all its connectors' Max Current: {maxCurrentSum}.");
        }

        return errorList;
    }

    internal static ValidationMessageList ValidateConnectorUpdate(Connector connector, Group parentGroup, ChargeStation parentStation)
    {
        var errorList = new ValidationMessageList();

        if (parentStation.Connectors.Count > MaxConnectorsPerStation)
        {
            errorList.Add($"A station cannot have more than {MaxConnectorsPerStation} connectors.");
        }

        if (parentStation.Connectors.Count(c => c.Id == connector.Id) > 1)
        {
            errorList.Add($"Connector Id {connector.Id} already exists within this Charge Station.");
        }

        if (connector.MaxCurrentInAmps <= 0)
        {
            errorList.Add($"{nameof(Connector.MaxCurrentInAmps)} must be greater than 0.");
        }

        var maxCurrentSum = parentGroup.ChargeStations.Sum(cs => cs.Connectors.Where(c => cs.Id != parentStation.Id || c.Id != connector.Id).Sum(c => c.MaxCurrentInAmps));
        if (maxCurrentSum + connector.MaxCurrentInAmps > parentGroup.CapacityInAmps)
        {
            errorList.Add($"Adding this connector's {nameof(Connector.MaxCurrentInAmps)} exceeds the Group's {nameof(Group.CapacityInAmps)}. Group's Capacity in Amps: {parentGroup.CapacityInAmps}. Sum of all other connectors' Max Current: {maxCurrentSum}.");
        }

        return errorList;
    }

    internal static ValidationMessageList ValidateChargeStationUpdate(ChargeStation station, Group parentGroup)
    {
        var errorList = new ValidationMessageList();

        if (station.Connectors.Count < MinConnectorsPerStation)
        {
            errorList.Add($"A station must have at least {MinConnectorsPerStation} connector.");
        }

        if (station.Connectors.Count > MaxConnectorsPerStation)
        {
            errorList.Add($"A station cannot have more than {MaxConnectorsPerStation} connectors.");
        }

        if (station.Connectors.DistinctBy(c => c.Id).Count() != station.Connectors.Count)
        {
            errorList.Add("Connector Ids must be unique.");
        }

        if (station.Connectors.Any(c => c.MaxCurrentInAmps <= 0))
        {
            errorList.Add($"{nameof(Connector.MaxCurrentInAmps)} must be greater than 0.");
        }

        var maxCurrentSum = parentGroup.ChargeStations.Where(cs => cs.Id != station.Id).Sum(cs => cs.Connectors.Sum(c => c.MaxCurrentInAmps));
        if (maxCurrentSum + station.Connectors.Sum(c => c.MaxCurrentInAmps) > parentGroup.CapacityInAmps)
        {
            errorList.Add($"The sum of the connectors' {nameof(Connector.MaxCurrentInAmps)} exceeds the Group's {nameof(Group.CapacityInAmps)}. Group's Capacity in Amps: {parentGroup.CapacityInAmps}. Sum of all other connectors' Max Current: {maxCurrentSum}.");
        }

        return errorList;
    }

    internal class ValidationMessageList : List<string>
    {
        public bool IsValid => Count == 0;

        public void ThrowIfInValid()
        {
            if (!IsValid)
            {
                throw new BusinessRulesValidationException(this);
            }
        }
    }
}
