namespace GanadoresRetoEmpresarial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = new HotelData();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Sistema de administración del hotel ===");
                Console.WriteLine("¿Quién eres?");
                Console.WriteLine("1. Cliente");
                Console.WriteLine("2. Receptionista");
                Console.WriteLine("3. Admin");
                Console.WriteLine("0. Salir");
                Console.Write("\nElección: ");

                switch (Console.ReadLine())
                {
                    case "1": ClientMenu.Show(data); break;
                    case "2": ReceptionistMenu.Show(data); break;
                    case "3": AdminMenu.Show(data); break;
                    case "0": running = false; break;
                    default:
                        Console.WriteLine("Opción inválida, presiona cualquier tecla...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine("Adiós!");
        }
    }
}
