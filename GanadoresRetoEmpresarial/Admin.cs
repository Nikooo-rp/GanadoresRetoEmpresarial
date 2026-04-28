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


            // Para partir en meses, tomamos el mes de la primera y agrupamos todas las facturas que compartan ese mes en una nueva lista,
            // Al encontrarnos con una factura con un nuevo mes, la guardamos, terminamos de recorrer la lista y luego volvemos a buscar
            // facturas qeu compartan mes con la nueva, agrupándolos en una nueva lista.

            //foreach (Facturacion f in allf)
            //{
            //    if (f.fecha >= fechaInicio && f.fecha <= fechaFin)
            //    {
            //        Console.WriteLine($"Factura del: {f.fecha}, Cliente: {f.nombreCliente}, Total: {f.costoTotal}.");
            //        ingresosTotales += f.costoTotal;
            //    }
            //}

            List<Facturacion> fInRange = allf.Where (f => f.fechaFacturacion >= fechaInicio && f.fechaFacturacion <= fechaFin).ToList();
            Dictionary<int, List<Facturacion>> facturasPorMes = fInRange.GroupBy(f => f.fechaFacturacion.Month).ToDictionary(g => g.Key, g => g.ToList());

            foreach (KeyValuePair<int, List<Facturacion>> entry in facturasPorMes)
            {
                Console.WriteLine($"Ingresos de Mes #{entry.Key}");
                decimal d = CalcularIngresos(entry.Value);
                Console.WriteLine("$" + d);
                ingresosTotales += d;

            }
            Console.WriteLine($"Ingresos totales dentro del periodo:{fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}");
            Console.WriteLine("$" + ingresosTotales);
        }

        public decimal CalcularIngresos(List<Facturacion> f)
        {
            decimal ingresosTotales = 0;
            foreach (Facturacion factura in f)
            {
                ingresosTotales += factura.costoTotal;
            }
            return ingresosTotales;
        }
        public void GestionarPromociones()
        {
            // Datetimes específicos para descuentos en habitaciones, etc.
        }
    }
}
