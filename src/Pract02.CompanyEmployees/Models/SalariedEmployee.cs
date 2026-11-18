using Pract02.CompanyEmployees.Exceptions;

namespace Pract02.CompanyEmployees.Models;

/// <summary>
/// Штатний співробітник із фіксованим місячним окладом та премією.
/// </summary>
public class SalariedEmployee : Employee
{
    private decimal _baseSalary;
    private decimal _bonusPercent;

    public decimal BaseSalary
    {
        get => _baseSalary;
        set
        {
            if (value <= 0)
            {
                throw new EmployeeValidationException(
                    $"Базовий оклад має бути більшим за 0 грн (передано: {value}).",
                    nameof(BaseSalary));
            }

            _baseSalary = value;
        }
    }

    public decimal BonusPercent
    {
        get => _bonusPercent;
        set
        {
            if (value < 0 || value > 200)
            {
                throw new EmployeeValidationException(
                    $"Відсоток премії має бути в межах від 0% до 200% (передано: {value}%).",
                    nameof(BonusPercent));
            }

            _bonusPercent = value;
        }
    }

    public SalariedEmployee(string fullName, string position, decimal baseSalary)
        : this(fullName, position, experienceYears: 0, baseSalary, bonusPercent: 0m)
    {
    }

    public SalariedEmployee(
        string fullName,
        string position,
        int experienceYears,
        decimal baseSalary,
        decimal bonusPercent)
        : base(fullName, position, experienceYears)
    {
        BaseSalary = baseSalary;
        BonusPercent = bonusPercent;
    }

    public override decimal CalculateMonthlySalary()
    {
        return BaseSalary * (1m + BonusPercent / 100m);
    }

    public override string GetSummary()
    {
        return $"[Штатний] {base.GetSummary()} (оклад: {BaseSalary:N2} грн, премія: {BonusPercent:F1}%)";
    }
}