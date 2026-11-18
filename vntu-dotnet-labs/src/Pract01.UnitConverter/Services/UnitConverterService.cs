using Pract01.UnitConverter.Models;

namespace Pract01.UnitConverter.Services;

/// <summary>
/// Сервіс виконання конвертацій величин та валідації фізичних обмежень.
/// </summary>
public sealed class UnitConverterService
{
    private static readonly UnitDefinition[] TemperatureUnits =
    [
        new("°C", "Градуси Цельсія", 1.0),
        new("°F", "Градуси Фаренгейта", 1.0),
        new("K", "Кельвіни", 1.0)
    ];

    // Базова одиниця довжини — метр (м)
    private static readonly UnitDefinition[] LengthUnits =
    [
        new("мм", "Міліметри", 0.001),
        new("см", "Сантиметри", 0.01),
        new("м", "Метри", 1.0),
        new("км", "Кілометри", 1000.0),
        new("дюйм", "Дюйми", 0.0254),
        new("фут", "Фути", 0.3048),
        new("миля", "Милі", 1609.344)
    ];

    // Базова одиниця маси — кілограм (кг)
    private static readonly UnitDefinition[] MassUnits =
    [
        new("мг", "Міліграми", 0.000001),
        new("г", "Грамами", 0.001),
        new("кг", "Кілограми", 1.0),
        new("т", "Тонни", 1000.0),
        new("унція", "Унції", 0.028349523125),
        new("фунт", "Фунти", 0.45359237)
    ];

    // Базова одиниця обсягу інформації — байт (Б), двійкова система (1024)
    private static readonly UnitDefinition[] InformationUnits =
    [
        new("Б", "Байти", 1.0),
        new("КБ", "Кілобайти (KiB)", 1024.0),
        new("МБ", "Мегабайти (MiB)", 1024.0 * 1024.0),
        new("ГБ", "Гігабайти (GiB)", 1024.0 * 1024.0 * 1024.0),
        new("ТБ", "Терабайти (TiB)", 1024.0 * 1024.0 * 1024.0 * 1024.0)
    ];

    public string GetCategoryDisplayName(UnitCategory category) => category switch
    {
        UnitCategory.Temperature => "Температура",
        UnitCategory.Length => "Довжина та відстань",
        UnitCategory.Mass => "Маса",
        UnitCategory.Information => "Обсяг інформації",
        _ => "Невідома категорія"
    };

    public UnitDefinition[] GetUnitsForCategory(UnitCategory category) => category switch
    {
        UnitCategory.Temperature => TemperatureUnits,
        UnitCategory.Length => LengthUnits,
        UnitCategory.Mass => MassUnits,
        UnitCategory.Information => InformationUnits,
        _ => []
    };

    /// <summary>
    /// Перевіряє коректність вхідного значення (граничні випадки) та виконує конвертацію.
    /// </summary>
    public (bool Success, double Result, string? ErrorMessage) TryConvert(
        UnitCategory category,
        double value,
        int sourceUnitIndex,
        int targetUnitIndex)
    {
        UnitDefinition[] units = GetUnitsForCategory(category);

        if (units.Length == 0)
        {
            return (false, 0, "Обрано непідтримувану категорію величин.");
        }

        if (sourceUnitIndex < 0 || sourceUnitIndex >= units.Length ||
            targetUnitIndex < 0 || targetUnitIndex >= units.Length)
        {
            return (false, 0, "Індекс одиниці вимірювання виходить за межі допустимого діапазону.");
        }

        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return (false, 0, "Значення має бути скінченним дійсним числом.");
        }

        UnitDefinition sourceUnit = units[sourceUnitIndex];
        UnitDefinition targetUnit = units[targetUnitIndex];

        // Граничний випадок №1: фізичні величини (довжина, маса, обсяг даних) не можуть бути від'ємними
        if (category != UnitCategory.Temperature && value < 0)
        {
            return (false, 0, $"Величина категорії «{GetCategoryDisplayName(category)}» не може бути від'ємною ({value}).");
        }

        // Граничний випадок №2: температура не може бути нижчою за абсолютний нуль
        if (category == UnitCategory.Temperature && IsBelowAbsoluteZero(value, sourceUnit.Code))
        {
            return (false, 0, "Температура не може бути нижчою за абсолютний нуль (-273.15 °C / -459.67 °F / 0 K).");
        }

        if (sourceUnitIndex == targetUnitIndex)
        {
            return (true, value, null);
        }

        double converted = category == UnitCategory.Temperature
            ? ConvertTemperature(value, sourceUnit.Code, targetUnit.Code)
            : ConvertLinear(value, sourceUnit.FactorToBase, targetUnit.FactorToBase);

        return (true, converted, null);
    }

    /// <summary>
    /// Пакетна конвертація масиву значень (демонстрація роботи з масивами та циклами).
    /// </summary>
    public double[] ConvertArray(
        double[] sourceValues,
        UnitCategory category,
        int sourceUnitIndex,
        int targetUnitIndex)
    {
        if (sourceValues.Length == 0)
        {
            return [];
        }

        double[] results = new double[sourceValues.Length];

        for (int i = 0; i < sourceValues.Length; i++)
        {
            var (success, result, error) = TryConvert(category, sourceValues[i], sourceUnitIndex, targetUnitIndex);
            if (!success)
            {
                throw new ArgumentException($"Помилка на елементі [{i}] ({sourceValues[i]}): {error}");
            }

            results[i] = result;
        }

        return results;
    }

    private static bool IsBelowAbsoluteZero(double value, string unitCode) => unitCode switch
    {
        "°C" => value < -273.15,
        "°F" => value < -459.67,
        "K" => value < 0.0,
        _ => false
    };

    private static double ConvertLinear(double value, double sourceFactorToBase, double targetFactorToBase)
    {
        double valueInBaseUnits = value * sourceFactorToBase;
        return valueInBaseUnits / targetFactorToBase;
    }

    private static double ConvertTemperature(double value, string fromCode, string toCode)
    {
        // Крок 1: переводимо в градуси Цельсія
        double celsius = fromCode switch
        {
            "°C" => value,
            "°F" => (value - 32.0) * 5.0 / 9.0,
            "K" => value - 273.15,
            _ => throw new ArgumentOutOfRangeException(nameof(fromCode), $"Невідома шкала: {fromCode}")
        };

        // Крок 2: переводимо з Цельсія у цільову шкалу
        return toCode switch
        {
            "°C" => celsius,
            "°F" => celsius * 9.0 / 5.0 + 32.0,
            "K" => celsius + 273.15,
            _ => throw new ArgumentOutOfRangeException(nameof(toCode), $"Невідома шкала: {toCode}")
        };
    }
}