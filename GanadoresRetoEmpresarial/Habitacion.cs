using System;
using System.Collections.Generic;
using System.Text;

public enum TipoHabitacion
{
    Sencilla,
    Doble
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
        public int numero;

        public double precioNoche;

        public TipoHabitacion tipo;

        public EstadoHabitacion estadoH;

        //Constructor de la clase Habitación
        public Habitacion(int numero, double precioNoche, TipoHabitacion tipo)
        {
            if (precioNoche < 0) throw new ArgumentException("El precio no puede ser negativo.");

            this.numero = numero;
            this.tipo = tipo;
            this.precioNoche = precioNoche;

            //toda habitación inicia disponible
            this.estadoH = EstadoHabitacion.Disponible;
        }

        //Metodo para calcular el costo total según la cantidad de noches.
        public double ConsultarPrecio(int noches)
        {
            Console.WriteLine("El precio por noche de la habitación es: " + precioNoche);
            return precioNoche * noches;
        }

        //Método para cambiar el estado de la habitación.
        public void GetEstado(EstadoHabitacion nuevoEstado)
        {
            //No es válido ocupar una habitación en mantenimiento.
            if (estadoH == EstadoHabitacion.EnMantenimiento && nuevoEstado == EstadoHabitacion.Ocupada)
            {
                throw new InvalidOperationException("No se puede ocupar una habitación en mantenimiento.");
            }

            estadoH = nuevoEstado;
        }

        //Devuelve true si la habitación está disponible.
        public bool EstaDisponible()
        {
            return estadoH == EstadoHabitacion.Disponible;
        }

        //Método para modificar el precio de la habitación
        public void SetPrecioNoche(double nuevoPrecio)
        {
            if (nuevoPrecio < 0) throw new ArgumentException("El precio no puede ser negativo.");
            precioNoche = nuevoPrecio;
        }

        //Sobreescritura del método ToString para mostrar información legible.
        public override string ToString()
        {
            return $"Habitación #{numero} - Tipo: {tipo}, Precio por noche: ${precioNoche}, Estado: {estadoH}";
        }
    }
}
