using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Habitacion
    {
        public int numero;
        
        public double precioNoche;

        public string tipo, estadoH;

        public double ConsultarPrecio()
        {
            Console.WriteLine("El precio por noche de la habitación es: " + precioNoche);
            return precioNoche;
        }

        public string GetEstado()
        {
            return estado;
        }

    }
}
