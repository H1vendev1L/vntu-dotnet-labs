using Pract02.CompanyEmployees.Exceptions;

namespace Pract02.CompanyEmployees.Models;

/// <summary>
/// Абстрактний базовий клас співробітника компанії.
/// </summary>
public abstract class Employee : IComparable<Employee>
{
    private string _fullName = string.Empty;
    private string _position = string.Empty;
    private int _experienceYears;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2)
            {
                throw new EmployeeValidationException(
                    "ПІБ співробітника не може бути порожнім або коротшим за 2 символи.",
                    nameof(FullName));
            }

            _fullName = value.Trim();
        }
    }

    public string Position
    {
        get => _position;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new EmployeeValidationException(
                    "Посада співробітника не може бути порожньою.",
                    nameof(Position));
            }

            _position = value.Trim();
        }
    }

    public int ExperienceYears
    {
        get => _experienceYears;
        set
        {
            if (value < 0 || value > 60)
            {
                throw new EmployeeValidationException(
                    $"Стаж роботи має бути в межах від 0 до 60 років (передано: {value}).",
                    nameof(ExperienceYears));
            }

            _experienceYears = value;
        }
    }

    protected Employee(string fullName, string position)
        : this(fullName, position, experienceYears: 0)
    {
    }

    protected Employee(string fullName, string position, int experienceYears)
    {
        FullName = fullName;
        Position = position;
        ExperienceYears = experienceYears;
    }

    /// <summary>
    /// Абстрактний метод розрахунку місячної заробітної плати (перевизначається у нащадках).
    /// </summary>
    public abstract decimal CalculateMonthlySalary();

    /// <summary>
    /// Віртуальний метод формування детального опису співробітника.
    /// </summary>
    public virtual string GetSummary()
    {
        return $"{FullName} | Посада: {Position} | Стаж: {ExperienceYears} р. | До виплати: {CalculateMonthlySalary():N2} грн";
    }

    public int CompareTo(Employee? other)
    {
        if (other is null)
        {
            return 1;
        }

        return CalculateMonthlySalary().CompareTo(other.CalculateMonthlySalary());
    }

    public override string ToString() => GetSummary();
}