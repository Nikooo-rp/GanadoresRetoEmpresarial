using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Cliente: Usuario
    {
        public string correoCliente;
        public List<Reserva> reservasCliente = new List<Reserva>();

        public Cliente(string correo, string nombre, string contraseña) : base(nombre, contraseña)
        {
            this.correoCliente = correo;
            this.nombre = nombre;
            this.contraseña = contraseña;
        }

        // ==================== CREAR RESERVA ====================

        public Reserva? CrearReserva(DateTime fechaEntrada, DateTime fechaSalida, Habitacion habitacion)
        {
            // Validar período
            if (!Admin.isValidPeriod(fechaEntrada, fechaSalida))
            {
                Console.WriteLine("La fecha de salida debe ser posterior a la fecha de entrada.");
                return null;
            }

            // Verificar disponibilidad de fechas
            if (!VerificarDisponibilidadFechas(habitacion, fechaEntrada, fechaSalida))
            {
                Console.WriteLine("Lo siento, la habitación no está disponible para esas fechas.");
                return null;
            }

            // Crear la reserva
            Reserva nuevaReserva = new Reserva(fechaEntrada, fechaSalida, habitacion.precioNoche);
            nuevaReserva.ModificarEstado("Confirmada");
            nuevaReserva.habitacion = habitacion;

            reservasCliente.Add(nuevaReserva);

            // Cambiar estado de la habitación
            habitacion.GetEstado(EstadoHabitacion.Ocupada);

            return nuevaReserva;
        }

        // ==================== CANCELAR RESERVA ====================

        public bool CancelarReserva(Reserva reserva)
        {
            // Validación 1: ¿Existe la reserva?
            if (reserva == null)
            {
                Console.WriteLine("Error: La reserva no existe.");
                return false;
            }

            // Validación 2: ¿Ya está cancelada?
            if (reserva.GetEstadoR() == "Cancelada")
            {
                Console.WriteLine("Esta reserva ya estaba cancelada.");
                return false;
            }

            // Liberar la habitación
            if (reserva.habitacion != null)
            {
                reserva.habitacion.GetEstado(EstadoHabitacion.Disponible);
            }

            // Cancelar la reserva
            reserva.ModificarEstado("Cancelada");

            Console.WriteLine("✓ Reserva cancelada exitosamente.");
            return true;
        }

        // ==================== MODIFICAR RESERVA ====================

        public bool ModificarFechasReserva(Reserva reserva, DateTime nuevaFechaEntrada, DateTime nuevaFechaSalida)
        {
            // Validación 1: ¿Existe la reserva?
            if (reserva == null)
            {
                Console.WriteLine("Error: La reserva no existe.");
                return false;
            }

            // Validación 2: ¿Está cancelada?
            if (reserva.GetEstadoR() == "Cancelada")
            {
                Console.WriteLine("No se puede modificar una reserva cancelada.");
                return false;
            }

            // Validación 3: Fechas válidas
            if (!Admin.isValidPeriod(nuevaFechaEntrada, nuevaFechaSalida))
            {
                Console.WriteLine("La fecha de salida debe ser posterior a la fecha de entrada.");
                return false;
            }

            // Validación 4: Disponibilidad de las nuevas fechas
            if (!VerificarDisponibilidadFechas(reserva.habitacion, nuevaFechaEntrada, nuevaFechaSalida, reserva))
            {
                Console.WriteLine("Las nuevas fechas no están disponibles para esta habitación.");
                return false;
            }

            // Actualizar fechas
            reserva.ActualizarFechas(nuevaFechaEntrada, nuevaFechaSalida, reserva.habitacion.precioNoche);
            Console.WriteLine("✓ Fechas actualizadas exitosamente.");

            return true;
        }

        // ==================== CONSULTAR DISPONIBILIDAD ====================

        public List<Habitacion> ObtenerHabitacionesDisponibles(HotelData data)
        {
            return data.habitaciones.Where(h => h.EstaDisponible()).ToList();
        }

        public List<Habitacion> ObtenerHabitacionesDisponiblesParaFechas(HotelData data, DateTime entrada, DateTime salida)
        {
            var disponibles = new List<Habitacion>();

            foreach (var habitacion in data.habitaciones)
            {
                if (habitacion.EstaDisponible() && VerificarDisponibilidadFechas(habitacion, entrada, salida))
                {
                    disponibles.Add(habitacion);
                }
            }

            return disponibles;
        }

        // ==================== CONSULTAR RESERVAS ====================

        public List<Reserva> ObtenerReservasActivas()
        {
            return reservasCliente.Where(r => r.GetEstadoR() != "Cancelada").ToList();
        }

        public string ObtenerDetalleReserva(Reserva reserva)
        {
            if (reserva == null)
                return "Reserva no encontrada.";

            string detalle = string.Empty;
            detalle += $"Número de habitación: {reserva.habitacion.numero}\n";
            detalle += $"Tipo de habitación: {reserva.habitacion.tipo}\n";
            detalle += $"Fecha de entrada: {reserva.GetFechaEntrada().ToShortDateString()}\n";
            detalle += $"Fecha de salida: {reserva.GetFechaSalida().ToShortDateString()}\n";
            detalle += $"Número de noches: {reserva.GetNumeroNoches()}\n";
            detalle += $"Costo total: ${reserva.GetCostoTotal()}\n";
            detalle += $"Estado de la reserva: {reserva.GetEstadoR()}";

            return detalle;
        }

        // ==================== CONSULTAR PRECIOS ====================

        public double ConsultarPrecioHabitacion(Habitacion habitacion, int numeroNoches)
        {
            return habitacion.ConsultarPrecio(numeroNoches);
        }

        // ==================== LÓGICA PRIVADA ====================

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
