using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace auto_designs
{
    internal static class FilesManager
    {
        private static readonly string keywordsPath = "keywords.json";
        private static readonly string dbPath = "tags.db";
        private static readonly string newDesignPath = "D:\\printOnDemand\\new designs.txt";
        private static readonly string newDesignfolderPath = "D:\\printOnDemand\\designs\\to upload";
        private static Dictionary<string, List<string>> keywordsData = new Dictionary<string, List<string>>();

        /// <summary>
        /// this function creates the keywords JSON file if it does not already exist
        /// </summary>
        public static void createKeywordsDataFile()
        {
            if (!File.Exists(keywordsPath))
            {
                // Create a new file with an empty JSON object
                File.WriteAllText(keywordsPath, "{}");
            }
        }

        /// <summary>
        /// this function creates the SQLite database file if it does not already exist
        /// </summary>
        public static void createDBFile()
        {
            if (!File.Exists(dbPath))
            {
                // Create a new empty database file
                File.Create(dbPath).Close();
            }
        }

        /// <summary>
        /// this function load the keywords data from the JSON file into the keywordsData dictionary
        /// </summary>
        public static void deserializeKeywordsData()
        {
            string json = File.ReadAllText(keywordsPath);
            if (!string.IsNullOrWhiteSpace(json))
            {
                keywordsData = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                               ?? new Dictionary<string, List<string>>();
            }
            else
            {
                keywordsData = new Dictionary<string, List<string>>();
            }
        }

        /// <summary>
        /// this function saves the keywordsData dictionary back to the JSON file
        /// </summary>
        public static void serializeKeywordsData()
        {
            string newJson = JsonSerializer.Serialize(keywordsData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(keywordsPath, newJson);

            Console.WriteLine("Updated JSON saved:\n" + newJson);
        }

        /// <summary>
        /// this function prints the contents of the keywordsData dictionary to the console
        /// </summary>
        public static void printkeywordsData()
        {
            foreach (var entry in keywordsData)
            {
                Console.WriteLine($"Niche: {entry.Key}");
                Console.WriteLine("Tags: " + string.Join(", ", entry.Value));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// this function adds a new niche with its associated tags to the keywordsData dictionary
        /// </summary>
        /// <param name="niche">the niche</param>
        /// <param name="tags">the tags</param>
        public static void addNiche(string niche, List<string> tags)
        {
            if (!keywordsData.ContainsKey(niche))
            {
                keywordsData[niche] = tags;
            }
            else
            {
                Console.WriteLine($"Niche '{niche}' already exists.");
            }
        }

        /// <summary>
        /// this function removes a niche from the keywordsData dictionary
        /// </summary>
        /// <param name="niche">the niche</param>
        public static void removeNiche(string niche)
        {
            if (keywordsData.ContainsKey(niche))
            {
                keywordsData.Remove(niche);
            }
            else
            {
                Console.WriteLine($"Niche '{niche}' does not exist.");
            }
        }

        /// <summary>
        /// this function returns the keywordsData dictionary
        /// </summary>
        /// <returns>the keywordsData dictionary</returns>
        public static Dictionary<string, List<string>> GetKeywordsData()
        {
            return keywordsData;
        }

        /// <summary>
        /// this function creates the directory for a given path if it does not already exist
        /// </summary>
        /// <param name="path">the path</param>
        private static void createFolderIfNotExists(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// this function creates a file at a given path if it does not already exist
        /// </summary>
        /// <param name="path">the path</param>
        private static void createFileIfNotExists(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        public static void createNewDesignFiles()
        {
            List<string> newDesigns = readLinesFromFile(newDesignPath);

            foreach (string design in newDesigns)
            {
                string name = design.Split(',')[0].Trim();
                string niche = design.Split(',')[1].Trim();

                createFolderIfNotExists(newDesignfolderPath + "\\" + name);
                createFileIfNotExists(newDesignfolderPath + "\\" + name + "\\tags.txt");
                createFileIfNotExists(newDesignfolderPath + "\\" + name + "\\title.txt");
            }
        }

        public static List<string> readLinesFromFile(string filePath)
        {
            List<string> lines = new List<string>();

            if (File.Exists(filePath))
            {
                lines = File.ReadAllLines(filePath).ToList();
                File.WriteAllText(filePath, string.Empty);
            }
            else
            {
                Console.WriteLine($"File '{filePath}' does not exist.");
                return new List<string>();
            }

            return lines;
        }
    }
}
