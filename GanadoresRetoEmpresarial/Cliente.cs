using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Cliente: Usuario
    {
        public string correo = string.Empty;
        
        public Reserva CrearReserva(Habitacion h)
        {
            Reserva r = new Reserva();
            // Lógica.
            return r;
        }
        public string ConsultarReserva(Habitacion h)
        {
            string rta = string.Empty;
            // Lógica.
            return rta;
        }
        public void CancelarReserva(Habitacion h)
        {
        }
    }
}
