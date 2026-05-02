using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class ServicioAdicional: Servicios
    {
        public string descripcion { get; private set; }
        public DateTime fecha { get; private set; }

        public ServicioAdicional(string tipo, double precio, string descripcion) : base(tipo, precio)
        {
            this.descripcion = descripcion;
            fecha = DateTime.Now;
        }

        public ServicioAdicional Registrar()
        {
            fecha = DateTime.Now;
            Console.WriteLine($"Servicio registrado: {this}");
            return this;
        }

        public override double CalcularCosto()
        {
            return base.CalcularCosto();
        }

        public override string ToString()
        {
            return $"ServicioAdicional: {tipo}, Precio: {precio}, Descripción: {descripcion}, Fecha: {fecha}";
        }
    }
}
