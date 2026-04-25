using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Admin: Usuario
    {
        string idAdmin = string.Empty;
        // -----------------------------------------------------------------------------------------
        public void ModificarCosto(Habitacion h, int nuevoCosto)
        {
            h.precioNoche = nuevoCosto;
            Console.WriteLine($"Costo de la habitación {h.numero} modificado a {nuevoCosto}");
        }
        public void GenerarReporte(List<Facturacion> allf)
        {
            Console.WriteLine("Ingresa el inicio del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            string inicio = Console.ReadLine();
            Console.WriteLine("Ingresa el fin del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            string fin = Console.ReadLine();

            DateTime fechaInicio = DateTime.Parse(inicio);
            DateTime fechaFin = DateTime.Parse(fin);
            decimal ingresosTotales = 0;

            foreach (Facturacion f in allf)
            {
                if (f.fecha >= fechaInicio && f.fecha <= fechaFin)
                {
                    Console.WriteLine($"Factura del: {f.fecha}, Cliente: {f.nombreCliente}, Total: {f.costoTotal}.");
                    ingresosTotales += f.costoTotal;
                }
            }
            Console.WriteLine($"Ingresos totales en el periodo {fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}: {ingresosTotales}");
        }

        public void CalcularIngresos()
        {
            // Para el cálculo de ingresos en periodos de tiempo, se hará después cuando se complejize la lógica del reporte.
        }
        public void GestionarPromociones()
        {
            // Datetimes específicos para descuentos en habitaciones, etc.
        }
    }
}
