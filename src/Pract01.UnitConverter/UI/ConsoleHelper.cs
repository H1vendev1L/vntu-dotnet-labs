using System.Globalization;

namespace Pract01.UnitConverter.UI;

/// <summary>
/// Допоміжні методи консольного вводу/виводу та валідації.
/// </summary>
public static class ConsoleHelper
{
    public static void PrintHeader(string title)
    {
        string border = new('=', title.Length + 8);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(border);
        Console.WriteLine($"    {title}");
        Console.WriteLine(border);
        Console.ResetColor();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Помилка] {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Зчитує ціле число в заданому діапазоні [min; max] через int.TryParse у циклі.
    /// </summary>
    public static int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input?.Trim(), out int value) && value >= min && value <= max)
            {
                return value;
            }

            PrintError($"Введіть ціле число від {min} до {max}.");
        }
    }

    /// <summary>
    /// Зчитує дробове число через double.TryParse (підтримує крапку та кому).
    /// </summary>
    public static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                string normalized = input.Trim().Replace(',', '.');
                if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                    && !double.IsNaN(value)
                    && !double.IsInfinity(value))
                {
                    return value;
                }
            }

            PrintError("Некоректне число. Спробуйте ще раз (наприклад: 25.5 або -10).");
        }
    }

    /// <summary>
    /// Зчитує масив дробових чисел через пробіл із перевіркою кожного елемента.
    /// </summary>
    public static double[] ReadDoubleArray(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                PrintError("Рядок не може бути порожнім. Введіть хоча б одне число.");
                continue;
            }

            string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            double[] values = new double[tokens.Length];
            bool allValid = true;

            for (int i = 0; i < tokens.Length; i++)
            {
                string normalized = tokens[i].Trim().Replace(',', '.');
                if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)
                    || double.IsNaN(parsed)
                    || double.IsInfinity(parsed))
                {
                    PrintError($"Елемент «{tokens[i]}» не є коректним числом.");
                    allValid = false;
                    break;
                }

                values[i] = parsed;
            }

            if (allValid)
            {
                return values;
            }
        }
    }
}