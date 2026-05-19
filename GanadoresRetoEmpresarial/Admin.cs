using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Admin : Usuario
    {

        public Admin(string nombre, string contraseña) : base(nombre, contraseña)
        {
            this.nombre = nombre;
            this.contraseña = contraseña;
        }
        // -----------------------------------------------------------------------------------------
        // Recibe una habitación y un valor para el costo nuevo, luego lo asigna a la habitación.
        public void ModificarCosto(Habitacion h, double nuevoCosto)
        {
            h.SetPrecioNoche(nuevoCosto);
            Console.WriteLine($"Costo de la habitación {h.numero} modificado a {nuevoCosto}");
        }
        public void GenerarReporte(List<Facturacion> allf)
        {
            // El siguiente bloque pide al usuario una fecha de inicio y fin para el reporte hasta que ingrese un periodo válido (inicio antes o el mismo día que el final).
            DateTime fechaInicio;
            DateTime fechaFin;
            do
            {
                fechaInicio = AskTypes.AskDate("Ingresa el inicio del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
                fechaFin = AskTypes.AskDate("Ingresa el fin del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            } while (!isValidPeriod(fechaInicio, fechaFin));
            // Variable para acumular los ingresos totales del periodo.
            decimal ingresosTotales = 0;

            // Primero tomamos todas las facturas dentro del periodo de tiempo establecido por el usuario, filtrando la lista de facturas completa.
            List<Facturacion> fInRange = allf.Where(f => f.fechaFacturacion >= fechaInicio && f.fechaFacturacion <= fechaFin).ToList();

            //Luego, agrupamos las facturas por mes usando GroupBy y ToDictionary para crear un diccionario donde la clave es el número del mes y el valor es la lista de facturas de ese mes.

            // Para partir en meses, tomamos el mes de la primera factura y agrupamos todas las facturas que compartan ese mes en una nueva lista,
            // Al encontrarnos con una factura con un nuevo mes, la guardamos, terminamos de recorrer la lista y luego volvemos a buscar
            // facturas que compartan mes con la nueva, agrupándolos en una nueva lista.
            Dictionary<int, List<Facturacion>> facturasPorMes = fInRange.GroupBy(f => f.fechaFacturacion.Month).ToDictionary(g => g.Key, g => g.ToList());

            int mesPico = 0;
            int mesValle = int.MaxValue;
            decimal ingresosPico = 0;
            decimal ingresosValle = decimal.MaxValue;

            // Recorremos cada pareja del diccionario, calculamos los ingresos del mes usando la función CalcularIngresos y los vamos sumando a los ingresos totales.
            foreach (KeyValuePair<int, List<Facturacion>> entry in facturasPorMes)
            {
                // Para cada mes, imprimimos el nombre del mes usando CultureInfo para obtener el nombre del mes a partir del número.
                Console.WriteLine($"Ingresos de Mes {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(entry.Key)}");
                decimal d = CalcularIngresos(entry.Value);
                Console.WriteLine("$" + d);
                ingresosTotales += d;

                // Si los ingresos del mes actual son mayores que los ingresos pico registrados hasta ahora, actualizamos el mes pico y los ingresos pico.
                if (d > ingresosPico)
                {
                    mesPico = entry.Key;
                    ingresosPico = d;
                }
                // Si los ingresos del mes actual son menores que los ingresos valle registrados hasta ahora, actualizamos el mes valle y los ingresos valle.
                if (d < ingresosValle)
                {
                    mesValle = entry.Key;
                    ingresosValle = d;
                }
            }
            Console.WriteLine($"Mes con mayores ingresos: {mesPico} (${ingresosPico})");
            Console.WriteLine($"Mes con menores ingresos: {mesValle} (${ingresosValle})");
            Console.WriteLine($"Ingresos totales dentro del periodo:{fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}");
            Console.WriteLine(ingresosTotales);
        }

        // Calcular ingresos recorre la lista entregada de factura y suma el costo total de cada una a un decimal que luego devuelve.
        public decimal CalcularIngresos(List<Facturacion> f)
        {
            decimal ingresosTotales = 0;
            foreach (Facturacion factura in f)
            {
                ingresosTotales += (decimal)factura.costoTotal;
            }
            return ingresosTotales;
        }

        // Submenú de gestión de promociones.
        public void GestionarPromociones(List<Promocion> promociones)
        {
            while (true)
            {
                Console.Clear();
                if (promociones.Count == 0)
                {
                    Console.WriteLine("Actualmente no hay promociones disponibles.");
                }
                else
                {
                    Console.WriteLine("Lista de promociones actuales:");
                    for (int i = 0; i < promociones.Count; i++)
                    {
                        // Imprime la lista de promociones con índice base 1, es decir, empieza en 1 en vez de 0 para que sea más intuitivo para el usuario.
                        Console.WriteLine($"{i + 1}. {promociones[i].nombre} - {promociones[i].descripcion} - Descuento: {promociones[i].Descuento}% - Validez: {promociones[i].periodoValidez.Item1.ToShortDateString()} a {promociones[i].periodoValidez.Item2.ToShortDateString()}");
                    }
                }
                Console.WriteLine("------------------------------------------------------------------------------------------");
                int opcion = AskTypes.AskInt("1. Agregar nueva promoción \n 2. Modificar promoción existente \n 3. Eliminar promoción \n 0. Salir");
                switch (opcion)
                {
                    case 1:
                        // nueva promoción puede ser null (como lo indica el ? después del tipo), ya que se utiliza para determinar si hubo éxito generándola o no.
                        Promocion? nuevaPromocion = NuevaPromocion();
                        if (nuevaPromocion != null)
                        {
                            promociones.Add(nuevaPromocion);
                            Console.WriteLine("Promoción agregada exitosamente.");
                        }
                        Console.ReadLine();
                        break;
                    // Casos 2 y 3 restan 1 al número ingresado por el usuario para obtener el índice correcto. Recordando que anteriormente se le enseñaron los índices ajustados a base 1, pero los necesitamos en base 0 para evitar errores de rango.
                    case 2:
                        Console.WriteLine();
                        int numPromocion = AskTypes.AskInt("Ingresa el número de la promoción a modificar:");
                        if (ModificarPromocion(promociones[numPromocion - 1]))
                        {
                            Console.WriteLine("Promoción modificada exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("Error al modificar la promoción.");
                        }
                        Console.ReadLine();
                        break;
                    case 3:
                        int numPromocionEliminar = AskTypes.AskInt("Ingresa el número de la promoción a eliminar:");
                        try
                        {
                            promociones.RemoveAt(numPromocionEliminar - 1);

                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("Número de promoción no válido.");
                            Console.ReadLine();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al eliminar la promoción: {ex.Message}");
                            Console.ReadLine();
                            break;
                        }

                        // Si no lanza ninguna excepción, se asume que la eliminación fue exitosa y se muestra el mensaje correspondiente.
                        Console.WriteLine("Promoción eliminada exitosamente.");
                        Console.ReadLine();
                        break;
                    case 0:
                        // Sale del menú de gestión de promociones, volviendo al menú principal del admin.
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Por favor, selecciona una opción del 1 al 4.");
                        Console.ReadLine();
                        break;
                }
            }
        }
        private Promocion? NuevaPromocion()
        {
            // Obtención de datos. El método AskString y AskInt se encargan de validar que el usuario ingrese un string no vacío o un número respectivamente, por lo que no es necesario agregar validaciones adicionales para esos casos.
            string nombre = AskTypes.AskString("Nombre de la promoción:");
            string descripcion = AskTypes.AskString("Descripción de la promoción:");
            decimal descuento = (decimal)AskTypes.AskInt("Descuento de la promoción (de 0 a 100 sin el %):");
            DateTime fechaInicio = AskTypes.AskDate("Fecha de inicio de la promoción (dd/mm/yyyy):");
            DateTime fechaFin = AskTypes.AskDate("Fecha de fin de la promoción (dd/mm/yyyy):");
            if (isValidPeriod(fechaInicio, fechaFin))
            {
                try
                {
                    Promocion nuevaPromocion = new Promocion(nombre, descripcion, descuento, fechaInicio, fechaFin);
                    return nuevaPromocion;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error al crear la promoción: {ex.Message}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Periodo de validez incorrecto.");
                return null;
            }
        }
        private bool ModificarPromocion(Promocion p)
        {
            // Retorna verdadero si se logró la operación, falso de lo contrario. Esto se utiliza para mostrar un mensaje de éxito o error al usuario dependiendo del resultado.
            string rta = AskTypes.AskString("1. Modificar nombre \n 2. Modificar descripción \n 3. Modificar descuento \n 4. Modificar periodo de validez \n 0. Volver");
            switch (rta)
            {
                case "1":
                    p.nombre = AskTypes.AskString("Nuevo nombre:");
                    Console.WriteLine("Nombre modificado exitosamente.");
                    return true;
                case "2":
                    p.descripcion = AskTypes.AskString("Nueva descripción:");
                    Console.WriteLine("Descripción modificada exitosamente.");
                    return true;
                case "3":
                    p.Descuento = (decimal)AskTypes.AskInt("Nuevo descuento (de 0 a 100 sin el %):");
                    Console.WriteLine("Descuento modificado exitosamente.");
                    return true;
                case "4":
                    DateTime fechaInicio;
                    DateTime fechaFin;

                    fechaInicio = AskTypes.AskDate("Nueva fecha de inicio (dd/mm/yyyy):");
                    fechaFin = AskTypes.AskDate("Nueva fecha de fin (dd/mm/yyyy):");

                    if (!isValidPeriod(fechaInicio, fechaFin))
                    {
                        Console.WriteLine("Periodo inválido: el inicio no puede ser después de la fecha fin");
                        return false;
                    }
                    else
                    {
                        p.periodoValidez = (fechaInicio, fechaFin);
                        Console.WriteLine("Periodo de validez modificado exitosamente.");
                        return true;
                    }
                case "0":
                    return false;
                default:
                    Console.WriteLine("Ingresa un número válido");
                    return false;
            }
        }
        public static bool isValidPeriod(DateTime start, DateTime end)
        {
            return start <= end; // Puede ser un solo día.
        }
    
    }
}
