namespace Pract01.UnitConverter.Models;

/// <summary>
/// Категорії фізичних та інформаційних величин, що підтримуються конвертером.
/// </summary>
public enum UnitCategory
{
    Temperature = 1,
    Length = 2,
    Mass = 3,
    Information = 4
}

/// <summary>
/// Опис одиниці вимірювання та її коефіцієнта переведення до базової одиниці категорії.
/// </summary>
public readonly record struct UnitDefinition(string Code, string Name, double FactorToBase);