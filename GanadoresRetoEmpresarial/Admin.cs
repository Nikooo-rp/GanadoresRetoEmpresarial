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
            // Check si hay facturas en la lista para evitar errores.
            if (allf.Count == 0)
            {
                Console.WriteLine("No hay facturas registradas para generar un reporte.");
                Console.ReadLine();
                return;
            }
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

            // Check de si hay facturas en el periodo de tiempo seleccionado para evitar errores.
            if (fInRange.Count == 0)
            {
                Console.WriteLine("No hay facturas dentro del periodo de tiempo seleccionado para generar un reporte.");
                Console.ReadLine();
                return;
            }

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
            string mesPicoNombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mesPico);
            string mesValleNombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mesValle);

            string reporte = "";

            reporte += "-----------------------------REPORTE DE INGRESOS-----------------------------\n";
            reporte += $"Reporte generado el {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n";
            reporte += "------------------------------------------------------------------------------------------\n";
            reporte += $"Ingresos totales dentro del periodo:{fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}\n";
            reporte += $"{ingresosTotales}\n";

            // si son iguales, quiere decir que solo hay un mes. En ese caso, no tiene sentido mostrar un mes pico y valle, ya que serían el mismo mes, por lo que solo se muestra el mes pico (que es el mismo que el valle).
            if (mesPico != mesValle)
            {
                reporte += "------------------------------------------------------------------------------------------\n";
                reporte += $"Mes con mayores ingresos: {mesPicoNombre} (${ingresosPico})\n";
                reporte += $"Mes con menores ingresos: {mesValleNombre} (${ingresosValle})\n";
                reporte += "------------------------------------------------------------------------------------------\n";
            }

            Console.WriteLine(reporte);
            string respuesta = AskTypes.AskString("Deseas imprimir el reporte? (s/n)");
            if (respuesta.ToLower() == "s")
            {
                ImprimirReporte(reporte, fechaInicio, fechaFin);
            }
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
        // Imprimir reporte recibe un string con el reporte formateado, luego crea una carpeta llamada "Reportes" dentro de la carpeta principal del programa (si no existe) y guarda el reporte en un archivo de texto con un nombre que incluye un timestamp para evitar sobreescrituras.
        private void ImprimirReporte(string reporte, DateTime inicio, DateTime fin)
        {
            string pathReportes = Path.Combine(Program.fullPath, "Reportes");
            Directory.CreateDirectory(pathReportes);

            // Lo ponemos dentro de un try catch para manejar cualquier error que pueda surgir al escribir el archivo, como problemas de permisos o espacio en disco insuficiente.
            try
            {
                string inicioString = inicio.ToString("yyyy-MM-dd");
                string finString = fin.ToString("yyyy-MM-dd");
                StreamWriter sw = new StreamWriter(Path.Combine(pathReportes, $"reporte_{inicioString}_a_{finString}.txt"));
                sw.WriteLine(reporte);
                sw.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al imprimir el reporte: {ex.Message}");
            }
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
                int opcion = AskTypes.AskInt(" 1. Agregar nueva promoción \n 2. Modificar promoción existente \n 3. Eliminar promoción \n 0. Salir");
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
            DateTime fechaInicio = AskTypes.AskDate("Fecha de inicio de la promoción (yyyy/mm/dd):");
            DateTime fechaFin = AskTypes.AskDate("Fecha de fin de la promoción (yyyy/mm/dd):");
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
            string rta = AskTypes.AskString(" 1. Modificar nombre \n 2. Modificar descripción \n 3. Modificar descuento \n 4. Modificar periodo de validez \n 0. Volver");
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

                    fechaInicio = AskTypes.AskDate("Nueva fecha de inicio (yyyy/mm/dd):");
                    fechaFin = AskTypes.AskDate("Nueva fecha de fin (yyyy/mm/dd):");

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
