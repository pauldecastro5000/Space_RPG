using Newtonsoft.Json;
using Space_RPG.Models;
using System.IO;

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
    }
}