using System.Text;
using Pract01.UnitConverter.Models;
using Pract01.UnitConverter.Services;
using Pract01.UnitConverter.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

UnitConverterService converterService = new();
List<ConversionRecord> history = [];

bool isRunning = true;

while (isRunning)
{
    ConsoleHelper.PrintHeader("КОНВЕРТЕР ВЕЛИЧИН (.NET 10)");
    Console.WriteLine("1. Конвертувати величину");
    Console.WriteLine("2. Пакетна конвертація масиву значень");
    Console.WriteLine("3. Переглянути історію конвертацій за сесію");
    Console.WriteLine("4. Очистити історію конвертацій");
    Console.WriteLine("0. Вихід");
    Console.WriteLine(new string('-', 42));

    int menuChoice = ConsoleHelper.ReadIntInRange("Оберіть пункт меню: ", 0, 4);

    // Використання switch expression для маршрутизації дій меню
    Action selectedAction = menuChoice switch
    {
        1 => () => HandleSingleConversion(converterService, history),
        2 => () => HandleBatchConversion(converterService, history),
        3 => () => ShowHistory(history),
        4 => () => ClearHistory(history),
        0 => () =>
        {
            ConsoleHelper.PrintSuccess("\nРоботу програми завершено. До побачення!");
            isRunning = false;
        },
        _ => () => ConsoleHelper.PrintError("Невідомий пункт меню.")
    };

    selectedAction.Invoke();
}

static UnitCategory SelectCategory(UnitConverterService service)
{
    Console.WriteLine("\nОберіть категорію величин:");
    Console.WriteLine($"  1. {service.GetCategoryDisplayName(UnitCategory.Temperature)}");
    Console.WriteLine($"  2. {service.GetCategoryDisplayName(UnitCategory.Length)}");
    Console.WriteLine($"  3. {service.GetCategoryDisplayName(UnitCategory.Mass)}");
    Console.WriteLine($"  4. {service.GetCategoryDisplayName(UnitCategory.Information)}");

    int choice = ConsoleHelper.ReadIntInRange("Ваш вибір (1-4): ", 1, 4);
    return (UnitCategory)choice;
}

static (int SourceIndex, int TargetIndex) SelectUnits(UnitDefinition[] units)
{
    Console.WriteLine("\nДоступні одиниці вимірювання:");
    for (int i = 0; i < units.Length; i++)
    {
        Console.WriteLine($"  {i + 1}. {units[i].Name} ({units[i].Code})");
    }

    int sourceChoice = ConsoleHelper.ReadIntInRange($"З якої одиниці переводимо (1-{units.Length}): ", 1, units.Length);
    int targetChoice = ConsoleHelper.ReadIntInRange($"У яку одиницю переводимо (1-{units.Length}): ", 1, units.Length);

    return (sourceChoice - 1, targetChoice - 1);
}

static void HandleSingleConversion(UnitConverterService service, List<ConversionRecord> history)
{
    UnitCategory category = SelectCategory(service);
    UnitDefinition[] units = service.GetUnitsForCategory(category);
    var (sourceIndex, targetIndex) = SelectUnits(units);

    double inputValue = ConsoleHelper.ReadDouble($"Введіть значення ({units[sourceIndex].Code}): ");

    var (success, result, errorMessage) = service.TryConvert(category, inputValue, sourceIndex, targetIndex);

    if (!success)
    {
        ConsoleHelper.PrintError(errorMessage ?? "Не вдалося виконати конвертацію.");
        return;
    }

    string formattedResult = $"{inputValue:G10} {units[sourceIndex].Code} = {result:F4} {units[targetIndex].Code}";
    ConsoleHelper.PrintSuccess($"\nРезультат: {formattedResult}");

    history.Add(new ConversionRecord(
        Timestamp: DateTime.Now,
        CategoryName: service.GetCategoryDisplayName(category),
        SourceValue: inputValue,
        SourceUnitCode: units[sourceIndex].Code,
        TargetValue: result,
        TargetUnitCode: units[targetIndex].Code));
}

static void HandleBatchConversion(UnitConverterService service, List<ConversionRecord> history)
{
    UnitCategory category = SelectCategory(service);
    UnitDefinition[] units = service.GetUnitsForCategory(category);
    var (sourceIndex, targetIndex) = SelectUnits(units);

    double[] values = ConsoleHelper.ReadDoubleArray(
        $"Введіть масив значень у [{units[sourceIndex].Code}] через пробіл: ");

    try
    {
        double[] convertedValues = service.ConvertArray(values, category, sourceIndex, targetIndex);

        Console.WriteLine("\nРезультати пакетної конвертації:");
        Console.WriteLine(new string('-', 45));

        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"  [{i + 1}] {values[i]:G10} {units[sourceIndex].Code}  ->  {convertedValues[i]:F4} {units[targetIndex].Code}");

            history.Add(new ConversionRecord(
                Timestamp: DateTime.Now,
                CategoryName: service.GetCategoryDisplayName(category),
                SourceValue: values[i],
                SourceUnitCode: units[sourceIndex].Code,
                TargetValue: convertedValues[i],
                TargetUnitCode: units[targetIndex].Code));
        }
    }
    catch (ArgumentException ex)
    {
        ConsoleHelper.PrintError(ex.Message);
    }
}

static void ShowHistory(List<ConversionRecord> history)
{
    ConsoleHelper.PrintHeader("ІСТОРІЯ КОНВЕРТАЦІЙ");

    // Граничний випадок: порожній список історії
    if (history.Count == 0)
    {
        ConsoleHelper.PrintWarning("Історія порожня. Виконайте хоча б одну конвертацію.");
        return;
    }

    for (int i = 0; i < history.Count; i++)
    {
        ConversionRecord item = history[i];
        Console.WriteLine(
            $"{i + 1,2}. [{item.Timestamp:HH:mm:ss}] ({item.CategoryName}) " +
            $"{item.SourceValue:G10} {item.SourceUnitCode} = {item.TargetValue:F4} {item.TargetUnitCode}");
    }
}

static void ClearHistory(List<ConversionRecord> history)
{
    if (history.Count == 0)
    {
        ConsoleHelper.PrintWarning("Історія вже порожня.");
        return;
    }

    history.Clear();
    ConsoleHelper.PrintSuccess("Історію конвертацій очищено.");
}