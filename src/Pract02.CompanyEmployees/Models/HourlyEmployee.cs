using Pract02.CompanyEmployees.Exceptions;

namespace Pract02.CompanyEmployees.Models;

/// <summary>
/// Співробітник із погодинною оплатою праці (понад 160 год — коефіцієнт 1.5).
/// </summary>
public class HourlyEmployee : Employee
{
    private const double StandardMonthlyHours = 160.0;
    private const double MaxMonthlyHours = 744.0; // Максимум годин у 31-денному місяці

    private decimal _hourlyRate;
    private double _hoursWorked;

    public decimal HourlyRate
    {
        get => _hourlyRate;
        set
        {
            if (value <= 0)
            {
                throw new EmployeeValidationException(
                    $"Погодинна ставка має бути більшою за 0 грн (передано: {value}).",
                    nameof(HourlyRate));
            }

            _hourlyRate = value;
        }
    }

    public double HoursWorked
    {
        get => _hoursWorked;
        set
        {
            if (value < 0 || value > MaxMonthlyHours)
            {
                throw new EmployeeValidationException(
                    $"Відпрацьовані години мають бути в межах від 0 до {MaxMonthlyHours} год (передано: {value}).",
                    nameof(HoursWorked));
            }

            _hoursWorked = value;
        }
    }

    public HourlyEmployee(string fullName, string position, decimal hourlyRate, double hoursWorked)
        : this(fullName, position, experienceYears: 0, hourlyRate, hoursWorked)
    {
    }

    public HourlyEmployee(
        string fullName,
        string position,
        int experienceYears,
        decimal hourlyRate,
        double hoursWorked)
        : base(fullName, position, experienceYears)
    {
        HourlyRate = hourlyRate;
        HoursWorked = hoursWorked;
    }

    public override decimal CalculateMonthlySalary()
    {
        if (HoursWorked <= StandardMonthlyHours)
        {
            return HourlyRate * (decimal)HoursWorked;
        }

        double overtimeHours = HoursWorked - StandardMonthlyHours;
        decimal regularPay = HourlyRate * (decimal)StandardMonthlyHours;
        decimal overtimePay = HourlyRate * 1.5m * (decimal)overtimeHours;

        return regularPay + overtimePay;
    }

    public override string GetSummary()
    {
        return $"[Погодинний] {base.GetSummary()} (ставка: {HourlyRate:N2} грн/год, відпрацьовано: {HoursWorked:F1} год)";
    }
}