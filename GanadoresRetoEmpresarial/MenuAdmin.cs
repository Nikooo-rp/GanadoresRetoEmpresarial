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
                    "\n [5] Salir del menú.");

                int opcion = int.TryParse(Console.ReadLine(), out int op) ? op : 5;

                switch (opcion)
                {
                    case 1:
                        for (int i = 0; i < data.habitaciones.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. Habitación #{data.habitaciones[i].numero}");
                        }

                        Console.Write("Seleccione una habitación: ");

                        int opc = int.TryParse(Console.ReadLine(), out int seleccion) ? seleccion : 1;

                        Habitacion habitacionSeleccionada = data.habitaciones[opc - 1];

                        Console.WriteLine("Nuevo precio:");
                        double nuevoPrecio;
                        if (double.TryParse(Console.ReadLine(), out double precio) && precio >= habitacionSeleccionada.precioNoche)
                        {
                            nuevoPrecio = precio;
                        }
                        else
                        {
                            Console.WriteLine("Precio inválido. Se mantendrá el precio original.");
                            nuevoPrecio = habitacionSeleccionada.precioNoche;
                        }

                        a.ModificarCosto(habitacionSeleccionada, nuevoPrecio);
                        break;
                    case 2:
                        a.GestionarPromociones(data.promociones);
                        break;
                    case 3:
                        a.CalcularIngresos(data.facturas);
                        break;
                    case 4:
                        a.GenerarReporte(data.facturas);
                        break;
                    case 5:
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
        }
    }
}
