using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PetCareManagementSystem.Data
{
    /// <summary>
    /// Provides low-level read/write operations against plain text files.
    /// All services in the application use this class.
    /// Each record is stored as a single line; fields are pipe-delimited (|).
    /// </summary>    
    public class FileStorageService
    {
        /// <summary>
        /// Creates the file at the given path if it does not already exist.
        /// </summary>        
        private void EnsureFileExists(string path)
        {
            // Create the parent directory if it doesn't exist yet
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(path))
                File.Create(path).Close();
        }

        /// <summary>
        /// Appends a single line of data to the specified file.
        /// </summary>
        public void Save(string path, string data)
        {
            EnsureFileExists(path);
            File.AppendAllText(path, data + Environment.NewLine);
        }

        /// <summary>
        /// Reads all lines from the specified file and returns them as a list.
        /// Returns an empty list if the file is empty.
        /// </summary>
        public List<string> Load(string path)
        {
            EnsureFileExists(path);
            return File.ReadAllLines(path).ToList();
        }

        /// <summary>
        /// Replaces the entire contents of a file with the provided lines.
        /// </summary>
        public void Overwrite(string path, List<string> lines)
        {
            EnsureFileExists(path);
            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// Removes all lines from the file that satisfy the given condition.
        /// </summary>
        public void DeleteLine(string path, Func<string, bool> condition)
        {
            var lines = Load(path);
            var filtered = lines.Where(line => !condition(line)).ToList();
            Overwrite(path, filtered);
        }

        /// <summary>
        /// Finds the first line matching the condition and replaces it with a new value.
        /// Only the first match is updated; subsequent matches are left unchanged.
        /// </summary>
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