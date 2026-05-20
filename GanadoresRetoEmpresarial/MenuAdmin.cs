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
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("   MÓDULO DE ADMINISTRACIÓN    ");
                Console.WriteLine("======================================");
                Console.WriteLine("Bienvenido, " + a.nombre);
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("[1] Modificar el costo de una habitacion" +
                    "\n[2] Gestionar promociones y tarifas especiales" +
                    "\n[3] Calcular ingresos" +
                    "\n[4] Generar reportes" +
                    "\n[0] Cerrar sesión");
                Console.WriteLine("======================================");
                int opcion = AskTypes.AskInt("Seleccione una operación: ");

                switch (opcion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("====Habitaciones a modificar====");
                        for (int i = 0; i < data.habitaciones.Count; i++)
                        {
                            Console.WriteLine($"[{i + 1}] Habitación {data.habitaciones[i].numero} - Precio actual: {data.habitaciones[i].precioNoche}");
                        }
                        Console.WriteLine("[0] Cancelar.");

                        int opc = AskTypes.AskInt("Selecciona una habitación: ");

                        if (opc == 0)
                        {
                            Console.WriteLine("Operación cancelada.");
                            Console.ReadKey();
                            break;
                        }

                        if(opc < 1 || opc > data.habitaciones.Count)
                        {
                            Console.WriteLine("Opción inválida.");
                            Console.ReadKey();
                            break;
                        }

                        Habitacion habitacionSeleccionada = data.habitaciones[opc - 1];

                        Console.Write("Nuevo precio: ");
                                
                        double nuevoPrecio;
                                
                        if (double.TryParse(Console.ReadLine(), out double precio))        
                        {                                    
                            Console.WriteLine($"El nuevo precio de la habitación {habitacionSeleccionada.numero} es {precio}.");                                   
                            Console.ReadKey();                                    
                            nuevoPrecio = precio;                                
                        }
                                
                        else                                
                        {                                    
                            Console.WriteLine("Precio inválido. Se mantendrá el precio original."); 
                            Console.ReadKey();
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
                        Console.ReadKey();
                        break;
                    case 0:
                        Console.WriteLine("Cerrando sesión. Saliendo del Módulo de Administración...");
                        Console.ReadKey();
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
