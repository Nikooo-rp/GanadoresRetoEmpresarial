using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Usuario
    {
        public string nombre = string.Empty;
        protected string contraseña = string.Empty;

        public Usuario(string nombre, string contraseña)
        {
            this.nombre = nombre;
            this.contraseña = contraseña;
        }

        public string ConsultarDisponibilidad(Habitacion h) 
        {
            return h.GetEstado();
        }

    }
}
