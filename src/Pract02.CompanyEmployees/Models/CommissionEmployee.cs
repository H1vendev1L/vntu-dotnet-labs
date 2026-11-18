using Pract02.CompanyEmployees.Exceptions;

namespace Pract02.CompanyEmployees.Models;

/// <summary>
/// Менеджер з продажу: фіксована ставка плюс відсоток від залучених продажів.
/// </summary>
public class CommissionEmployee : Employee
{
    private decimal _baseRetainer;
    private decimal _salesAmount;
    private decimal _commissionRatePercent;

    public decimal BaseRetainer
    {
        get => _baseRetainer;
        set
        {
            if (value < 0)
            {
                throw new EmployeeValidationException(
                    $"Базова ставка не може бути від'ємною (передано: {value}).",
                    nameof(BaseRetainer));
            }

            _baseRetainer = value;
        }
    }

    public decimal SalesAmount
    {
        get => _salesAmount;
        set
        {
            if (value < 0)
            {
                throw new EmployeeValidationException(
                    $"Обсяг продажів не може бути від'ємним (передано: {value}).",
                    nameof(SalesAmount));
            }

            _salesAmount = value;
        }
    }

    public decimal CommissionRatePercent
    {
        get => _commissionRatePercent;
        set
        {
            if (value <= 0 || value > 100)
            {
                throw new EmployeeValidationException(
                    $"Комісійний відсоток має бути в межах (0; 100]% (передано: {value}%).",
                    nameof(CommissionRatePercent));
            }

            _commissionRatePercent = value;
        }
    }

    public CommissionEmployee(
        string fullName,
        string position,
        decimal baseRetainer,
        decimal salesAmount,
        decimal commissionRatePercent)
        : this(fullName, position, experienceYears: 0, baseRetainer, salesAmount, commissionRatePercent)
    {
    }

    public CommissionEmployee(
        string fullName,
        string position,
        int experienceYears,
        decimal baseRetainer,
        decimal salesAmount,
        decimal commissionRatePercent)
        : base(fullName, position, experienceYears)
    {
        BaseRetainer = baseRetainer;
        SalesAmount = salesAmount;
        CommissionRatePercent = commissionRatePercent;
    }

    public override decimal CalculateMonthlySalary()
    {
        return BaseRetainer + SalesAmount * (CommissionRatePercent / 100m);
    }

    public override string GetSummary()
    {
        return $"[Комісійний] {base.GetSummary()} (ставка: {BaseRetainer:N2} грн, продажі: {SalesAmount:N2} грн × {CommissionRatePercent:F1}%)";
    }
}