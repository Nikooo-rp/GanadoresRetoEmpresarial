using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    internal class SaveManager
    {
        private static readonly System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true, // Para que el JSON sea más legible
            IncludeFields = true // Para incluir campos públicos en la serialización
        };

        public static void SaveData(HotelData data, string path)
        {
            try
            {
                string jsonData = System.Text.Json.JsonSerializer.Serialize(data, options);
                File.WriteAllText(path, jsonData);
                Console.WriteLine("Datos guardados exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar los datos: {ex.Message}");
            }
        }

        public static HotelData LoadData(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string jsonData = File.ReadAllText(path);
                    return System.Text.Json.JsonSerializer.Deserialize<HotelData>(jsonData, options) ?? new HotelData();
                }
                else
                {
                    return new HotelData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los datos: {ex.Message}");
                return new HotelData();
            }
        }
    }
}