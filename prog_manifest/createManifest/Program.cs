using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.Write("Введите путь к директории: ");
        string directoryPath = Console.ReadLine();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Указанная директория не существует!");
            return;
        }

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string manifestName = $"manifest_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string manifestPath = Path.Combine(desktopPath, manifestName);

        try
        {
            using (StreamWriter writer = new StreamWriter(manifestPath, false, Encoding.UTF8))
            {
                ProcessDirectory(directoryPath, directoryPath, writer);
            }
            Console.WriteLine($"Манифест создан: {manifestPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static void ProcessDirectory(string rootDir, string currentDir, StreamWriter writer)
    {
        foreach (string file in Directory.GetFiles(currentDir))
        {
            string relativePath = GetRelativePath(rootDir, file).Replace('\\', '/');
            string hash = ComputeFileHash(file);
            writer.WriteLine($"{hash} {relativePath}");
        }

        foreach (string subDir in Directory.GetDirectories(currentDir))
        {
            ProcessDirectory(rootDir, subDir, writer);
        }
    }

    static string GetRelativePath(string rootDir, string fullPath)
    {
        return Path.GetRelativePath(rootDir, fullPath);
    }

    static string ComputeFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hashBytes = sha256.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}