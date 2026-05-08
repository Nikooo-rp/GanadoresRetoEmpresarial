using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Recepcionista : Usuario
    {
        public string idRecepcionista = string.Empty;
        public Reserva reservaGestionada;

        public List<Reserva> Reservas = new List<Reserva>();
        public List<Cliente> ClientesActivos = new List<Cliente>();

        public Recepcionista(string nombre, string contraseña, string idRecepcionista) : base(nombre, contraseña)
        {
            this.idRecepcionista = idRecepcionista;
        }             

        public Reserva RegistrarReserva(Cliente cliente, Habitacion habitacion, int noches)
        {
            /* ¿Por qué evaluamos contra 'null' y usamos '||')? 'null' representa la ausencia total de información en el sistema. 
              El operador '||' significa "O". Con esto le decimos al sistema:
             "Si el registro del cliente está vacío, O si el registro de la habitación está vacío, 
             detén el proceso inmediatamente". Es una medida de seguridad para impedir 
             que el algoritmo intente crear una transacción con información incompleta.*/
            if (cliente == null || habitacion == null)
            {
                Console.WriteLine("[ERROR] Faltan datos del cliente o la habitación.");
                return null;
            }

            if (habitacion.estadoH != EstadoHabitacion.Disponible)
            {
                Console.WriteLine($"[AVISO] Imposible asignar habitación {habitacion.numero}. Estado actual: {habitacion.estadoH}.");
                return null;
            }

            DateTime entrada = DateTime.Now;
            DateTime salida = DateTime.Now.AddDays(noches);
            Reserva nuevaReserva = new Reserva(entrada, salida, habitacion.precioNoche);

            nuevaReserva.ModificarEstado("Confirmada");
            habitacion.GetEstado(EstadoHabitacion.Ocupada);

            cliente.Reservas.Add(nuevaReserva);
            this.Reservas.Add(nuevaReserva);
            this.reservaGestionada = nuevaReserva;

            Console.WriteLine($"[ÉXITO] Reserva asignada a {cliente.correoCliente}. Habitación {habitacion.numero} reservada.");
            return nuevaReserva;
        }

        public Reserva CancelarReserva(Reserva reserva)
        {
            if (reserva == null || reserva.GetEstadoR() == "Completada") return reserva;
            reserva.ModificarEstado("Cancelada");
            Console.WriteLine("[ÉXITO] Reserva cancelada correctamente.");
            return reserva;
        }

        public Reserva ModificarHabitación(Reserva reserva, Habitacion habitacionActual, Habitacion nuevaHabitacion)
        {
            if (reserva == null || habitacionActual == null || nuevaHabitacion == null) return reserva;

            if (nuevaHabitacion.estadoH != EstadoHabitacion.Disponible)
            {
                Console.WriteLine($"[AVISO] La habitación {nuevaHabitacion.numero} ya se encuentra ocupada.");
                return reserva;
            }

            habitacionActual.GetEstado(EstadoHabitacion.Disponible);
            nuevaHabitacion.GetEstado(EstadoHabitacion.Ocupada);
            reserva.RecalcularCosto(nuevaHabitacion.precioNoche);

            Console.WriteLine($"[ÉXITO] Habitación actualizada a {nuevaHabitacion.numero}. Costo recalculado.");
            return reserva;
        }

        public void FinalizarEstancia(Reserva reserva, Habitacion habitacion, Cliente cliente, List<ServicioAdicional> servicios)
        {
            if (reserva == null || reserva.GetEstadoR() != "Confirmada") return;

            reserva.ModificarEstado("Completada");
            habitacion.GetEstado(EstadoHabitacion.Disponible);

            Facturacion nuevaFactura = Facturacion.CalcularCostos(habitacion, reserva.GetNumeroNoches(), servicios, cliente.nombre);
            Console.WriteLine($"[ÉXITO] Estancia finalizada. Generando facturación para {cliente.nombre}...");
        }

        public Cliente ConsultarInfoHuesped(string correoCliente)
        {
            if (string.IsNullOrEmpty(correoCliente)) return null;

            foreach (Cliente nodoCliente in ClientesActivos)
            {
                if (nodoCliente != null && nodoCliente.correoCliente == correoCliente)
                {
                    Console.WriteLine($"[ÉXITO] Información encontrada para {nodoCliente.nombre}.");
                    return nodoCliente;
                }
            }
            Console.WriteLine("[FALLO] Cliente no encontrado.");
            return null;
        }

        public ServicioAdicional SolicitarServicioAdicional(Cliente cliente, string tipo, double precio)
        {
            if (cliente == null) return null;

            ServicioAdicional nuevoServicio = new ServicioAdicional("Adicional", precio, tipo, DateTime.Now);
            nuevoServicio.Registrar();

            Console.WriteLine($"[ÉXITO] Servicio '{tipo}' registrado.");
            return nuevoServicio;
        }

        public double MostrarCosto(Reserva reserva)
        {
            if (reserva == null) return 0.0;
            return reserva.GetCostoTotal();
        }
                       
    }
}
