using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class MenuCliente
    {
        private HotelData data;
        private Cliente cliente;

        public void MostrarMenu(Cliente cliente, HotelData data)
        {
            this.cliente = cliente;
            this.data = data;

            int opcion;
            do
            {
                Console.Clear();
                MostrarEncabezado();

                opcion = AskTypes.AskInt("\nSeleccione una opción: ");

                switch (opcion)
                {
                    case 1: EjecutarConMensaje(CrearReserva); break;
                    case 2: EjecutarConMensaje(ConsultarDisponibilidad); break;
                    case 3: EjecutarConMensaje(VerTodasReservas); break;
                    case 4: EjecutarConMensaje(CancelarReserva); break;
                    case 5: EjecutarConMensaje(VerDetalleReserva); break;
                    case 6: EjecutarConMensaje(ModificarFechasReserva); break;
                    case 7: EjecutarConMensaje(ConsultarPrecioHabitacion); break;
                    case 0: Console.WriteLine("\n¡Gracias por usar nuestro servicio!"); break;
                    default: Console.WriteLine("Opción no válida"); break;
                }

                if (opcion != 0) Pausar();

            } while (opcion != 0);
        }

        // ==================== MÉTODOS DE UI (responsabilidad principal) ====================

        private void MostrarEncabezado()
        {
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
        }

        private void Pausar()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void EjecutarConMensaje(Action accion)
        {
            Console.Clear();
            accion();
        }

        // ==================== MÉTODOS QUE SOLO PIDEN DATOS Y MUESTRAN RESULTADOS ====================

        private void CrearReserva()
        {
            Console.WriteLine("=== CREAR NUEVA RESERVA ===\n");

            var fechas = SolicitarFechas();
            if (!fechas.HasValue) return;

            var habitaciones = cliente.ObtenerHabitacionesDisponiblesParaFechas(data, fechas.Value.entrada, fechas.Value.salida);

            if (habitaciones.Count == 0)
            {
                Console.WriteLine("No hay habitaciones disponibles para esas fechas.");
                return;
            }

            MostrarListaHabitaciones(habitaciones);

            var habitacion = SeleccionarHabitacion(habitaciones);
            if (habitacion == null) return;

            var reserva = cliente.CrearReserva(fechas.Value.entrada, fechas.Value.salida, habitacion);

            if (reserva != null)
            {
                Console.WriteLine("\n✓ ¡Reserva creada exitosamente!");
                Console.WriteLine(cliente.ObtenerDetalleReserva(reserva));
            }
        }

        private void CancelarReserva()
        {
            Console.WriteLine("=== CANCELAR RESERVA ===\n");

            var activas = cliente.ObtenerReservasActivas();
            if (activas.Count == 0)
            {
                Console.WriteLine("No tienes reservas activas.");
                return;
            }

            MostrarReservasResumidas(activas);

            var reserva = SeleccionarReserva(activas);
            if (reserva == null) return;

            Console.WriteLine("\n--- Detalle de la reserva ---");
            Console.WriteLine(cliente.ObtenerDetalleReserva(reserva));

            if (Confirmar("¿Cancelar esta reserva?"))
                cliente.CancelarReserva(reserva);
        }

        private void ModificarFechasReserva()
        {
            Console.WriteLine("=== MODIFICAR FECHAS ===\n");

            var activas = cliente.ObtenerReservasActivas();
            if (activas.Count == 0)
            {
                Console.WriteLine("No hay reservas activas.");
                return;
            }

            MostrarReservasResumidas(activas);

            var reserva = SeleccionarReserva(activas);
            if (reserva == null) return;

            Console.WriteLine("\nReserva actual:");
            Console.WriteLine(cliente.ObtenerDetalleReserva(reserva));

            var nuevasFechas = SolicitarFechas();
            if (!nuevasFechas.HasValue) return;

            if (cliente.ModificarFechasReserva(reserva, nuevasFechas.Value.entrada, nuevasFechas.Value.salida))
            {
                Console.WriteLine($"\n✓ Nuevo costo: ${reserva.GetCostoTotal()}");
                Console.WriteLine($"  Nuevas noches: {reserva.GetNumeroNoches()}");
            }
        }

        private void VerTodasReservas()
        {
            Console.WriteLine("=== MIS RESERVAS ===\n");

            var reservas = cliente.reservasCliente;
            if (reservas.Count == 0)
            {
                Console.WriteLine("No tiene reservas.");
                return;
            }

            for (int i = 0; i < reservas.Count; i++)
            {
                Console.WriteLine($"--- Reserva {i + 1} ---");
                Console.WriteLine(cliente.ObtenerDetalleReserva(reservas[i]));
                Console.WriteLine();
            }
        }

        private void VerDetalleReserva()
        {
            Console.WriteLine("=== DETALLE DE RESERVA ===\n");

            var reservas = cliente.reservasCliente;
            if (reservas.Count == 0)
            {
                Console.WriteLine("No tiene reservas.");
                return;
            }

            MostrarReservasResumidas(reservas);

            var reserva = SeleccionarReserva(reservas);
            if (reserva == null) return;

            Console.Clear();
            Console.WriteLine("=== DETALLE COMPLETO ===\n");
            Console.WriteLine(cliente.ObtenerDetalleReserva(reserva));
        }

        private void ConsultarDisponibilidad()
        {
            Console.WriteLine("=== DISPONIBILIDAD ===\n");

            MostrarTablaHabitaciones(data.habitaciones);

            if (Confirmar("¿Consultar disponibilidad para fechas específicas?"))
            {
                var fechas = SolicitarFechas();
                if (fechas.HasValue)
                {
                    Console.WriteLine("\n=== HABITACIONES DISPONIBLES ===\n");
                    var disponibles = cliente.ObtenerHabitacionesDisponiblesParaFechas(data, fechas.Value.entrada, fechas.Value.salida);
                    MostrarTablaHabitaciones(disponibles);
                }
            }
        }

        private void ConsultarPrecioHabitacion()
        {
            Console.WriteLine("=== CONSULTAR PRECIO ===\n");

            MostrarTablaPrecios(data.habitaciones);

            var habitacion = SeleccionarHabitacion(data.habitaciones);
            if (habitacion == null) return;

            Console.Write("Número de noches: ");
            if (int.TryParse(Console.ReadLine(), out int noches) && noches > 0)
            {
                Console.WriteLine($"\nPrecio por noche: ${habitacion.precioNoche}");
                Console.WriteLine($"Total para {noches} noches: ${cliente.ConsultarPrecioHabitacion(habitacion, noches)}");
            }
        }

        // ==================== MÉTODOS AUXILIARES SIMPLES ====================

        private (DateTime entrada, DateTime salida)? SolicitarFechas()
        {
            Console.Write("Fecha de entrada (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime entrada))
            {
                Console.WriteLine("Fecha inválida.");
                return null;
            }

            Console.Write("Fecha de salida (dd/mm/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime salida))
            {
                Console.WriteLine("Fecha inválida.");
                return null;
            }

            return (entrada, salida);
        }

        private Habitacion? SeleccionarHabitacion(List<Habitacion> habitaciones)
        {
            Console.Write("\nSeleccione una habitación: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= habitaciones.Count)
                return habitaciones[idx - 1];

            Console.WriteLine("Selección inválida.");
            return null;
        }

        private Reserva? SeleccionarReserva(List<Reserva> reservas)
        {
            Console.Write("\nOpción: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= reservas.Count)
                return reservas[idx - 1];

            Console.WriteLine("Opción inválida.");
            return null;
        }

        private bool Confirmar(string mensaje)
        {
            Console.Write($"\n{mensaje} (s/n): ");
            return Console.ReadLine()?.ToLower() == "s";
        }

        // ==================== MÉTODOS DE VISUALIZACIÓN ====================

        private void MostrarListaHabitaciones(List<Habitacion> habitaciones)
        {
            Console.WriteLine("\nN°  | Número | Tipo           | Precio/noche");
            Console.WriteLine("---------------------------------------------");
            for (int i = 0; i < habitaciones.Count; i++)
                Console.WriteLine($"{i + 1,-3} | {habitaciones[i].numero,-6} | {habitaciones[i].tipo,-14} | ${habitaciones[i].precioNoche}");
        }

        private void MostrarTablaHabitaciones(List<Habitacion> habitaciones)
        {
            Console.WriteLine("Número | Tipo           | Precio/noche | Estado");
            Console.WriteLine("------------------------------------------------");
            foreach (var h in habitaciones)
                Console.WriteLine($"{h.numero,-6} | {h.tipo,-14} | ${h.precioNoche,-11} | {(h.EstaDisponible() ? "Disponible" : h.estadoH)}");
        }

        private void MostrarTablaPrecios(List<Habitacion> habitaciones)
        {
            Console.WriteLine("N°  | Número | Tipo           | Precio/noche");
            Console.WriteLine("---------------------------------------------");
            for (int i = 0; i < habitaciones.Count; i++)
                Console.WriteLine($"{i + 1,-3} | {habitaciones[i].numero,-6} | {habitaciones[i].tipo,-14} | ${habitaciones[i].precioNoche}");
        }

        private void MostrarReservasResumidas(List<Reserva> reservas)
        {
            for (int i = 0; i < reservas.Count; i++)
            {
                var r = reservas[i];
                Console.WriteLine($"{i + 1}. Habitación {r.habitacion.numero} ({r.habitacion.tipo})");
                Console.WriteLine($"   {r.GetFechaEntrada():dd/MM/yyyy} → {r.GetFechaSalida():dd/MM/yyyy} | ${r.GetCostoTotal()}\n");
            }
        }
    }
}
