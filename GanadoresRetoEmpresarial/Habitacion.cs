using System;
using System.Collections.Generic;
using System.Text;

public enum TipoHabitacion
{
    Sencilla,
    Doble,
    Matrimonial
}

public enum EstadoHabitacion
{
    Disponible,
    Ocupada,
    EnMantenimiento
}

namespace GanadoresRetoEmpresarial
{    
    public class Habitacion
    {
        public int numero { get; private set; }
        
        public double precioNoche { get; private set; }

        public TipoHabitacion tipo { get; private set; }

        public EstadoHabitacion estadoH { get; private set; }

        public Habitacion(int numero, double precioNoche, TipoHabitacion tipo)
        {
            if (precioNoche < 0) throw new ArgumentException("El precio no puede ser negativo.");

            this.numero = numero;
            this.tipo = tipo;
            this.precioNoche = precioNoche;
            this.estadoH = EstadoHabitacion.Disponible;
        }

        public double ConsultarPrecio(int noches)
        {
            Console.WriteLine("El precio por noche de la habitación es: " + precioNoche);
            return precioNoche * noches;
        }

        public void GetEstado(EstadoHabitacion nuevoEstado)
        {
            if (estadoH == EstadoHabitacion.EnMantenimiento && nuevoEstado == EstadoHabitacion.Ocupada)
            {
                throw new InvalidOperationException("No se puede ocupar una habitación en mantenimiento.");
            }

            estadoH = nuevoEstado;
        }

        public bool EstaDisponible()
        {
            return estadoH == EstadoHabitacion.Disponible;
        }

        public void SetPrecioNoche(double nuevoPrecio)
        {
            if (nuevoPrecio < 0) throw new ArgumentException("El precio no puede ser negativo.");
            precioNoche = nuevoPrecio;
        }

        public override string ToString()
        {
            return $"Habitación #{numero} - Tipo: {tipo}, Precio por noche: ${precioNoche}, Estado: {estadoH}";
        }
    }
}
