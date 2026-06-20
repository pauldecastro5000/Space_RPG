using Newtonsoft.Json;
using Space_RPG.Models;
using System;
using System.IO;
using System.Windows;

namespace Space_RPG.Services
{
    public static class ShipDesignLoader
    {
        public static ShipDesign LoadFromFile(string filePath)
        {
            string resolvedFilePath = ResolveExistingShipDesignPath(filePath);

            if (string.IsNullOrWhiteSpace(resolvedFilePath) || !File.Exists(resolvedFilePath))
                return null;

            string json = File.ReadAllText(resolvedFilePath);

            return JsonConvert.DeserializeObject<ShipDesign>(json);
        }

        public static bool SaveToFile(string filePath, ShipDesign design)
        {
            try
            {
                string resolvedFilePath = ResolveSaveShipDesignPath(filePath);
                string folder = Path.GetDirectoryName(resolvedFilePath);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string json = JsonConvert.SerializeObject(design, Formatting.Indented);

                File.WriteAllText(resolvedFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[SaveToFile] Error: {ex.Message}");
                return false;
            }

            return true;
        }

        private static string ResolveSaveShipDesignPath(string filePath)
        {
            if (File.Exists(filePath))
                return filePath;

            string existingPath = ResolveExistingShipDesignPath(filePath);

            if (!string.IsNullOrWhiteSpace(existingPath) && File.Exists(existingPath))
                return existingPath;

            return filePath;
        }

        private static string ResolveExistingShipDesignPath(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                return filePath;

            string fileName = Path.GetFileName(filePath);

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "default_ship_design.json";

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string pathFromBaseDirectory = Path.Combine(baseDirectory, "ShipDesigns", fileName);

            if (File.Exists(pathFromBaseDirectory))
                return pathFromBaseDirectory;

            DirectoryInfo directory = new DirectoryInfo(baseDirectory);

            while (directory != null)
            {
                string candidatePath = Path.Combine(directory.FullName, "ShipDesigns", fileName);

                if (File.Exists(candidatePath))
                    return candidatePath;

                directory = directory.Parent;
            }

            string pathFromCurrentDirectory = Path.Combine(Environment.CurrentDirectory, "ShipDesigns", fileName);

            if (File.Exists(pathFromCurrentDirectory))
                return pathFromCurrentDirectory;

            return filePath;
        }
    }
}
