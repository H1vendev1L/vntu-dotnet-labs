using System.Text;
using Pract02.CompanyEmployees.Exceptions;
using Pract02.CompanyEmployees.Models;
using Pract02.CompanyEmployees.Services;
using Pract02.CompanyEmployees.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

CompanyPayrollService payrollService = new();
bool isRunning = true;

while (isRunning)
{
    ConsoleHelper.PrintHeader("ОБЛІК СПІВРОБІТНИКІВ КОМПАНІЇ (.NET 10)");
    Console.WriteLine("1. Вивести список усіх співробітників (поліморфний foreach)");
    Console.WriteLine("2. Додати нового співробітника до штату");
    Console.WriteLine("3. Відсортувати співробітників за зарплатою (IComparable<Employee>)");
    Console.WriteLine("4. Розрахувати загальний фонд оплати праці");
    Console.WriteLine("5. Демонстрація обробки виключень (тест валідації властивостей)");
    Console.WriteLine("0. Вихід");
    Console.WriteLine(new string('-', 62));

    int choice = ConsoleHelper.ReadIntInRange("Оберіть пункт меню: ", 0, 5);

    Action action = choice switch
    {
        1 => () => DisplayEmployees(payrollService.GetAll(), "СПИСОК СПІВРОБІТНИКІВ КОМПАНІЇ"),
        2 => () => AddNewEmployeeInteractive(payrollService),
        3 => () => DisplaySortedEmployees(payrollService),
        4 => () => DisplayPayrollSummary(payrollService),
        5 => RunExceptionHandlingDemo,
        0 => () =>
        {
            ConsoleHelper.PrintSuccess("\nРоботу програми завершено. До побачення!");
            isRunning = false;
        },
        _ => () => ConsoleHelper.PrintError("Невідомий пункт меню.")
    };

    action.Invoke();
}

static void DisplayEmployees(IReadOnlyList<Employee> employees, string title)
{
    ConsoleHelper.PrintHeader(title);

    if (employees.Count == 0)
    {
        ConsoleHelper.PrintWarning("Список співробітників порожній.");
        return;
    }

    int index = 1;
    // Демонстрація поліморфізму: викликається перевизначений GetSummary() конкретного класу
    foreach (Employee employee in employees)
    {
        Console.WriteLine($"{index,2}. {employee.GetSummary()}");
        index++;
    }
}

static void AddNewEmployeeInteractive(CompanyPayrollService service)
{
    Console.WriteLine("\nОберіть тип співробітника:");
    Console.WriteLine("  1. Штатний співробітник (оклад + премія)");
    Console.WriteLine("  2. Погодинний співробітник (ставка за год + години)");
    Console.WriteLine("  3. Комісійний співробітник (ставка + % від продажів)");

    int typeChoice = ConsoleHelper.ReadIntInRange("Тип (1-3): ", 1, 3);

    string fullName = ConsoleHelper.ReadRawString("Введіть ПІБ співробітника: ");
    string position = ConsoleHelper.ReadRawString("Введіть посаду: ");
    int experience = ConsoleHelper.ReadInt("Введіть стаж роботи (років): ");

    bool addedSuccessfully = false;

    try
    {
        Employee newEmployee = typeChoice switch
        {
            1 => CreateSalariedEmployee(fullName, position, experience),
            2 => CreateHourlyEmployee(fullName, position, experience),
            3 => CreateCommissionEmployee(fullName, position, experience),
            _ => throw new InvalidOperationException("Непідтримуваний тип співробітника.")
        };

        service.AddEmployee(newEmployee);
        addedSuccessfully = true;
        ConsoleHelper.PrintSuccess($"\nСпівробітника успішно додано:\n  {newEmployee.GetSummary()}");
    }
    catch (EmployeeValidationException ex)
    {
        string fieldInfo = string.IsNullOrEmpty(ex.PropertyName) ? string.Empty : $" (поле: {ex.PropertyName})";
        ConsoleHelper.PrintError($"Помилка валідації даних{fieldInfo}: {ex.Message}");
    }
    catch (Exception ex)
    {
        ConsoleHelper.PrintError($"Непередбачена помилка при створенні запису: {ex.Message}");
    }
    finally
    {
        if (!addedSuccessfully)
        {
            ConsoleHelper.PrintWarning("Операцію додавання скасовано. Колекцію залишено без змін.");
        }
    }
}

static SalariedEmployee CreateSalariedEmployee(string fullName, string position, int experience)
{
    decimal baseSalary = ConsoleHelper.ReadDecimal("Введіть базовий місячний оклад (грн): ");
    decimal bonusPercent = ConsoleHelper.ReadDecimal("Введіть відсоток премії (%): ");
    return new SalariedEmployee(fullName, position, experience, baseSalary, bonusPercent);
}

static HourlyEmployee CreateHourlyEmployee(string fullName, string position, int experience)
{
    decimal hourlyRate = ConsoleHelper.ReadDecimal("Введіть погодинну ставку (грн/год): ");
    double hoursWorked = ConsoleHelper.ReadDouble("Введіть кількість відпрацьованих годин: ");
    return new HourlyEmployee(fullName, position, experience, hourlyRate, hoursWorked);
}

static CommissionEmployee CreateCommissionEmployee(string fullName, string position, int experience)
{
    decimal retainer = ConsoleHelper.ReadDecimal("Введіть фіксовану ставку (грн): ");
    decimal sales = ConsoleHelper.ReadDecimal("Введіть суму продажів за місяць (грн): ");
    decimal commissionRate = ConsoleHelper.ReadDecimal("Введіть комісійний відсоток (%): ");
    return new CommissionEmployee(fullName, position, experience, retainer, sales, commissionRate);
}

static void DisplaySortedEmployees(CompanyPayrollService service)
{
    Console.WriteLine("\nОберіть напрямок сортування за зарплатою:");
    Console.WriteLine("  1. За зростанням (від найменшої до найбільшої)");
    Console.WriteLine("  2. За спаданням (від найбільшої до найменшої)");

    int direction = ConsoleHelper.ReadIntInRange("Ваш вибір (1-2): ", 1, 2);
    List<Employee> sorted = service.GetSortedBySalary(descending: direction == 2);

    DisplayEmployees(sorted, "ВІДСОРТОВАНИЙ СПИСОК СПІВРОБІТНИКІВ");
}

static void DisplayPayrollSummary(CompanyPayrollService service)
{
    ConsoleHelper.PrintHeader("ФОНД ОПЛАТИ ПРАЦІ КОМПАНІЇ");
    IReadOnlyList<Employee> employees = service.GetAll();

    if (employees.Count == 0)
    {
        ConsoleHelper.PrintWarning("У компанії немає зареєстрованих співробітників.");
        return;
    }

    decimal totalPayroll = service.CalculateTotalPayroll();
    decimal averageSalary = totalPayroll / employees.Count;

    Console.WriteLine($"Кількість співробітників : {employees.Count}");
    Console.WriteLine($"Загальний фонд виплат    : {totalPayroll:N2} грн");
    Console.WriteLine($"Середня заробітна плата  : {averageSalary:N2} грн");
}

static void RunExceptionHandlingDemo()
{
    ConsoleHelper.PrintHeader("ДЕМОНСТРАЦІЯ ВЛАСНОГО ВИНЯТКУ (EmployeeValidationException)");

    Action[] invalidCases =
    [
        () => _ = new SalariedEmployee("", "Developer", 2, 30000m, 10m),
        () => _ = new SalariedEmployee("Іваненко Іван", "Tester", 1, -15000m, 5m),
        () => _ = new HourlyEmployee("Петренко Петро", "Designer", 3, 400m, 900.0),
        () => _ = new CommissionEmployee("Сидоренко Анна", "Manager", 2, 15000m, 100000m, 150m)
    ];

    for (int i = 0; i < invalidCases.Length; i++)
    {
        try
        {
            Console.WriteLine($"\nТест #{i + 1}: спроба створити об'єкт із некоректними даними...");
            invalidCases[i].Invoke();
        }
        catch (EmployeeValidationException ex)
        {
            ConsoleHelper.PrintError($"Перехоплено EmployeeValidationException [Поле: {ex.PropertyName}]: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"Блок finally для тесту #{i + 1} виконано.");
        }
    }
}