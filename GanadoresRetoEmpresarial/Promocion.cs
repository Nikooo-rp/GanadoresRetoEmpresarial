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
        public (DateTime, DateTime) periodoValidez; // Una tupla, almacena la fecha de inicio (item 1) y la fecha de fin (item 2)

        // Propiedad para validacion del descuento, clamp entre 0 y 100, valor absoluto para evitar negativos
        public decimal Descuento
        {
            get { return descuento; }
            set
            {
                descuento = Math.Abs(Math.Clamp(value, 0, 100));
            }
        }

        public Promocion(string nombre, string descripcion, decimal descuento, DateTime fechaInicio, DateTime fechaFin)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.descuento = descuento;
            this.periodoValidez = (fechaInicio, fechaFin);
        }
    }
}
