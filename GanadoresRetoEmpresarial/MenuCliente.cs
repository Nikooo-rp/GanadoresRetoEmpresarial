using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class MenuCliente
    {
        /*private List<Habitacion> habitacionesDisponibles;*/
        private List<Reserva> reservasCliente;
        HotelData? data = null;
        Cliente cliente = null;

        public MenuCliente()
        {
            this.reservasCliente = new List<Reserva>();
        }

        public void MostrarMenu(Cliente cliente, HotelData data)
        {
            this.data = data;
            this.cliente = cliente;

            Console.Clear();
            Console.WriteLine("=== MENÚ DEL CLIENTE ===");
            Console.WriteLine($"Bienvenido: {cliente.nombre}");
            Console.WriteLine($"Correo: {cliente.correoCliente}");
            Console.WriteLine("\n--- OPCIONES ---");
            Console.WriteLine("1. Crear nueva reserva");
            Console.WriteLine("2. Consultar disponibilidad de habitaciones");
            Console.WriteLine("3. Ver todas mis reservas");
            Console.WriteLine("4. Cancelar una reserva");
            Console.WriteLine("5. Ver detalle de una reserva específica");
            Console.WriteLine("6. Modificar fechas de una reserva");
            Console.WriteLine("7. Consultar precio de una habitación");
            Console.WriteLine("0. Salir");
            Console.Write("\nSeleccione una opción: ");
            int opcion=Convert.ToInt32(Console.ReadLine());
                switch (opcion)
                {
                    case 1:
                        CrearReserva();
                        break;
                    case 2:
                        ConsultarDisponibilidad();
                        break;
                    case 3:
                        VerTodasReservas();
                        break;
                    case 4:
                        CancelarReserva();
                        break;
                    case 5:
                        VerDetalleReserva();
                        break;
                    case 6:
                        ModificarFechasReserva();
                        break;
                    case 7:
                        ConsultarPrecioHabitacion();
                        break;
                    case 0:
                        Console.WriteLine("\n¡Gracias por usar nuestro servicio!");
                        break;
                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }

                if (opcion != 0)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
                
            
        }

        private void CrearReserva()
        {
            Console.Clear();
            Console.WriteLine("=== CREAR NUEVA RESERVA ===\n");

            // Mostrar habitaciones disponibles
            Console.WriteLine("Habitaciones disponibles:");
            Console.WriteLine("N°  | Número | Tipo           | Precio/noche | Estado");
            Console.WriteLine("--------------------------------------------------------");

            List<Habitacion> habitacionesDisponiblesActual = data.habitaciones
                .Where(h => h.EstaDisponible())
                .ToList();

            if (habitacionesDisponiblesActual.Count == 0)
            {
                Console.WriteLine("No hay habitaciones disponibles en este momento.");
                return;
            }

            for (int i = 0; i < habitacionesDisponiblesActual.Count; i++)
            {
                Console.WriteLine($"{i + 1,-3} | {habitacionesDisponiblesActual[i].numero,-6} | {habitacionesDisponiblesActual[i].tipo,-14} | ${habitacionesDisponiblesActual[i].precioNoche,-11} | {habitacionesDisponiblesActual[i].estadoH}");
            }

            // Seleccionar habitación
            Console.Write("\nSeleccione el número de la habitación que desea reservar: ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= habitacionesDisponiblesActual.Count)
            {
                Habitacion habitacionSeleccionada = habitacionesDisponiblesActual[seleccion - 1];

                // Fecha de entrada
                Console.Write("Ingrese la fecha de entrada (dd/mm/yyyy): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaEntrada))
                {
                    // Fecha de salida
                    Console.Write("Ingrese la fecha de salida (dd/mm/yyyy): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaSalida))
                    {
                        if (fechaSalida > fechaEntrada)
                        {
                            // Verificar disponibilidad de fechas
                            if (VerificarDisponibilidadFechas(habitacionSeleccionada, fechaEntrada, fechaSalida))
                            {
                                // Crear la reserva usando el constructor con fechas
                                Reserva nuevaReserva = cliente.CrearReserva(fechaEntrada, fechaSalida, habitacionSeleccionada);
                                nuevaReserva.ModificarEstado("Confirmada");

                                // Guardar referencia a la habitación (necesario para el menú)
                                // Podrías agregar un campo habitacion en tu clase Reserva o usar un diccionario
                                // Por ahora, asumimos que agregaste ese campo
                                nuevaReserva.habitacion = habitacionSeleccionada;

                                reservasCliente.Add(nuevaReserva);

                                // Cambiar el estado de la habitación a Ocupada
                                habitacionSeleccionada.GetEstado(EstadoHabitacion.Ocupada);

                                Console.WriteLine("\n¡Reserva creada exitosamente!");
                                nuevaReserva.MostrarInformacion();
                            }
                            else
                            {
                                Console.WriteLine("Lo siento, la habitación no está disponible para esas fechas.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("La fecha de salida debe ser posterior a la fecha de entrada.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Fecha de salida inválida.");
                    }
                }
                else
                {
                    Console.WriteLine("Fecha inválida.");
                }
            }
            else
            {
                Console.WriteLine("Selección inválida.");
            }
        }

        private void ConsultarDisponibilidad()
        {
            Console.Clear();
            Console.WriteLine("=== DISPONIBILIDAD DE HABITACIONES ===\n");

            Console.WriteLine("Número | Tipo           | Precio/noche | Estado");
            Console.WriteLine("------------------------------------------------");

            foreach (Habitacion habitacion in data.habitaciones)
            {
                string disponibilidad = habitacion.EstaDisponible() ? "Disponible" : habitacion.estadoH.ToString();
                Console.WriteLine($"{habitacion.numero,-6} | {habitacion.tipo,-14} | ${habitacion.precioNoche,-11} | {disponibilidad}");
            }

            Console.WriteLine("\n¿Desea consultar disponibilidad para fechas específicas? (s/n): ");
            string respuesta = Console.ReadLine();

            if (respuesta.ToLower() == "s")
            {
                Console.Write("\nIngrese fecha de entrada (dd/mm/yyyy): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaEntrada))
                {
                    Console.Write("Ingrese fecha de salida (dd/mm/yyyy): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaSalida))
                    {
                        Console.WriteLine("\n=== DISPONIBILIDAD PARA FECHAS ESPECÍFICAS ===\n");
                        Console.WriteLine("Número | Tipo           | Precio/noche | Disponible para esas fechas");
                        Console.WriteLine("--------------------------------------------------------------");

                        foreach (Habitacion habitacion in data.habitaciones)
                        {
                            bool disponible = VerificarDisponibilidadFechas(habitacion, fechaEntrada, fechaSalida);
                            string estado = disponible && habitacion.EstaDisponible() ? "Sí" : "No";
                            Console.WriteLine($"{habitacion.numero,-6} | {habitacion.tipo,-14} | ${habitacion.precioNoche,-11} | {estado}");
                        }
                    }
                }
            }
        }

        private void VerTodasReservas()
        {
            Console.Clear();
            Console.WriteLine("=== MIS RESERVAS ===\n");

            if (reservasCliente.Count == 0)
            {
                Console.WriteLine("No tiene reservas registradas.");
                return;
            }

            for (int i = 0; i < reservasCliente.Count; i++)
            {
                Console.WriteLine($"--- Reserva {i + 1} ---");
                reservasCliente[i].MostrarInformacion();
                if (reservasCliente[i].habitacion != null)
                {
                    Console.WriteLine($"Tipo de habitación: {reservasCliente[i].habitacion.tipo}");
                }
                Console.WriteLine();
            }
        }

        private void CancelarReserva()
        {
            Console.Clear();
            Console.WriteLine("=== CANCELAR RESERVA ===\n");

            if (reservasCliente.Count == 0)
            {
                Console.WriteLine("No tiene reservas para cancelar.");
                return;
            }

            // Mostrar reservas activas (no canceladas)
            Console.WriteLine("Seleccione la reserva a cancelar:");
            int indiceVisible = 1;
            List<int> indicesActivos = new List<int>();

            for (int i = 0; i < reservasCliente.Count; i++)
            {
                if (reservasCliente[i].GetEstadoR() != "Cancelada")
                {
                    Console.WriteLine($"{indiceVisible}. Habitación {reservasCliente[i].habitacion.numero} ({reservasCliente[i].habitacion.tipo}) - " +
                                    $"Entrada: {reservasCliente[i].GetFechaEntrada().ToShortDateString()} - " +
                                    $"Salida: {reservasCliente[i].GetFechaSalida().ToShortDateString()} - " +
                                    $"Estado: {reservasCliente[i].GetEstadoR()}");
                    indicesActivos.Add(i);
                    indiceVisible++;
                }
            }

            if (indicesActivos.Count == 0)
            {
                Console.WriteLine("No hay reservas activas para cancelar.");
                return;
            }

            Console.Write("\nSeleccione una opción: ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= indicesActivos.Count)
            {
                Reserva reservaACancelar = reservasCliente[indicesActivos[seleccion - 1]];

                Console.WriteLine($"\nDetalle de la reserva a cancelar:");
                reservaACancelar.MostrarInformacion();

                Console.Write($"\n¿Está seguro de cancelar esta reserva? (s/n): ");
                string confirmacion = Console.ReadLine();

                if (confirmacion.ToLower() == "s")
                {
                    // Cambiar el estado de la habitación de vuelta a Disponible
                    reservaACancelar.habitacion?.GetEstado(EstadoHabitacion.Disponible);
                    cliente.CancelarReserva(reservaACancelar);
                }
                else
                {
                    Console.WriteLine("Cancelación abortada.");
                }
            }
            else
            {
                Console.WriteLine("Selección inválida.");
            }
        }

        private void VerDetalleReserva()
        {
            Console.Clear();
            Console.WriteLine("=== DETALLE DE RESERVA ===\n");

            if (reservasCliente.Count == 0)
            {
                Console.WriteLine("No tiene reservas registradas.");
                return;
            }

            Console.WriteLine("Seleccione la reserva que desea ver:");
            for (int i = 0; i < reservasCliente.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Habitación {reservasCliente[i].habitacion.numero} ({reservasCliente[i].habitacion.tipo}) - " +
                                $"Entrada: {reservasCliente[i].GetFechaEntrada().ToShortDateString()} - " +
                                $"Estado: {reservasCliente[i].GetEstadoR()}");
            }

            Console.Write("\nOpción: ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= reservasCliente.Count)
            {
                Console.Clear();
                Console.WriteLine("=== DETALLE COMPLETO DE LA RESERVA ===\n");
                reservasCliente[seleccion - 1].MostrarInformacion();
                if (reservasCliente[seleccion - 1].habitacion != null)
                {
                    Console.WriteLine($"Tipo de habitación: {reservasCliente[seleccion - 1].habitacion.tipo}");
                    Console.WriteLine($"Número de habitación: {reservasCliente[seleccion - 1].habitacion.numero}");
                }
            }
            else
            {
                Console.WriteLine("Selección inválida.");
            }
        }

        private void ModificarFechasReserva()
        {
            Console.Clear();
            Console.WriteLine("=== MODIFICAR FECHAS DE RESERVA ===\n");

            if (reservasCliente.Count == 0)
            {
                Console.WriteLine("No tiene reservas para modificar.");
                return;
            }

            // Mostrar reservas no canceladas
            Console.WriteLine("Seleccione la reserva a modificar:");
            int indiceVisible = 1;
            List<int> indicesActivos = new List<int>();

            for (int i = 0; i < reservasCliente.Count; i++)
            {
                if (reservasCliente[i].GetEstadoR() != "Cancelada")
                {
                    Console.WriteLine($"{indiceVisible}. Habitación {reservasCliente[i].habitacion.numero} - " +
                                    $"Entrada: {reservasCliente[i].GetFechaEntrada().ToShortDateString()} - " +
                                    $"Salida: {reservasCliente[i].GetFechaSalida().ToShortDateString()}");
                    indicesActivos.Add(i);
                    indiceVisible++;
                }
            }

            if (indicesActivos.Count == 0)
            {
                Console.WriteLine("No hay reservas activas para modificar.");
                return;
            }

            Console.Write("\nOpción: ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= indicesActivos.Count)
            {
                Reserva reservaAModificar = reservasCliente[indicesActivos[seleccion - 1]];

                Console.WriteLine($"\nReserva actual:");
                reservaAModificar.MostrarInformacion();

                Console.Write("\nIngrese nueva fecha de entrada (dd/mm/yyyy): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime nuevaFechaEntrada))
                {
                    Console.Write("Ingrese nueva fecha de salida (dd/mm/yyyy): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime nuevaFechaSalida))
                    {
                        try
                        {
                            // Verificar disponibilidad de las nuevas fechas
                            if (VerificarDisponibilidadFechas(reservaAModificar.habitacion, nuevaFechaEntrada, nuevaFechaSalida, reservaAModificar))
                            {
                                reservaAModificar.ActualizarFechas(nuevaFechaEntrada, nuevaFechaSalida, reservaAModificar.habitacion.precioNoche);
                                Console.WriteLine("\n¡Fechas actualizadas exitosamente!");
                                Console.WriteLine($"Nuevo costo total: ${reservaAModificar.GetCostoTotal()}");
                                Console.WriteLine($"Nuevo número de noches: {reservaAModificar.GetNumeroNoches()}");
                            }
                            else
                            {
                                Console.WriteLine("Las nuevas fechas no están disponibles para esta habitación.");
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void ConsultarPrecioHabitacion()
        {
            Console.Clear();
            Console.WriteLine("=== CONSULTAR PRECIO DE HABITACIÓN ===\n");

            Console.WriteLine("Habitaciones disponibles:");
            Console.WriteLine("N°  | Número | Tipo           | Precio/noche");
            Console.WriteLine("---------------------------------------------");

            for (int i = 0; i < data.habitaciones.Count; i++)
            {
                Console.WriteLine($"{i + 1,-3} | {data.habitaciones[i].numero,-6} | {data.habitaciones[i].tipo,-14} | ${data.habitaciones[i].precioNoche}");
            }

            Console.Write("\nSeleccione una habitación: ");
            if (int.TryParse(Console.ReadLine(), out int seleccion) && seleccion > 0 && seleccion <= data.habitaciones.Count)
            {
                Habitacion habitacion = data.habitaciones[seleccion - 1];
                Console.Write("Ingrese el número de noches: ");
                if (int.TryParse(Console.ReadLine(), out int noches) && noches > 0)
                {
                    double costoTotal = habitacion.ConsultarPrecio(noches);
                    Console.WriteLine($"\nCosto total para {noches} noches: ${costoTotal}");
                }
                else
                {
                    Console.WriteLine("Número de noches inválido.");
                }
            }
            else
            {
                Console.WriteLine("Selección inválida.");
            }
        }

        private bool VerificarDisponibilidadFechas(Habitacion habitacion, DateTime fechaEntrada, DateTime fechaSalida, Reserva reservaExcluir = null)
        {
            foreach (Reserva reserva in reservasCliente)
            {
                // Si es la misma reserva que estamos modificando, la excluimos
                if (reservaExcluir != null && reserva == reservaExcluir)
                    continue;

                // Verificar si la reserva es de la misma habitación y no está cancelada
                if (reserva.habitacion != null &&
                    reserva.habitacion.numero == habitacion.numero &&
                    reserva.GetEstadoR() != "Cancelada")
                {
                    DateTime reservaEntrada = reserva.GetFechaEntrada();
                    DateTime reservaSalida = reserva.GetFechaSalida();

                    // Verificar si hay solapamiento de fechas
                    bool haySolapamiento = !(fechaSalida <= reservaEntrada || fechaEntrada >= reservaSalida);

                    if (haySolapamiento)
                    {
                        return false; // Hay conflicto de fechas
                    }
                }
            }

            return true;
        }
    }
}
