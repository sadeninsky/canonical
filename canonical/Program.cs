using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using canonical.Exceptions;

namespace canonical
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
                InteractiveInput();
            else if (args.Length == 1)
                FileInput(args[0]);
            else
                Console.Error.WriteLine($"Usage: {Path.GetFileName(Environment.GetCommandLineArgs()[0])} [filename]");
        }

        /// <exception cref="CanonicalException"></exception>
        private static IEnumerable<string> ReadFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.Error.WriteLine($"Файл с именем {filename} не найден");
                Environment.Exit(1);
            }

            try
            {
                return File.ReadAllLines(filename);
            }
            catch (Exception ex)
            {
                throw new CanonicalException(ex);
            }
        }

        private static string ToCanonical(string input)
        {
            try
            {
                return Expression.Parse(input).Simplify().ToString();
            }
            catch (CanonicalException ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        /// <exception cref="CanonicalException"></exception>
        private static void WriteFile(string filename, IEnumerable<string> lines)
        {
            try
            {
                File.WriteAllLines(filename, lines);
            }
            catch (Exception ex)
            {
                throw new CanonicalException(ex);
            }
        }

        private static void FileInput(string filename)
        {
            try
            {
                var lines = ReadFile(filename);
                var canonicals = lines.Select(ToCanonical);
                WriteFile($"{filename}.out", canonicals);
            }
            catch (CanonicalException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(2);
            }
        }

        private static void InteractiveInput()
        {
            while (true)
            {
                Console.Write("Введите уравнение: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    try
                    {
                        var canonical = Expression.Parse(input).Simplify().ToString();
                        Console.WriteLine($"Каноническая форма: {canonical}");
                    }
                    catch (CanonicalException ex)
                    {
                        Console.Error.WriteLine($"Ошибка: {ex.Message}");
                    }
                }
            }
        }
    }
}