
namespace Garagev1.Models;

public abstract class Vehicle
{
    public string RegistrationNumber { get; }
    public string Color { get; }
    public int NumberOfWheels { get; }

    protected Vehicle(string registrationNumber, string color, int numberOfWheels)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
        {
            throw new ArgumentException("Registration number cannot be empty.", nameof(registrationNumber));
        }

        if (string.IsNullOrWhiteSpace(color))
        {
            throw new ArgumentException("Color cannot be empty.", nameof(color));
        }

        if (numberOfWheels < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfWheels), "Number of wheels cannot be negative.");
        }

        RegistrationNumber = registrationNumber.Trim();
        Color = color.Trim();
        NumberOfWheels = numberOfWheels;
    }

    public override string ToString()
    {
        return $"{GetType().Name} | Registration number: {RegistrationNumber} | Color: {Color} | Number of wheels: {NumberOfWheels}";
    }
}

