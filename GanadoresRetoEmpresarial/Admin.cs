using System.Data;

namespace GanadoresRetoEmpresarial
{
    public class Admin : Usuario
    {
        public Admin(string nombre, string contraseña) : base(nombre, contraseña)
        {
            this.nombre = nombre;
            this.contraseña = contraseña;
        }

        //Método para modificar el costo de una habitación.
        public void ModificarCosto(Habitacion h, double nuevoCosto)
        {
            h.SetPrecioNoche(nuevoCosto);
            Console.WriteLine($"Costo de la habitación {h.numero} modificado a {nuevoCosto}");
        }
        
        //Genera un reporte financiero entre dos fechas.
        public void GenerarReporte(List<Facturacion> allf)
        {

            //Solicita fechas al usuario
            DateTime fechaInicio = AskDate("Ingresa el inicio del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            DateTime fechaFin = AskDate("Ingresa el fin del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            decimal ingresosTotales = 0;

            // Para partir en meses, tomamos el mes de la primera y agrupamos todas las facturas que compartan ese mes en una nueva lista,
            // Al encontrarnos con una factura con un nuevo mes, la guardamos, terminamos de recorrer la lista y luego volvemos a buscar
            // facturas qeu compartan mes con la nueva, agrupándolos en una nueva lista.
            List<Facturacion> fInRange = allf.Where(f => f.fechaFacturacion >= fechaInicio && f.fechaFacturacion <= fechaFin).ToList();
            Dictionary<int, List<Facturacion>> facturasPorMes = fInRange.GroupBy(f => f.fechaFacturacion.Month).ToDictionary(g => g.Key, g => g.ToList());

            //Variables para identificar el mejor y peor mes.
            int mesPico = 0;
            int mesValle = int.MaxValue;
            decimal ingresosPico = 0;
            decimal ingresosValle = decimal.MaxValue;

            //Recorre cada grupo de facturas.
            foreach (KeyValuePair<int, List<Facturacion>> entry in facturasPorMes)
            {
                Console.WriteLine($"Ingresos de Mes #{entry.Key}");

                //Calcula ingresos del mes.
                decimal d = CalcularIngresos(entry.Value);
                Console.WriteLine("$" + d);
                ingresosTotales += d;

                //Determina el mes con mayores ingresos.
                if (d > ingresosPico)
                {
                    mesPico = entry.Key;
                    ingresosPico = d;
                }

                //Determina el mes con menores ingresos.
                if (d < ingresosValle)
                {
                    mesValle = entry.Key;
                    ingresosValle = d;
                }
            }

            //Resumen final
            Console.WriteLine($"Mes con mayores ingresos: {mesPico} (${ingresosPico})");
            Console.WriteLine($"Mes con menores ingresos: {mesValle} (${ingresosValle})");
            Console.WriteLine($"Ingresos totales dentro del periodo:{fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()}");
            Console.WriteLine(ingresosTotales);
        }

        //Calcula los ingresos totales de una lista de facturas.
        public decimal CalcularIngresos(List<Facturacion> f)
        {
            decimal ingresosTotales = 0;
            foreach (Facturacion factura in f)
            {
                ingresosTotales += (decimal)factura.costoTotal;
            }
            return ingresosTotales;
        }

        //Método para administrar promociones.
        public void GestionarPromociones(List<Promocion> promociones)
        {

            //Verifica si existen promociones.
            if (promociones.Count == 0)
            {
                Console.WriteLine("No hay promociones disponibles.");
            }
            else
            {
                Console.WriteLine("Lista de promociones actuales:");
                
                //Muestra promociones existentes.
                for (int i = 0; i < promociones.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {promociones[i].nombre} - {promociones[i].descripcion} - Descuento: {promociones[i].descuento}% - Validez: {promociones[i].periodoValidez.Item1.ToShortDateString()} a {promociones[i].periodoValidez.Item2.ToShortDateString()}");
                }
            }
            Console.WriteLine("------------------------------------------------------------------------------------------");

            //Solicita una opción al administrador.
            int opcion = AskInt("1. Agregar nueva promoción \n 2. Modificar promoción existente \n 3. Eliminar promoción \n 4. Salir");
            switch (opcion)
            {
                case 1:
                    
                    //Crea una nueva promoción.
                    Promocion? nuevaPromocion = NuevaPromocion();
                    if (nuevaPromocion != null)
                    {
                        promociones.Add(nuevaPromocion);
                        Console.WriteLine("Promoción agregada exitosamente.");
                    }
                    break;
                case 2:

                    //Modifica promoción existente.
                    Console.WriteLine();
                    int numPromocion = AskInt("Ingresa el número de la promoción a modificar:");
                    ModificarPromocion(promociones[numPromocion - 1]);
                    Console.WriteLine("Promoción modificada exitosamente.");
                    break;
                case 3:

                    //Elimina una promoción.
                    int numPromocionEliminar = AskInt("Ingresa el número de la promoción a eliminar:");
                    promociones.RemoveAt(numPromocionEliminar - 1);
                    Console.WriteLine("Promoción eliminada exitosamente.");
                    break;
                case 4:
                    return;
                default:
                    Console.WriteLine("Opción no válida. Por favor, selecciona una opción del 1 al 4.");
                    break;
            }

        }

        //Método privado encargado de crear una nueva promoción
        private Promocion? NuevaPromocion()
        {
            //Solicita toda la información necesaria para crear la promoción.
            string nombre = AskString("Nombre de la promoción:");
            string descripcion = AskString("Descripción de la promoción:");
            decimal descuento = (decimal)AskInt("Descuento de la promoción (sin el %):");
            DateTime fechaInicio = AskDate("Fecha de inicio de la promoción (dd/mm/yyyy):");
            DateTime fechaFin = AskDate("Fecha de fin de la promoción (dd/mm/yyyy):");
            try
            {
                //Intenta crear la nueva promoción.
                Promocion nuevaPromocion = new Promocion(nombre, descripcion, descuento, fechaInicio, fechaFin);
                return nuevaPromocion;

            }
            catch (ArgumentException ex)
            {
                //Captura errores de validación y muestra el mensaje correspondiente.
                Console.WriteLine($"Error al crear la promoción: {ex.Message}");
                return null;
            }
        }
        
        //Método privado para modificar una promoción existente.
        private void ModificarPromocion(Promocion p)
        {
            //Solicita al administrador qué desea modificar.
            string rta = AskString("1. Modificar nombre \n 2. Modificar descripción \n 3. Modificar descuento \n 4. Modificar periodo de validez");
            switch (rta)
            {
                case "1":
                    p.nombre = AskString("Nuevo nombre:");
                    Console.WriteLine("Nombre modificado exitosamente.");
                    break;
                case "2":
                    p.descripcion = AskString("Nueva descripción:");
                    Console.WriteLine("Descripción modificada exitosamente.");
                    break;
                case "3":
                    p.descuento = (decimal)AskInt("Nuevo descuento (sin el %):");
                    Console.WriteLine("Descuento modificado exitosamente.");
                    break;
                case "4":
                    bool validDate = false;
                    DateTime fechaInicio;
                    DateTime fechaFin;
                    while (!validDate)
                    {
                        fechaInicio = AskDate("Nueva fecha de inicio (dd/mm/yyyy):");
                        fechaFin = AskDate("Nueva fecha de fin (dd/mm/yyyy):");

                        //Verifica que la fecha inicio no sea mayor que la fecha fin.
                        validDate = isValidPeriod(fechaInicio, fechaFin);
                        if (!validDate)
                            //Mensaje de error si el periodo es inválido.
                            Console.WriteLine("Ingresa un periodo válido: el inicio no puede ser después de la fecha fin");
                        else
                        {
                            //Actualiza el periodo de validez.
                            p.periodoValidez = (fechaInicio, fechaFin);
                            Console.WriteLine("Periodo de validez modificado exitosamente.");
                        }
                    }
                    break;
            }
        }

        //Método estático para solicitar una fecha válida.
        public static DateTime AskDate(string prompt)
        {
            DateTime value;
            do
            {
                //Muestra el mensaje recibido.
                Console.Write(prompt);

                //Intenta convertir el texto ingresado a DateTime.
                if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                    null, System.Globalization.DateTimeStyles.None, out value))

                    //Mensaje de error si el formato es incorrecto.
                    Console.WriteLine("  ⚠ Usa el formato yyyy-MM-dd.");
            } while (value == default);

            //Retorna la fecha válida.
            return value;
        }

        //Método estático para solicitar un número entero válido.
        public static int AskInt(string prompt)
        {
            int value;

            //Muestra el mensaje recibido.
            Console.Write(prompt);

            //Repite hasta ingresar un entero válido.
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.WriteLine("  ⚠ Debe ser un número.");
                Console.Write(prompt);
            }
            return value;
        }

        //Método estático para solicitar un texto no vacío.
        public static string AskString(string prompt)
        {
            string? value;
            do
            {

                //Muestra el mensaje.
                Console.Write(prompt);

                //Lee el texto y elimina espacios al inicio y final.
                value = Console.ReadLine()?.Trim();

                //Verifica si está vacío.
                if (string.IsNullOrEmpty(value))
                    Console.WriteLine("  ⚠ Can't be empty, try again.");
            } while (string.IsNullOrEmpty(value));

            //Retorna el texto válido.
            return value;
        }

        //Método privado que valida si un periodo de fechas es correcto.
        private bool isValidPeriod(DateTime start, DateTime end)
        {
            //Retorna true si la fecha inicio es menor o igual a la fecha fin.
            return start <= end; // Puede ser un solo día.
        }
    
    }
}
