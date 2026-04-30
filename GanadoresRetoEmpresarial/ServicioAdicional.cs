using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class ServicioAdicional: Servicios
    {
        // Campos adicionales
        private string descripcion;
        private DateTime fecha;

        // Constructor
        public ServicioAdicional(string tipo, double precio, string descripcion, DateTime fecha)
            : base(tipo, precio)
        {
            this.descripcion = descripcion;
            this.fecha = fecha;
        }

        // Getters y Setters
        public string GetDescripcion()
        {
            return this.descripcion;
        }

        public void SetDescripcion(string descripcion)
        {
            this.descripcion = descripcion;
        }

        public DateTime GetFecha()
        {
            return this.fecha;
        }

        public void SetFecha(DateTime fecha)
        {
            this.fecha = fecha;
        }

        // Método para registrar el servicio adicional
        public ServicioAdicional Registrar()
        {
            Console.WriteLine($"Registrando servicio adicional: {this.descripcion}");
            Console.WriteLine($"Fecha: {this.fecha.ToShortDateString()}");
            Console.WriteLine($"Tipo: {this.GetTipo()}");
            Console.WriteLine($"Precio: {this.GetPrecio():C}");

            // Aquí se podría agregar lógica para guardar en base de datos
            return this;
        }

        // Método para calcular el costo del servicio adicional
        public double CalcularCosto()
        {
            // Se puede agregar lógica adicional como impuestos, descuentos, etc.
            double costoBase = this.GetPrecio();
            double impuesto = costoBase * 0.19; // 19% de IVA como ejemplo
            double costoTotal = costoBase + impuesto;

            Console.WriteLine($"Cálculo de costo para {this.descripcion}:");
            Console.WriteLine($"  Precio base: {costoBase:C}");
            Console.WriteLine($"  Impuesto (19%): {impuesto:C}");
            Console.WriteLine($"  Costo total: {costoTotal:C}");

            return costoTotal;
        }

        // Sobrescribir método para mostrar información
        public override void MostrarInformacion()
        {
            base.MostrarInformacion();
            Console.WriteLine($"Descripción: {this.descripcion}");
            Console.WriteLine($"Fecha del servicio: {this.fecha.ToShortDateString()}");
            Console.WriteLine($"Costo total con impuestos: {this.CalcularCosto():C}");
        }
    }
}
