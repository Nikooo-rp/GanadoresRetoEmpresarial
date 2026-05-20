using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Recepcionista : Usuario
    {             
        // Constructor principal. Utiliza la palabra clave 'base' para enviar         
        public Recepcionista(string nombre, string contraseña) : base(nombre, contraseña)
        {
            // No necesitamos inicializar listas locales porque usamos HotelData.
        }

        public Reserva RegistrarReserva(Cliente cliente, Habitacion habitacion, int noches, HotelData data)
        {            
            
            if (cliente == null || habitacion == null)
            {
                throw new ArgumentNullException("Faltan datos críticos para completar la reserva.");
            }

            // Validamos que la habitación realmente esté disponible antes de proceder.
            if (habitacion.estadoH != EstadoHabitacion.Disponible)
            {                
                return null;
            }

            // Lógica de fechas
            DateTime entrada = DateTime.Now;
            DateTime salida = DateTime.Now.AddDays(noches);

            // Instanciamos la nueva reserva
            Reserva nuevaReserva = new Reserva(entrada, salida, habitacion.precioNoche);
            nuevaReserva.ModificarEstado("Confirmada");
            nuevaReserva.habitacion = habitacion; // Vinculamos la habitación a la reserva

            // Cambiamos el estado de la habitación
            habitacion.GetEstado(EstadoHabitacion.Ocupada);

            // Actualizamos la base de datos central (HotelData)
            cliente.reservasCliente.Add(nuevaReserva);

            return nuevaReserva;
        }
                
        // Crea un servicio adicional, le asigna una descripción y lo guarda en HotelData.       
        public ServicioAdicional SolicitarServicioAdicional(Cliente cliente, string tipo, string descripcion, double precio, HotelData data)
        {
            if (cliente == null) return null;

            // Instanciamos el servicio pasando la descripción solicitada por el usuario.
            ServicioAdicional nuevoServicio = new ServicioAdicional(tipo, precio, descripcion, DateTime.Now);
            nuevoServicio.Registrar();

            // Guardamos el servicio en la base de datos central
            data.serviciosAdicionales.Add(nuevoServicio);

            return nuevoServicio;
        }
        
        // Finaliza la estancia del cliente, libera la habitación y genera la facturación.        
        public Facturacion FinalizarEstancia(Reserva reserva, Cliente cliente, HotelData data)
        {
            if (reserva == null || reserva.GetEstadoR() != "Confirmada") return null;

            // Cambiamos el estado de la reserva y liberamos la habitación.
            reserva.ModificarEstado("Completada");
            if (reserva.habitacion != null)
            {
                reserva.habitacion.GetEstado(EstadoHabitacion.Disponible);
            }

            // Incrementamos el número de factura para asignar un nuevo número único a esta transacción.
            data.numFactura++;
            // Calculamos los costos usando la clase Facturacion.
            Facturacion nuevaFactura = Facturacion.CalcularCostos(reserva.habitacion, reserva.GetNumeroNoches(), data.serviciosAdicionales, cliente.nombre);
            nuevaFactura.numeroFactura = data.numFactura; // Asignamos el número de factura generado.

            // Guardamos la factura en el sistema central.
            data.facturas.Add(nuevaFactura);

            // Limpiamos los servicios en memoria porque ya se cobraron.
            data.serviciosAdicionales.Clear();

            return nuevaFactura;
        }
                        
        // se genera una factura visual en consola con todos los detalles de la compra.       
        public void ImprimirFactura(Facturacion factura, HotelData data)
        {
            if (factura == null) return;

            int numeroFacturaAsignado = factura.numeroFactura; // Este número ya se asigna al generar la factura.

            // Diseño de la UI de la factura en consola
            Console.WriteLine("\n=======================================================");
            Console.WriteLine("                  HOTEL - FACTURA OFICIAL              ");
            Console.WriteLine("=======================================================");
            Console.WriteLine($"N° FACTURA:   {numeroFacturaAsignado:D4}"); 
            Console.WriteLine($"FECHA:        {factura.fechaFacturacion}");
            Console.WriteLine($"CLIENTE:      {factura.nombreCliente}");
            Console.WriteLine($"ATENDIDO POR: {this.nombre}"); // Polimorfismo: Usamos el atributo heredado de Usuario
            Console.WriteLine("-------------------------------------------------------");
                        
            Console.WriteLine("                       DETALLE                         ");
            Console.WriteLine("-------------------------------------------------------");
                         
            // mostramos el costo total directamente.
            Console.WriteLine($"TOTAL A PAGAR:                      ${factura.costoTotal}");
            Console.WriteLine("=======================================================\n");
            Console.WriteLine("             ¡Gracias por su preferencia!              \n");
        }
                
        // Busca a un cliente en la base de datos por su correo.        
        public Cliente ConsultarInfoHuesped(string correoCliente, HotelData data)
        {
            if (string.IsNullOrEmpty(correoCliente)) return null;
            
            return data.clientes.FirstOrDefault(c => c.correoCliente == correoCliente);
        }
    }
}

