
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class MenuRecepcionista
    {
        public void Mostrar(Recepcionista recepcionistaActual, HotelData data)
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
                Console.WriteLine("[1] Registrar nuevo cliente");
                Console.WriteLine("[2] Ver disponibilidad de habitaciones");
                Console.WriteLine("[3] Crear nueva reserva");
                Console.WriteLine("[4] Agregar servicios adicionales");
                Console.WriteLine("[5] Procesar check-out e imprimir factura");
                Console.WriteLine("[0] Cerrar sesión");
                Console.WriteLine("======================================");
                string opcion = AskTypes.AskString("Seleccione una operación: ");

                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        string nombre = AskTypes.AskString("Nombre del cliente: ");
                        
                        string correo = AskTypes.AskString("Correo del cliente (Servirá como identificador): ");

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
                        string correoBusqueda = AskTypes.AskString("Ingrese el correo del cliente: ");                      

                        // Recepcionista busca la info en la base de datos
                        Cliente clienteReserva = recepcionistaActual.ConsultarInfoHuesped(correoBusqueda, data);

                        if (clienteReserva != null)
                        {
                            Console.WriteLine("--- HABITACIONES DEL HOTEL DISPONIBLES ---");
                            foreach (Habitacion h in data.habitaciones)
                            {
                                if(h.EstaDisponible())
                                Console.WriteLine(h.ToString());
                            }
                            Console.Write("Ingrese el número de la habitación: ");
                            if (int.TryParse(Console.ReadLine(), out int numHabitacion))
                            {
                                // Buscamos la referencia exacta de la habitación en HotelData
                                Habitacion? habSeleccionada = data.habitaciones.Find(h => h.numero == numHabitacion);

                                if (habSeleccionada != null && habSeleccionada.EstaDisponible())
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
                                    Console.WriteLine("[ERROR] Número de habitación inexistente u ocupada.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Cliente no encontrado. Regístrelo primero.");
                        }
                        break;

                    case "4":
                        string correoServ = AskTypes.AskString("Correo del cliente: ");
                        
                        Cliente clienteEncontrado = recepcionistaActual.ConsultarInfoHuesped(correoServ, data);

                        if (clienteEncontrado != null)
                        {
                            string tipoSrv = AskTypes.AskString("Tipo de servicio (Ej. Restaurante, Lavandería): ");
                            

                            // Implementación de la descripción del servicio
                            string descSrv = AskTypes.AskString("Descripción detallada del servicio (Ej. Desayuno Continental a la habitación): ");
                            

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
                                return;
                            }
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Cliente no encontrado.");
                        }
                        break;

                    case "5":
                        string correoOut = AskTypes.AskString("Correo del cliente para Check-out: ");
                        Cliente clienteOut = recepcionistaActual.ConsultarInfoHuesped(correoOut, data);

                        // Verificamos que el cliente exista y tenga reservas activas en su lista
                        if (clienteOut != null && clienteOut.reservasCliente.Count > 0)
                        {
                            // Tomamos la primera reserva activa 
                            Reserva? resCierre = clienteOut.reservasCliente.FirstOrDefault(r => r.GetEstadoR() == "Confirmada");

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

                    case "0":
                        sesionActiva = false;
                        Console.WriteLine("Cerrando sesión. Saliendo del Módulo de Recepción...");
                        Console.ReadKey();
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