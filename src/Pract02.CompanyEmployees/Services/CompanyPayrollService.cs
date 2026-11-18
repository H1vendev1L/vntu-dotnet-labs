using Pract02.CompanyEmployees.Models;

namespace Pract02.CompanyEmployees.Services;

/// <summary>
/// Сервіс управління штатом співробітників та розрахунку фонду оплати праці.
/// </summary>
public sealed class CompanyPayrollService
{
    private readonly List<Employee> _employees;

    public CompanyPayrollService()
    {
        // Попереднє заповнення поліморфної колекції об'єктами різних похідних класів
        _employees =
        [
            new SalariedEmployee("Коваленко Олександр Петрович", "Senior .NET Developer", 5, 65000m, 15m),
            new HourlyEmployee("Мельник Ірина Сергіївна", "UI/UX Designer", 3, 420m, 172.5),
            new CommissionEmployee("Бондаренко Дмитро Ігорович", "Account Executive", 4, 22000m, 340000m, 8m),
            new SalariedEmployee("Ткаченко Марія Вікторівна", "QA Automation Engineer", 2, 42000m, 10m),
            new HourlyEmployee("Савченко Артем Олегович", "DevOps Consultant", 6, 650m, 120.0)
        ];
    }

    public IReadOnlyList<Employee> GetAll() => _employees;

    public void AddEmployee(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        _employees.Add(employee);
    }

    /// <summary>
    /// Поліморфний підрахунок загального фонду оплати праці через foreach.
    /// </summary>
    public decimal CalculateTotalPayroll()
    {
        decimal total = 0m;

        foreach (Employee employee in _employees)
        {
            total += employee.CalculateMonthlySalary();
        }

        return total;
    }

    /// <summary>
    /// Повертає відсортовану копію списку з використанням IComparable&lt;Employee&gt;.
    /// </summary>
    public List<Employee> GetSortedBySalary(bool descending = false)
    {
        List<Employee> sorted = [.. _employees];
        sorted.Sort(); // Використовує Employee.CompareTo(Employee?)

        if (descending)
        {
            sorted.Reverse();
        }

        return sorted;
    }
}