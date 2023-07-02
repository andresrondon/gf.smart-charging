using SmartCharging.Lib.Exceptions;
using SmartCharging.Lib.Models;

namespace SmartCharging.Lib.Services;

internal static class BusinessRules
{
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

        if (!(connector.Id >= 1 && connector.Id <= 5))
        {
            errorList.Add($"Connector Id must be between 1 and 5. Actual: {connector.Id}");
        }

        if (parentStation?.Connectors.Count(c => c.Id == connector.Id) > 1)
        {
            errorList.Add($"Connector Id {connector.Id} already exists within Charge Station.");
        }

        if (connector.MaxCurrentInAmps <= 0)
        {
            errorList.Add($"{nameof(Connector.MaxCurrentInAmps)} must be greater than 0.");
        }

        var maxCurrentSum = parentGroup?.ChargeStations.Sum(cs => cs.Connectors.Where(c => c.Id != connector.Id).Sum(c => c.MaxCurrentInAmps));
        if (parentGroup is not null && maxCurrentSum + connector.MaxCurrentInAmps > parentGroup.CapacityInAmps)
        {
            errorList.Add($"Adding this connector's {nameof(Connector.MaxCurrentInAmps)} exceeds the Group's {nameof(Group.CapacityInAmps)}. Group's Capacity in Amps: {parentGroup.CapacityInAmps}. Sum of all other connectors' Max Current: {maxCurrentSum}.");
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
                throw new ValidationException(this);
            }
        }
    }
}
