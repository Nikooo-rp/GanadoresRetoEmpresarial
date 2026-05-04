using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Promocion
    {
        public string nombre;
        public string descripcion;
        public decimal descuento; // Porcentaje
        public (DateTime, DateTime) periodoValidez;

        public Promocion(string nombre, string descripcion, decimal descuento, DateTime fechaInicio, DateTime fechaFin)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.descuento = descuento;
            this.periodoValidez = (fechaInicio, fechaFin);
        }
    }
}
