using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Promocion
    {
        public string nombre;
        public string descripcion;
        private decimal descuento; // Porcentaje
        public (DateTime, DateTime) periodoValidez;

        // Propiedad para validaciones para descuento: entre 0 y 100, debe ser positivo.
        public decimal Descuento
        {
            get { return descuento;  }
            // clamp para dejarlo entre 0 y 100, abs para asegurarnos que sea positivo.
            set { descuento = Math.Abs(Math.Clamp(value, 0, 100)); }
        }

        public Promocion(string nombre, string descripcion, decimal descuento, DateTime fechaInicio, DateTime fechaFin)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.Descuento = descuento;
            this.periodoValidez = (fechaInicio, fechaFin);
        }
    }
}
