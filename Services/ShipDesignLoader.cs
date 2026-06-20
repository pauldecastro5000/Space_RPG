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
            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ShipDesign>(json);
        }

        public static bool SaveToFile(string filePath, ShipDesign design)
        {
            try
            {
                string folder = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string json = JsonConvert.SerializeObject(design, Formatting.Indented);

                File.WriteAllText(filePath, json);
            } catch (Exception  ex) 
            {
                MessageBox.Show($"[SaveToFile] Error: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}