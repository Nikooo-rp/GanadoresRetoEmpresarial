using System;
using System.Collections.Generic;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Reserva
    {
        // Campos privados
        private DateTime fechaEntrada;
        private DateTime fechaSalida;
        private int numeroNoches;
        private string estadoR;
        private double costoTotal;

        // Constructor
        public Reserva(DateTime fechaEntrada, DateTime fechaSalida, double costoPorNoche)
        {
            this.fechaEntrada = fechaEntrada;
            this.fechaSalida = fechaSalida;

            // Calcular número de noches
            TimeSpan diferencia = fechaSalida - fechaEntrada;
            this.numeroNoches = (int)diferencia.TotalDays;

            // Calcular costo total
            this.costoTotal = this.numeroNoches * costoPorNoche;

            // Estado inicial
            this.estadoR = "Pendiente";
        }

        // Métodos getters
        public DateTime GetFechaEntrada()
        {
            return this.fechaEntrada;
        }

        public DateTime GetFechaSalida()
        {
            return this.fechaSalida;
        }

        public int GetNumeroNoches()
        {
            return this.numeroNoches;
        }

        public string GetEstadoR()
        {
            return this.estadoR;
        }

        public double GetCostoTotal()
        {
            return this.costoTotal;
        }

        // Métodos setters
        public void SetFechaEntrada(DateTime fechaEntrada)
        {
            this.fechaEntrada = fechaEntrada;
        }

        public void SetFechaSalida(DateTime fechaSalida)
        {
            this.fechaSalida = fechaSalida;
        }

        // Método para modificar el estado
        public void ModificarEstado(string nuevoEstado)
        {
            // Validar que el estado sea uno de los permitidos
            string[] estadosPermitidos = { "Pendiente", "Confirmada", "Cancelada", "Completada" };

            if (Array.Exists(estadosPermitidos, estado => estado == nuevoEstado))
            {
                this.estadoR = nuevoEstado;
            }
            else
            {
                throw new ArgumentException("Estado no válido. Los estados permitidos son: Pendiente, Confirmada, Cancelada, Completada");
            }
        }

        // Método para actualizar fechas y recalcular noches y costo
        public void ActualizarFechas(DateTime nuevaFechaEntrada, DateTime nuevaFechaSalida, double costoPorNoche)
        {
            if (nuevaFechaSalida > nuevaFechaEntrada)
            {
                this.fechaEntrada = nuevaFechaEntrada;
                this.fechaSalida = nuevaFechaSalida;

                TimeSpan diferencia = this.fechaSalida - this.fechaEntrada;
                this.numeroNoches = (int)diferencia.TotalDays;
                this.costoTotal = this.numeroNoches * costoPorNoche;
            }
            else
            {
                throw new ArgumentException("La fecha de salida debe ser posterior a la fecha de entrada");
            }
        }

        // Método para recalcular costo (por si cambia el precio por noche)
        public void RecalcularCosto(double costoPorNoche)
        {
            this.costoTotal = this.numeroNoches * costoPorNoche;
        }

        // Método para mostrar información de la reserva
        public void MostrarInformacion()
        {
            Console.WriteLine($"Fecha Entrada: {this.fechaEntrada.ToShortDateString()}");
            Console.WriteLine($"Fecha Salida: {this.fechaSalida.ToShortDateString()}");
            Console.WriteLine($"Número de Noches: {this.numeroNoches}");
            Console.WriteLine($"Estado: {this.estadoR}");
            Console.WriteLine($"Costo Total: {this.costoTotal:C}");
        }
    }
}