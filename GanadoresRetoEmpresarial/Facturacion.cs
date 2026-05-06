using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Facturacion
    {
        private static int contadorFacturas = 0; // Es recomendable mover esto al program para seguir el numero de facturas desde ahí.

        public int numeroFactura { get; private set; }
        public DateTime fechaFacturacion { get; private set; }
        public string nombreCliente { get; private set; }
        public double costoTotal { get; private set; }

        private readonly List<string> detalleLineas = new List<string>();

        private Facturacion(string nombreCliente)
        {
            // numeroFactura = contadorFacturas;
            fechaFacturacion = DateTime.Now;
            this.nombreCliente = nombreCliente;
            costoTotal = 0.0;
        }
        public static Facturacion CalcularCostos(Habitacion habitacion, int noches, List<ServicioAdicional> servicios, string nombreCliente)
        {
            if (habitacion == null) throw new ArgumentNullException(nameof(habitacion));
            if (noches <= 0) throw new ArgumentException("El número de noches debe ser mayor a cero.");

            var f = new Facturacion(nombreCliente);

            double costoHabitacion = habitacion.ConsultarPrecio(noches);

            f.AgregarLinea($"Habitación: {habitacion.tipo} - {noches} noches x {habitacion.precioNoche} = {costoHabitacion}");
            f.costoTotal += costoHabitacion;

            if (servicios != null)
            {
                foreach (var s in servicios)
                {
                    double costoServicio = s.CalcularCosto();
                    f.AgregarLinea($"Servicio: {s.descripcion} - Costo: {costoServicio}");
                    f.costoTotal += costoServicio;
                }
            }
            return f;
        }

        private void AgregarLinea(string linea) => detalleLineas.Add(linea);

        private void ImprimirFactura()
        {
            Console.WriteLine($"Factura #{numeroFactura} - Fecha: {fechaFacturacion}");
            Console.WriteLine($"Cliente: {nombreCliente}");
            Console.WriteLine("Detalle:");
            foreach (var linea in detalleLineas)
            {
                Console.WriteLine(linea);
            }
            Console.WriteLine($"Costo Total: {costoTotal}");
        }
    }
}
