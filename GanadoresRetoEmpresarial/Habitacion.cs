using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Habitacion
    {
        public int numero, precioNoche;

        public string tipo, estado;

        public double ConsultarPrecio()
        {
            Console.WriteLine("El precio por noche de la habitación es: " + precioNoche);
            return precioNoche;
        }

        public string GetEstado()
        {
            Console.WriteLine("El estado de la habitación es: " + estado); // <-- No es necesaria esta linea
            return estado;
        }

    }
}
