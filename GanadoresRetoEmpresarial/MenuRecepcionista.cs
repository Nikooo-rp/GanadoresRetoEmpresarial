<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class MenuRecepcionista
    {
        public static void MostrarMenu(Recepcionista admin, List<Habitacion> habitacionesHotel, List<Cliente> baseDatosClientes, List<ServicioAdicional> serviciosGlobales)
        {
            bool sesionActiva = true;
                        
            admin.ClientesActivos = baseDatosClientes;

            while (sesionActiva)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("   MÓDULO DE RECEPCIÓN");
                Console.WriteLine("======================================");
                Console.WriteLine($"Recepcionista: {admin.nombre} | ID: {admin.idRecepcionista}");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("1. Registrar nuevo Cliente");
                Console.WriteLine("2. Ver disponibilidad de Habitaciones");
                Console.WriteLine("3. Crear nueva Reserva");
                Console.WriteLine("4. Agregar Servicio Adicional (Ej. Desayuno)");
                Console.WriteLine("5. Procesar Check-out (Finalizar Estancia)");
                Console.WriteLine("6. Regresar al Menú Principal");
                Console.WriteLine("======================================");
                Console.Write("Seleccione una operación: ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Nombre del cliente: ");
                        string nombre = Console.ReadLine();
                        Console.Write("Correo del cliente (Servirá como ID): ");
                        string correo = Console.ReadLine();

                        Cliente nuevoCliente = new Cliente(nombre, "1234", correo);
                        admin.ClientesActivos.Add(nuevoCliente);
                        Console.WriteLine("[ÉXITO] Cliente guardado en la base de datos.");
                        break;

                    case "2":
                        Console.WriteLine("--- HABITACIONES DEL HOTEL ---");
                        foreach (Habitacion h in habitacionesHotel)
                        {
                            Console.WriteLine(h.ToString());
                        }
                        break;

                    case "3":
                        Console.Write("Ingrese el correo del cliente: ");
                        string correoBusqueda = Console.ReadLine();
                        Cliente clienteReserva = admin.ConsultarInfoHuesped(correoBusqueda);

                        if (clienteReserva != null)
                        {
                            Console.Write("Ingrese el número de la habitación: ");
                            if (int.TryParse(Console.ReadLine(), out int numHabitacion))
                            {
                                Habitacion habSeleccionada = habitacionesHotel.Find(h => h.numero == numHabitacion);

                                Console.Write("Ingrese cantidad de noches: ");
                                if (int.TryParse(Console.ReadLine(), out int noches))
                                {
                                    admin.RegistrarReserva(clienteReserva, habSeleccionada, noches);
                                }
                            }
                        }
                        break;

                    case "4":
                        Console.Write("Correo del cliente: ");
                        string correoServ = Console.ReadLine();
                        Cliente clienteEncontrado = admin.ConsultarInfoHuesped(correoServ);

                        if (clienteEncontrado != null)
                        {
                            Console.Write("Descripción del servicio: ");
                            string desc = Console.ReadLine();
                            Console.Write("Costo del servicio: ");
                            if (double.TryParse(Console.ReadLine(), out double precioSrv))
                            {
                                ServicioAdicional sa = admin.SolicitarServicioAdicional(clienteEncontrado, desc, precioSrv);
                                if (sa != null) serviciosGlobales.Add(sa);
                            }
                        }
                        break;

                    case "5":
                        Console.Write("Correo del cliente para Check-out: ");
                        string correoOut = Console.ReadLine();
                        Cliente clienteOut = admin.ConsultarInfoHuesped(correoOut);

                        if (clienteOut != null && clienteOut.Reservas.Count > 0)
                        {
                            Reserva resCierre = clienteOut.Reservas[0];
                            Habitacion habCierre = habitacionesHotel.Find(h => h.estadoH == EstadoHabitacion.Ocupada);

                            if (habCierre != null)
                            {
                                admin.FinalizarEstancia(resCierre, habCierre, clienteOut, serviciosGlobales);
                                serviciosGlobales.Clear();
                            }
                            else
                            {
                                Console.WriteLine("[ERROR] No se pudo resolver la habitación asignada en memoria.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[AVISO] El cliente no existe o no tiene reservas activas.");
                        }
                        break;

                    case "6":
                        sesionActiva = false;
                        Console.WriteLine("Saliendo del Módulo de Recepción...");
                        break;

                    default:
                        Console.WriteLine("[ERROR] Instrucción no válida.");
                        break;
                }

                if (sesionActiva)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class MenuRecepcionista
    {
        public static void MostrarMenu(Recepcionista admin, List<Habitacion> habitacionesHotel, List<Cliente> baseDatosClientes, List<ServicioAdicional> serviciosGlobales)
        {
            bool sesionActiva = true;
                        
            admin.ClientesActivos = baseDatosClientes;

            while (sesionActiva)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("   MÓDULO DE RECEPCIÓN");
                Console.WriteLine("======================================");
                Console.WriteLine($"Recepcionista: {admin.nombre} | ID: {admin.idRecepcionista}");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("1. Registrar nuevo Cliente");
                Console.WriteLine("2. Ver disponibilidad de Habitaciones");
                Console.WriteLine("3. Crear nueva Reserva");
                Console.WriteLine("4. Agregar Servicio Adicional (Ej. Desayuno)");
                Console.WriteLine("5. Procesar Check-out (Finalizar Estancia)");
                Console.WriteLine("6. Regresar al Menú Principal");
                Console.WriteLine("======================================");
                Console.Write("Seleccione una operación: ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Nombre del cliente: ");
                        string nombre = Console.ReadLine();
                        Console.Write("Correo del cliente (Servirá como ID): ");
                        string correo = Console.ReadLine();

                        Cliente nuevoCliente = new Cliente(nombre, "1234", correo);
                        admin.ClientesActivos.Add(nuevoCliente);
                        Console.WriteLine("[ÉXITO] Cliente guardado en la base de datos.");
                        break;

                    case "2":
                        Console.WriteLine("--- HABITACIONES DEL HOTEL ---");
                        foreach (Habitacion h in habitacionesHotel)
                        {
                            Console.WriteLine(h.ToString());
                        }
                        break;

                    case "3":
                        Console.Write("Ingrese el correo del cliente: ");
                        string correoBusqueda = Console.ReadLine();
                        Cliente clienteReserva = admin.ConsultarInfoHuesped(correoBusqueda);

                        if (clienteReserva != null)
                        {
                            Console.Write("Ingrese el número de la habitación: ");
                            if (int.TryParse(Console.ReadLine(), out int numHabitacion))
                            {
                                Habitacion habSeleccionada = habitacionesHotel.Find(h => h.numero == numHabitacion);

                                Console.Write("Ingrese cantidad de noches: ");
                                if (int.TryParse(Console.ReadLine(), out int noches))
                                {
                                    admin.RegistrarReserva(clienteReserva, habSeleccionada, noches);
                                }
                            }
                        }
                        break;

                    case "4":
                        Console.Write("Correo del cliente: ");
                        string correoServ = Console.ReadLine();
                        Cliente clienteEncontrado = admin.ConsultarInfoHuesped(correoServ);

                        if (clienteEncontrado != null)
                        {
                            Console.Write("Descripción del servicio: ");
                            string desc = Console.ReadLine();
                            Console.Write("Costo del servicio: ");
                            if (double.TryParse(Console.ReadLine(), out double precioSrv))
                            {
                                ServicioAdicional sa = admin.SolicitarServicioAdicional(clienteEncontrado, desc, precioSrv);
                                if (sa != null) serviciosGlobales.Add(sa);
                            }
                        }
                        break;

                    case "5":
                        Console.Write("Correo del cliente para Check-out: ");
                        string correoOut = Console.ReadLine();
                        Cliente clienteOut = admin.ConsultarInfoHuesped(correoOut);

                        if (clienteOut != null && clienteOut.Reservas.Count > 0)
                        {
                            Reserva resCierre = clienteOut.Reservas[0];
                            Habitacion habCierre = habitacionesHotel.Find(h => h.estadoH == EstadoHabitacion.Ocupada);

                            if (habCierre != null)
                            {
                                admin.FinalizarEstancia(resCierre, habCierre, clienteOut, serviciosGlobales);
                                serviciosGlobales.Clear();
                            }
                            else
                            {
                                Console.WriteLine("[ERROR] No se pudo resolver la habitación asignada en memoria.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[AVISO] El cliente no existe o no tiene reservas activas.");
                        }
                        break;

                    case "6":
                        sesionActiva = false;
                        Console.WriteLine("Saliendo del Módulo de Recepción...");
                        break;

                    default:
                        Console.WriteLine("[ERROR] Instrucción no válida.");
                        break;
                }

                if (sesionActiva)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }
    }
}
>>>>>>> 66b8016b02cda43c768e1407c0a564655cf72632
