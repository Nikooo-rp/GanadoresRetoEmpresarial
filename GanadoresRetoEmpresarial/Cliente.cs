using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Cliente: Usuario
    {
        public string correo = string.Empty;
        
        public Reserva CrearReserva(Habitacion h)
        {
            Reserva r = new Reserva();
            Console.WriteLine("Ingresa el número de la habitación que deseas reservar:");
            int numeroHabitacion = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingresa la fecha de entrada de tu reserva (dd/mm/yyyy):");
            string fechaEntrada = Console.ReadLine();
            Console.WriteLine("Ingresa el número de noches que vas a quedarte:");
            int numeroNoches = int.Parse(Console.ReadLine());

            r.fechaEntrada = DateTime.Parse(fechaEntrada);
            r.fechaSalida = r.fechaEntrada.AddDays(numeroNoches);

            r.habitacion = h;
            r.costoTotal = h.precioNoche * numeroNoches;

            // Lógica.
            return r;
        }
        public string ConsultarReserva(Habitacion h)
        {
            string rta = string.Empty;
            // Lógica.
            return rta;
        }
        public void CancelarReserva(Habitacion h)
        {
        }
    }
}
