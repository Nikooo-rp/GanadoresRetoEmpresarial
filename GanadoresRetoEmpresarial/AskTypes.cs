using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class AskTypes
    {
        // Estos métodos se encargan de pedir al usuario un valor específico (fecha, número o texto) y validarlo. Si el usuario ingresa un valor no válido, se muestra un mensaje de error y se vuelve a pedir el valor hasta que sea correcto.
        public static DateTime AskDate(string prompt)
        {
            DateTime value;
            do
            {
                Console.Write(prompt);
                if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                    null, System.Globalization.DateTimeStyles.None, out value))
                    Console.WriteLine("  ⚠ Usa el formato yyyy-MM-dd.");
            } while (value == default);
            return value;
        }
        public static int AskInt(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.WriteLine("Debe ser un número.");
                Console.Write(prompt);
            }
            return value;
        }
        public static string AskString(string prompt)
        {
            string? value;
            do
            {
                Console.Write(prompt);
                value = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(value))
                    Console.WriteLine("No puede estar vacío bb, vuelve a intentar.");
            } while (string.IsNullOrEmpty(value));
            return value;
        }
    }
}
