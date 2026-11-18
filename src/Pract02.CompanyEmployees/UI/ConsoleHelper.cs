using System.Globalization;

namespace Pract02.CompanyEmployees.UI;

/// <summary>
/// Допоміжні методи консольного вводу/виводу.
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
    /// Зчитує ціле число через int.TryParse (діапазон бізнес-правил перевіряється у властивості класу).
    /// </summary>
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input?.Trim(), out int value))
            {
                return value;
            }

            PrintError("Некоректний формат цілого числа. Спробуйте ще раз.");
        }
    }

    /// <summary>
    /// Зчитує число типу decimal через decimal.TryParse (підтримує крапку і кому).
    /// </summary>
    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                string normalized = input.Trim().Replace(',', '.');
                if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                {
                    return value;
                }
            }

            PrintError("Некоректний формат числа. Спробуйте ще раз (наприклад: 35000 або 12.5).");
        }
    }

    /// <summary>
    /// Зчитує число типу double через double.TryParse (підтримує крапку і кому).
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

            PrintError("Некоректний формат дробового числа. Спробуйте ще раз.");
        }
    }

    public static string ReadRawString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }
}