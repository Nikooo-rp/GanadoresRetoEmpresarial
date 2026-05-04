using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Servicios
    {
        public string tipo { get; protected set; }
        public double precio { get; protected set; }

        public Servicios(string tipo, double precio)
        {
            this.tipo = tipo;
            this.precio = precio;
        }

        public virtual double CalcularCosto()
        {
            return precio;
        }

        public override string ToString()
        {
            return $"Servicio: {tipo}, Precio: {precio}";
        }
    }
}
