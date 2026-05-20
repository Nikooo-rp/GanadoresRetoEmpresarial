
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class MenuRecepcionista
    {
        public static void Mostrar(Recepcionista recepcionistaActual, HotelData data)
        {
            bool sesionActiva = true;

            while (sesionActiva)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("   MÓDULO DE RECEPCIÓN    ");
                Console.WriteLine("======================================");
                Console.WriteLine($"Recepcionista en turno: {recepcionistaActual.nombre}");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("1. Registrar nuevo Cliente");
                Console.WriteLine("2. Ver disponibilidad de Habitaciones");
                Console.WriteLine("3. Crear nueva Reserva");
                Console.WriteLine("4. Agregar Servicio Adicional con Descripción");
                Console.WriteLine("5. Procesar Check-out e Imprimir Factura");
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
                        Console.Write("Correo del cliente (Servirá como identificador): ");
                        string correo = Console.ReadLine();

                        // Delegamos la creación y el guardado directamente a HotelData
                        Cliente nuevoCliente = new Cliente(correo, nombre, "password_generico");
                        data.clientes.Add(nuevoCliente);

                        Console.WriteLine("[ÉXITO] Cliente guardado en la base de datos central.");
                        break;

                    case "2":
                        Console.WriteLine("--- HABITACIONES DEL HOTEL ---");
                        foreach (Habitacion h in data.habitaciones)
                        {
                            Console.WriteLine(h.ToString());
                        }
                        break;

                    case "3":
                        Console.Write("Ingrese el correo del cliente: ");
                        string correoBusqueda = Console.ReadLine();

                        // Recepcionista busca la info en la base de datos
                        Cliente clienteReserva = recepcionistaActual.ConsultarInfoHuesped(correoBusqueda, data);

                        if (clienteReserva != null)
                        {
                            Console.Write("Ingrese el número de la habitación: ");
                            if (int.TryParse(Console.ReadLine(), out int numHabitacion))
                            {
                                // Buscamos la referencia exacta de la habitación en HotelData
                                Habitacion habSeleccionada = data.habitaciones.Find(h => h.numero == numHabitacion);

                                if (habSeleccionada != null)
                                {
                                    Console.Write("Ingrese cantidad de noches: ");
                                    if (int.TryParse(Console.ReadLine(), out int noches))
                                    {
                                        // Delegamos la lógica de negocio a la entidad Recepcionista
                                        Reserva resultado = recepcionistaActual.RegistrarReserva(clienteReserva, habSeleccionada, noches, data);

                                        if (resultado != null)
                                            Console.WriteLine($"[ÉXITO] Reserva confirmada para {clienteReserva.nombre}.");
                                        else
                                            Console.WriteLine("[ERROR] La habitación no está disponible.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("[ERROR] Número de habitación inexistente.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Cliente no encontrado. Regístrelo primero.");
                        }
                        break;

                    case "4":
                        Console.Write("Correo del cliente: ");
                        string correoServ = Console.ReadLine();
                        Cliente clienteEncontrado = recepcionistaActual.ConsultarInfoHuesped(correoServ, data);

                        if (clienteEncontrado != null)
                        {
                            Console.Write("Tipo de servicio (Ej. Restaurante, Lavandería): ");
                            string tipoSrv = Console.ReadLine();

                            // Implementación de la descripción del servicio
                            Console.Write("Descripción detallada del servicio (Ej. Desayuno Continental a la habitación): ");
                            string descSrv = Console.ReadLine();

                            Console.Write("Costo del servicio: ");
                            if (double.TryParse(Console.ReadLine(), out double precioSrv))
                            {
                                // Pasamos todos los parámetros, incluyendo la descripción y HotelData
                                recepcionistaActual.SolicitarServicioAdicional(clienteEncontrado, tipoSrv, descSrv, precioSrv, data);
                                Console.WriteLine($"[ÉXITO] Servicio añadido a la cuenta de {clienteEncontrado.nombre}.");
                            }
                            else
                            {
                                Console.WriteLine("[ERROR] Costo inválido.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Cliente no encontrado.");
                        }
                        break;

                    case "5":
                        Console.Write("Correo del cliente para Check-out: ");
                        string correoOut = Console.ReadLine();
                        Cliente clienteOut = recepcionistaActual.ConsultarInfoHuesped(correoOut, data);

                        // Verificamos que el cliente exista y tenga reservas activas en su lista
                        if (clienteOut != null && clienteOut.reservasCliente.Count > 0)
                        {
                            // Tomamos la primera reserva activa 
                            Reserva resCierre = clienteOut.reservasCliente.FirstOrDefault(r => r.GetEstadoR() == "Confirmada");

                            if (resCierre != null)
                            {
                                // Finalizamos estancia y obtenemos la factura
                                Facturacion facturaGenerada = recepcionistaActual.FinalizarEstancia(resCierre, clienteOut, data);

                                if (facturaGenerada != null)
                                {
                                    // Llamamos al método ImprimirFactura
                                    recepcionistaActual.ImprimirFactura(facturaGenerada, data);
                                }
                            }
                            else
                            {
                                Console.WriteLine("[AVISO] El cliente no tiene reservas en estado 'Confirmada' para hacer Check-out.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[AVISO] El cliente no existe o no tiene reservas.");
                        }
                        break;

                    case "6":
                        sesionActiva = false;
                        Console.WriteLine("Cerrando sesión. Saliendo del Módulo de Recepción...");
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
}﻿