using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace ShellSort
{
    internal class Program
    {
        private const string FilePath = @"C:\Games\shellsort_results.txt";
        private static BigInteger _iterationCount;

        static void Main()
        {
            PrepareFile();
            File.AppendAllText(FilePath, "РЕЗУЛЬТАТЫ СОРТИРОВКИ ШЕЛЛА (10^1 до 10^100)\n\n");

            // Генерируем 100 размеров в геометрической прогрессии от 10^1 до 10^100
            var arraySizes = GenerateGeometricSequence(1, 9, 100).Select(x => (int)BigInteger.Min(x, int.MaxValue)).ToArray();

            foreach (var length in arraySizes)
            {
                if (length <= 0) continue; // Пропускаем некорректные размеры

                _iterationCount = 0;
                var stopwatch = new Stopwatch();

                try
                {
                    // Генерация массива
                    var rnd = new Random();
                    int[] numbers;

                    if (length <= 100_000) // Для больших массивов показываем только первые элементы
                    {
                        numbers = Enumerable.Range(0, length).Select(_ => rnd.Next(10000, 100000)).ToArray();
                        File.AppendAllText(FilePath, $"=== РАЗМЕР МАССИВА: {length} элементов ===\n");
                        File.AppendAllText(FilePath, $"Исходный массив ({DateTime.Now}):\n");
                        File.AppendAllText(FilePath, string.Join(" ", numbers) + "\n\n");
                    }
                    else
                    {
                        numbers = new int[length];
                        for (int i = 0; i < length; i++) numbers[i] = rnd.Next(10000, 100000);
                        File.AppendAllText(FilePath, $"=== РАЗМЕР МАССИВА: {length} элементов (показаны первые 100) ===\n");
                        File.AppendAllText(FilePath, $"Исходный массив ({DateTime.Now}):\n");
                        File.AppendAllText(FilePath, string.Join(" ", numbers.Take(100)) + "...\n\n");
                    }

                    // Сортировка
                    stopwatch.Start();
                    ShellSort(numbers);
                    stopwatch.Stop();

                    // Запись результатов
                    if (length <= 100_000)
                    {
                        File.AppendAllText(FilePath, $"Отсортированный массив:\n");
                        File.AppendAllText(FilePath, string.Join(" ", numbers) + "\n\n");
                    }
                    else
                    {
                        File.AppendAllText(FilePath, $"Отсортированный массив (показаны первые 100):\n");
                        File.AppendAllText(FilePath, string.Join(" ", numbers.Take(100)) + "...\n\n");
                    }

                    File.AppendAllText(FilePath,
                        $"| Размер массива | Итерации | Время (мс) |\n" +
                        $"|----------------|----------|------------|\n" +
                        $"| {length,14} | {_iterationCount,8} | {stopwatch.Elapsed.TotalMilliseconds,10:F4} |\n\n");

                    Console.WriteLine($"Обработано: {length} элементов | Итераций: {_iterationCount} | Время: {stopwatch.Elapsed.TotalMilliseconds:F2} мс");
                }
                catch (Exception ex)
                {
                    File.AppendAllText(FilePath, $"Ошибка при обработке массива из {length} элементов: {ex.Message}\n\n");
                    Console.WriteLine($"Ошибка при {length} элементах: {ex.Message}");
                }
            }

            Console.WriteLine($"\nРезультаты сохранены в: {FilePath}");
        }

        // Генерация геометрической последовательности 10^start..10^end
        static BigInteger[] GenerateGeometricSequence(int start, int end, int count)
        {
            var result = new BigInteger[count];
            double step = (end - start) / (double)(count - 1);

            for (int i = 0; i < count; i++)
            {
                double exponent = start + step * i;
                result[i] = BigInteger.Pow(10, (int)exponent);
            }

            return result;
        }

        static void PrepareFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, string.Empty);
        }

        static void ShellSort(int[] arr)
        {
            for (int gap = arr.Length / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < arr.Length; i++)
                {
                    int temp = arr[i];
                    int j;
                    for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                    {
                        arr[j] = arr[j - gap];
                        _iterationCount++;
                    }
                    arr[j] = temp;
                    _iterationCount++;
                }
            }
        }
    }
}