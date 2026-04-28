using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Facturacion
    {
        public int numeroFactura;
        public DateTime fecha;
        public string nombreCliente;
        public double costoTotal;

        public double  CalcularCostos(Reserva r)
        {
            return costoTotal;
        }
    }
}
