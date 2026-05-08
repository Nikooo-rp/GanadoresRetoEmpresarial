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

        public void MenuRecepcionista(List<Habitacion> habitacionesHotel, List<Cliente> baseDatosClientes, List<ServicioAdicional> serviciosGlobales)
        {
            bool sesionActiva = true;
                        
            this.ClientesActivos = baseDatosClientes;

            while (sesionActiva)
            {
                Console.Clear();
                Console.WriteLine("======================================");
                Console.WriteLine("   MÓDULO DE RECEPCIÓN");
                Console.WriteLine("======================================");
                Console.WriteLine($"Recepcionista: {this.nombre} | ID: {this.idRecepcionista}");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("1. Registrar nuevo Cliente");
                Console.WriteLine("2. Ver disponibilidad de Habitaciones");
                Console.WriteLine("3. Crear nueva Reserva");
                Console.WriteLine("4. Agregar Servicio Adicional (Ej. Desayuno)");
                Console.WriteLine("5. Procesar Check-out (Finalizar Estancia)");
                Console.WriteLine("6. Regresar al Menú Principal");
                Console.WriteLine("======================================");
                Console.Write("Seleccione una operación: ");

                string opcion = Console.ReadLine();
                Console.WriteLine();

                switch (opcion)
                {
                    case "1":
                        Console.Write("Nombre del cliente: ");
                        string nombre = Console.ReadLine();
                        Console.Write("Correo del cliente (Servirá como ID): ");
                        string correo = Console.ReadLine();

                        Cliente nuevoCliente = new Cliente(nombre, "1234", correo);
                        this.ClientesActivos.Add(nuevoCliente);
                        Console.WriteLine("[ÉXITO] Cliente guardado en la base de datos.");
                        break;

                    case "2":
                        Console.WriteLine("--- HABITACIONES DEL HOTEL ---");
                        foreach (Habitacion h in habitacionesHotel)
                        {
                            Console.WriteLine(h.ToString());
                        }
                        break;

                    case "3":
                        Console.Write("Ingrese el correo del cliente: ");
                        string correoBusqueda = Console.ReadLine();
                        Cliente clienteReserva = this.ConsultarInfoHuesped(correoBusqueda);

                        if (clienteReserva != null)
                        {
                            Console.Write("Ingrese el número de la habitación: ");
                            if (int.TryParse(Console.ReadLine(), out int numHabitacion))
                            {
                                Habitacion habSeleccionada = habitacionesHotel.Find(h => h.numero == numHabitacion);

                                Console.Write("Ingrese cantidad de noches: ");
                                if (int.TryParse(Console.ReadLine(), out int noches))
                                {
                                    this.RegistrarReserva(clienteReserva, habSeleccionada, noches);
                                }
                            }
                        }
                        break;

                    case "4":
                        Console.Write("Correo del cliente: ");
                        string correoServ = Console.ReadLine();                                                
                        Cliente clienteEncontrado = this.ConsultarInfoHuesped(correoServ);

                        if (clienteEncontrado != null)
                        {
                            Console.Write("Descripción del servicio: ");
                            string desc = Console.ReadLine();
                            Console.Write("Costo del servicio: ");
                            if (double.TryParse(Console.ReadLine(), out double precioSrv))
                            {
                                
                                ServicioAdicional sa = this.SolicitarServicioAdicional(clienteEncontrado, desc, precioSrv);
                                if (sa != null) serviciosGlobales.Add(sa);
                            }
                        }
                        break;

                    case "5":
                        Console.Write("Correo del cliente para Check-out: ");
                        string correoOut = Console.ReadLine();

                        
                        Cliente clienteOut = this.ConsultarInfoHuesped(correoOut);

                        if (clienteOut != null && clienteOut.Reservas.Count > 0)
                        {
                            Reserva resCierre = clienteOut.Reservas[0];
                            Habitacion habCierre = habitacionesHotel.Find(h => h.estadoH == EstadoHabitacion.Ocupada);

                            if (habCierre != null)
                            {                                
                                this.FinalizarEstancia(resCierre, habCierre, clienteOut, serviciosGlobales);
                                serviciosGlobales.Clear();
                            }
                        }
                        else
                        {
                            Console.WriteLine("[AVISO] El cliente no existe o no tiene reservas activas.");
                        }
                        break;

                    case "6":
                        sesionActiva = false;
                        Console.WriteLine("Saliendo del Módulo de Recepción...");
                        break;

                    default:
                        Console.WriteLine("[ERROR] Instrucción no válida.");
                        break;
                }

                if (sesionActiva)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
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
