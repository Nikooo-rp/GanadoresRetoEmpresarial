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
        
        public Reserva CrearReserva(DateTime entrada, int numeroNoches, Habitacion h)
        {
            Reserva r = new Reserva(entrada, entrada.AddDays(numeroNoches), h.precioNoche);
            return r;
        }
        public string ConsultarReserva(Reserva r)
        {
            string rta = string.Empty;
            rta += $"Número de habitación: {r.habitacion.numero}\n";
            rta += $"Fecha de entrada: {r.GetFechaEntrada().ToShortDateString()}\n";
            rta += $"Fecha de salida: {r.GetFechaSalida().ToShortDateString()}\n";
            rta += $"Costo total: {r.GetCostoTotal()}\n";
            rta += $"Estado de la reserva: {r.GetEstadoR()}\n";
            return rta;
        }
        public void CancelarReserva(Reserva r)
        {
            r.ModificarEstado("Cancelada");
            Console.WriteLine("Reserva cancelada exitosamente.");
        }

        internal Reserva CrearReserva(DateTime fechaEntrada, DateTime fechaSalida, Habitacion habitacionSeleccionada)
        {
            throw new NotImplementedException();
        }
    }
}
