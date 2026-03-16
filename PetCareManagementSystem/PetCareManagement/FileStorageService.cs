using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PetCareManagementSystem.Data
{
    public class FileStorageService
    {
        private void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        public void Save(string path, string data)
        {
            EnsureFileExists(path);
            File.AppendAllText(path, data + Environment.NewLine);
        }

        public List<string> Load(string path)
        {
            EnsureFileExists(path);
            return File.ReadAllLines(path).ToList();
        }

        public void Overwrite(string path, List<string> lines)
        {
            EnsureFileExists(path);
            File.WriteAllLines(path, lines);
        }

        // DELETE a line that matches a condition
        public void DeleteLine(string path, Func<string, bool> condition)
        {
            var lines = Load(path);

            var filtered = lines
                .Where(line => !condition(line))
                .ToList();

            Overwrite(path, filtered);
        }

        // UPDATE a line that matches a condition
        public void UpdateLine(string path, Func<string, bool> condition, string newLine)
        {
            var lines = Load(path);

            for (int i = 0; i < lines.Count; i++)
            {
                if (condition(lines[i]))
                {
                    lines[i] = newLine;
                    break;
                }
            }

            Overwrite(path, lines);
        }
    }
}