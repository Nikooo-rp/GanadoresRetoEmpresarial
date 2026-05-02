using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Recepcionista: Usuario
    {
        public string idRecepcionista = string.Empty;
        public Reserva? reservaGestionada;

        // Estructura de datos 
        public List<Reserva> Reservas = new List<Reserva>();
        public List<Cliente> ClientesActivos = new List<Cliente>();

        // 1. REGISTRAR RESERVA        
        public Reserva? RegistrarReserva(Cliente cliente, Habitacion habitacion, int noches)
        {
           /* ¿Por qué evaluamos contra 'null' y usamos '||')? 'null' representa la ausencia total de información en el sistema. 
             El operador '||' significa "O". Con esto le decimos al sistema:
            "Si el registro del cliente está vacío, O si el registro de la habitación está vacío, 
            detén el proceso inmediatamente". Es una medida de seguridad para impedir 
            que el algoritmo intente crear una transacción con información incompleta.*/
            if (cliente == null || habitacion == null)
            {
                Console.WriteLine("Error: Faltan los datos del cliente o no se ha seleccionado una habitación disponible para procesar la reserva.");
                return null;
            }
            if (habitacion.estadoH != "Disponible")
            {
                Console.WriteLine($"Aviso: No es posible asignar la habitación {habitacion.numero} porque actualmente se encuentra ocupada o en mantenimiento.");
                return null;
            }
                        
            Reserva nuevaReserva = new Reserva
            {
                fechaEntrada = DateTime.Now,
                fechaSalida = DateTime.Now.AddDays(noches),
                numeroNoches = noches,
                estadoR = "Activa",
                costoTotal = habitacion.precioNoche * noches
            };

            // Modificacion de estado
            habitacion.estadoH = "Ocupada";
                        
            cliente.Reservas.Add(nuevaReserva);
            this.Reservas.Add(nuevaReserva);
            this.reservaGestionada = nuevaReserva; 
                        
            if (!ClientesActivos.Contains(cliente))
            {
                ClientesActivos.Add(cliente);
            }

            Console.WriteLine($"Transacción completada: Reserva asignada a {cliente.correoCliente}. Habitación {habitacion.numero} reservada.");
            return nuevaReserva;
        }

        // 2. CANCELAR RESERVA 
        public Reserva? CancelarReserva(Reserva reserva)
        {
            if (reserva == null || reserva.estadoR == "Finalizada") return reserva;

            reserva.estadoR = "Cancelada";
                        
            Console.WriteLine("Reserva cancelada,");
            return reserva;
        }

        // 3. MODIFICAR HABITACIÓN
        public Reserva? ModificarHabitación(Reserva reserva, Habitacion habitacionActual, Habitacion nuevaHabitacion)
        {
            if (reserva == null || habitacionActual == null || nuevaHabitacion == null) return reserva;

            if (nuevaHabitacion.estadoH != "Disponible")
            {
                Console.WriteLine($"Aviso: Imposible modificar La habitación: {nuevaHabitacion.numero}. ya se encuentra ocupada.");
                return reserva;
            }
                       
            habitacionActual.estadoH = "Disponible";
                        
            nuevaHabitacion.estadoH = "Ocupada";
                        
            reserva.costoTotal = nuevaHabitacion.precioNoche * reserva.numeroNoches;

            Console.WriteLine($"Nueva habitación asignada: {nuevaHabitacion.numero}. Costo recalculado.");
            return reserva;
        }

        // 4. FINALIZAR ESTANCIA 
        public void FinalizarEstancia(Reserva reserva, Cliente cliente)
        {
            if (reserva == null || reserva.estadoR != "Activa") return;
                        
            reserva.estadoR = "Finalizada";
                        
            Facturacion nuevaFactura = new Facturacion
            {
                numeroFactura = new Random().Next(1000, 9999),
                fechaFacturacion = DateTime.Now,
                nombreCliente = cliente.correoCliente, 
                CostoTotal = MostrarCosto(reserva)
            };

            Console.WriteLine($"Ciclo de vida cerrado. Factura {nuevaFactura.numeroFactura} generada en memoria por un valor de {nuevaFactura.CostoTotal}.");
        }

        // 5. CONSULTAR INFO HUÉSPED 
        public Cliente? CosultarInfoHuesped(string correoCliente)
        {
            if (string.IsNullOrEmpty(correoCliente)) return null;
                        
            foreach (Cliente nodoCliente in ClientesActivos)
            {
                if (nodoCliente != null && nodoCliente.correoCliente == correoCliente)
                {
                    Console.WriteLine($"Búsqueda exitosa: Información del cliente encontrada.");
                    return nodoCliente;
                }
            }

            Console.WriteLine("Fallo en la búsqueda: Información no encontrada.");
            return null;
        }

        // 6. SOLICITAR SERVICIO ADICIONAL 
        public ServicioAdicional? SolicitarServicioAdicional(Cliente cliente, string tipo, double precio)
        {
            if (cliente == null) return null;

            ServicioAdicional nuevoServicio = new ServicioAdicional
            {
                descripcion = tipo,
                fecha = DateTime.Now
            };
                        
            nuevoServicio.Registrar();

            Console.WriteLine($"Confirmación: El servicio adicional de '{tipo}' ha sido registrado exitosamente en la cuenta del cliente.");
            return nuevoServicio;
        }

        // 7. MOSTRAR COSTO
        public double MostrarCosto(Reserva reserva)
        {
            if (reserva == null) return 0.0;

          /* la Reserva debería autocalcular su costo 
          sumando la habitación y los servicios. */
            double costoBase = reserva.costoTotal;
                        
            return costoBase;
        }
    }
}
