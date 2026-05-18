using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class HotelData
    {
        public List<Cliente> clientes = new List<Cliente>();
        public List<Admin> admins = new List<Admin>();
        public List<Recepcionista> recepcionistas = new List<Recepcionista>();
        public List<Facturacion> facturas = new List<Facturacion>();
        public List<Habitacion> habitaciones = new List<Habitacion>();
        public List<Promocion> promociones = new List<Promocion>();
        public List<ServicioAdicional> serviciosAdicionales = new List<ServicioAdicional>();

        public int contadorFacturas = 0;
    }
}
