using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Admin: Usuario
    {
        public enum IntervaloReporte
        {
            Weekly,
            Monthly,
            Yearly
        }
        public class PeriodoIngresos
        {
            public string etiqueta;
            public DateTime inicio;
            public DateTime fin;
            public decimal ingresos;
        }
        public List<PeriodoIngresos> Generar(
        DateTime desde,
        DateTime hasta,
        IntervaloReporte intervalo)
        {
            if (desde > hasta)
                throw new ArgumentException("'from' must be before 'to'.");

            return intervalo switch
            {
                IntervaloReporte.Weekly => GroupByWeek(desde, hasta),
                IntervaloReporte.Monthly => GroupByMonth(desde, hasta),
                IntervaloReporte.Yearly => GroupByYear(desde, hasta),
                _ => throw new ArgumentOutOfRangeException(nameof(intervalo))
            };
        }
        public void ModficarCosto(Habitacion h, int nuevoCosto)
        {
            h.precioNoche = nuevoCosto;
        }
        // -----------------------------------------------------------------------------------------
        public decimal CalcularIngresos(DateTime inicio, DateTime fin) // Hace falta una lista de reservas.
        {
            decimal ingresos = 0;
            List<Reserva> reservas = new List<Reserva>();

            //Console.WriteLine("Ingrese la fecha de inicio (dd/mm/yyyy):");
            //string fechaInicio = Console.ReadLine();
            //Console.WriteLine("Ingrese la fecha de fin (dd/mm/yyyy):");
            //string fechaFin = Console.ReadLine();

            ingresos += reservas.Where(r => r.fechaEntrada >= inicio && r.fechaEntrada <= fin)
                        .Sum(r => r.costoTotal);
            return ingresos;
        }
        public string GenerarReporte(List<Reserva> reservas,DateTime inicio, DateTime fin, IntervaloReporte i)
        {
            string reporte = "";
            var resultados = Generar(
                desde: inicio,
                hasta: fin,
                intervalo: i
                ); 
            foreach (var periodo in resultados)
            {
                reporte += $"{periodo.etiqueta}: {periodo.ingresos:C}\n";
            }
            return reporte;
        }
        // ── Weekly ────────────────────────────────────────────────────────────
        private List<PeriodoIngresos> GroupByWeek(DateTime from, DateTime to)
        {
            var periods = new List<PeriodoIngresos>();

            // Snap to Monday of the starting week
            var weekStart = from.AddDays(-(int)from.DayOfWeek + (int)DayOfWeek.Monday);
            if (weekStart > from) weekStart = weekStart.AddDays(-7);

            while (weekStart <= to)
            {
                var weekEnd = weekStart.AddDays(7).AddTicks(-1);
                var clampedStart = weekStart < from ? from : weekStart;
                var clampedEnd = weekEnd > to ? to : weekEnd;

                periods.Add(new PeriodoIngresos
                {
                    etiqueta = $"Week of {weekStart:MMM dd, yyyy}",
                    inicio = clampedStart,
                    fin = clampedEnd,
                    ingresos = CalcularIngresos(clampedStart, clampedEnd)
                });

                weekStart = weekStart.AddDays(7);
            }

            return periods;
        }

        // ── Monthly ───────────────────────────────────────────────────────────
        private List<PeriodoIngresos> GroupByMonth(DateTime from, DateTime to)
        {
            var periods = new List<PeriodoIngresos>();
            var current = new DateTime(from.Year, from.Month, 1);

            while (current <= to)
            {
                var monthEnd = current.AddMonths(1).AddTicks(-1);
                var clampedStart = current < from ? from : current;
                var clampedEnd = monthEnd > to ? to : monthEnd;

                periods.Add(new PeriodoIngresos
                {
                    etiqueta = current.ToString("MMMM yyyy"),
                    inicio = clampedStart,
                    fin = clampedEnd,
                    ingresos = CalcularIngresos(clampedStart, clampedEnd)
                });

                current = current.AddMonths(1);
            }

            return periods;
        }

        // ── Yearly ────────────────────────────────────────────────────────────
        private List<PeriodoIngresos> GroupByYear(DateTime from, DateTime to)
        {
            var periods = new List<PeriodoIngresos>();
            var current = new DateTime(from.Year, 1, 1);

            while (current <= to)
            {
                var yearEnd = current.AddYears(1).AddTicks(-1);
                var clampedStart = current < from ? from : current;
                var clampedEnd = yearEnd > to ? to : yearEnd;

                periods.Add(new PeriodoIngresos
                {
                    etiqueta = current.Year.ToString(),
                    inicio = clampedStart,
                    fin = clampedEnd,
                    ingresos = CalcularIngresos(clampedStart, clampedEnd)
                });

                current = current.AddYears(1);
            }

            return periods;
        }
        // -----------------------------------------------------------------------------------------
        public void GestionarPromociones()
        {
            // Datetimes específicos para descuentos en habitaciones, etc.
        }
    }
}
