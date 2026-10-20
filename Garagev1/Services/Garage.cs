using Garagev1.Models;

namespace Garagev1.Services;

public class Garage
{
    private readonly Vehicle?[] vehicles;

    public int Capacity => vehicles.Length;
    public int Count { get; private set; }

    public Garage(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
        }

        vehicles = new Vehicle?[capacity];
    }

    public OperationResult Park(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        if (FindByRegistrationNumber(vehicle.RegistrationNumber) is not null)
        {
            return new OperationResult(false, "A vehicle with this registration number is already parked.");
        }

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] is null)
            {
                vehicles[i] = vehicle;
                Count++;
                return new OperationResult(true, "Vehicle parked successfully.");
            }
        }

        return new OperationResult(false, "The garage is full.");
    }

    public OperationResult Remove(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return new OperationResult(false, "Registration number cannot be empty.");
        }

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] is Vehicle vehicle &&
                string.Equals(vehicle.RegistrationNumber, registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                vehicles[i] = null;
                Count--;
                return new OperationResult(true, "Vehicle removed successfully.");
            }
        }

        return new OperationResult(false, "No vehicle with this registration number was found.");
    }

    public Vehicle? FindByRegistrationNumber(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            return null;
        }

        return GetAllVehicles().FirstOrDefault(vehicle =>
            string.Equals(vehicle.RegistrationNumber, registrationNumber.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public Vehicle[] GetAllVehicles()
    {
        return vehicles.OfType<Vehicle>().ToArray();
    }

    public Dictionary<string, int> GetVehicleTypeCounts()
    {
        return GetAllVehicles()
            .GroupBy(vehicle => vehicle.GetType().Name)
            .ToDictionary(group => group.Key, group => group.Count());
    }


    public Vehicle[] Search(VehicleSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (criteria.NumberOfWheels < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(criteria), "Number of wheels cannot be negative.");
        }

        if (criteria.VehicleType is not null && !typeof(Vehicle).IsAssignableFrom(criteria.VehicleType))
        {
            throw new ArgumentException("Vehicle type must inherit from Vehicle.", nameof(criteria));
        }

        return GetAllVehicles().Where(vehicle =>
            (string.IsNullOrWhiteSpace(criteria.RegistrationNumber) ||
                string.Equals(vehicle.RegistrationNumber, criteria.RegistrationNumber.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(criteria.Color) ||
                string.Equals(vehicle.Color, criteria.Color.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            (!criteria.NumberOfWheels.HasValue || vehicle.NumberOfWheels == criteria.NumberOfWheels.Value) &&
            (criteria.VehicleType is null || criteria.VehicleType.IsInstanceOfType(vehicle)))
            .ToArray();
    }
}
