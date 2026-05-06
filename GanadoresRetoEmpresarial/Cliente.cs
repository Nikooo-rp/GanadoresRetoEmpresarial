using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Cliente: Usuario
    {
        public string correoCliente;

        public Cliente(string correo, string nombre, string contraseña) : base(nombre, contraseña)
        {
            this.correoCliente = correo;
            this.nombre = nombre;
            this.contraseña = contraseña;
        }
        
        public Reserva CrearReserva(DateTime entrada, int numeroNoches, Habitacion h)
        {
            Reserva r = new Reserva();
            //Console.WriteLine("Ingresa el número de la habitación que deseas reservar:");
            //int numeroHabitacion = int.Parse(Console.ReadLine());
            //Console.WriteLine("Ingresa la fecha de entrada de tu reserva (dd/mm/yyyy):");
            //string fechaEntrada = Console.ReadLine();
            //Console.WriteLine("Ingresa el número de noches que vas a quedarte:");
            //int numeroNoches = int.Parse(Console.ReadLine());

            r.fechaEntrada = entrada;
            r.fechaSalida = entrada.AddDays(numeroNoches);

            r.habitacion = h;
            r.costoTotal = h.precioNoche * numeroNoches;

            // Lógica.
            return r;
        }
        public string ConsultarReserva(Reserva r)
        {
            string rta = string.Empty;
            rta += $"Número de habitación: {r.habitacion.numero}\n";
            rta += $"Fecha de entrada: {r.fechaEntrada.ToShortDateString()}\n";
            rta += $"Fecha de salida: {r.fechaSalida.ToShortDateString()}\n";
            rta += $"Costo total: {r.costoTotal}\n";
            rta += $"Estado de la reserva: {r.estado}\n";
            return rta;
        }
        public void CancelarReserva(Reserva r)
        {
            r.CancelarReserva();
            Console.WriteLine("Reserva cancelada exitosamente.");
        }
    }
}
