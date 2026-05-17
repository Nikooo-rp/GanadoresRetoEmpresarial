using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    internal class MenuAdmin
    {
        public void Mostrar(Admin a, HotelData data)
        {
            bool salir = false;

            while (!salir)
            {
                Console.WriteLine("Bienvenido al menú del admin");
                Console.WriteLine("¿Qué quieres hacer");
                Console.WriteLine("[1] Modificar el costo de una habitacion." +
                    "\n [2] Gestionar promociones y tarifas especiales." +
                    "\n [3] Calcular ingresos." +
                    "\n [4] Generar reportes." +
                    "\n [0] Salir del menú.");

                int opcion = int.TryParse(Console.ReadLine(), out int op) ? op : 0;

                switch (opcion)
                {
                    case 1:
                        for (int i = 0; i < data.habitaciones.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. Habitación #{habitaciones[i].numero}");
                        }
                        int opc = int.Parse(Console.ReadLine());

                        Habitacion habitacionSeleccionada = habitaciones[opc - 1];

                        Console.WriteLine("Nuevo precio:");
                        double nuevoPrecio = double.Parse(Console.ReadLine());
                        a.ModificarCosto(habitacionSeleccionada, nuevoPrecio);
                        break;
                    case 2:
                        GestionarPromociones(promocions);
                        break;
                    case 3:
                        CalcularIngresos(facturacions);
                        break;
                    case 4:
                        GenerarReporte(facturacions);
                        break;
                    case 0:
                        salir = true;
                        break;
                }
            }
        }
    }
}
