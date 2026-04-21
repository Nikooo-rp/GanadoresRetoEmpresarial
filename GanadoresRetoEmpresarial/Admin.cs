using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Admin: Usuario
    {
        public void ModficarCosto(Habitacion h, int nuevoCosto)
        {
            h.precioNoche = nuevoCosto;
        }
        public decimal CalcularIngresos() // Hace falta una lista de reservas.
        {
            decimal ingresos = 0;
            List<Reserva> reservas = new List<Reserva>();

            Console.WriteLine("Ingrese la fecha de inicio (dd/mm/yyyy):");
            string fechaInicio = Console.ReadLine();
            Console.WriteLine("Ingrese la fecha de fin (dd/mm/yyyy):");
            string fechaFin = Console.ReadLine();

            ingresos += reservas.Where(r => r.fechaEntrada >= DateTime.Parse(fechaInicio) && r.fechaEntrada <= DateTime.Parse(fechaFin))
                        .Sum(r => r.costoTotal);
            return ingresos;
        }
        public void GenerarReporte()
        {
            // Reporta ingresos en periodo de tiempo (semanal, mensual, anual).
        }
        public void GestionarPromociones()
        {
            // Datetimes específicos para descuentos en habitaciones, etc.
        }
    }
}
