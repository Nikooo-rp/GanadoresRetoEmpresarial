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
        // -----------------------------------------------------------------------------------------
        public void ModificarCosto(Habitacion h, double nuevoCosto)
        {
            h.SetPrecioNoche(nuevoCosto);
            Console.WriteLine($"Costo de la habitación {h.numero} modificado a {nuevoCosto}");
        }

        // Recibe lista de todas las facturas y pide un periodo de tiemp. Genera un reporte de ingresos totales por mes dentro del periodo, indicando el mes con mayores ingresos y el mes con menores ingresos.
        public void GenerarReporte(List<Facturacion> allf)
        {
            // Requiere al usuario un periodo de tiempo y se asegura de que sea válido. Inicia el valor de ingresos totales en 0.
            DateTime fechaInicio = AskDate("Ingresa el inicio del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            DateTime fechaFin = AskDate("Ingresa el fin del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            if (isValidPeriod(fechaInicio, fechaFin))
            {
                Console.WriteLine("Periodo de tiempo válido. Generando reporte...");
            }
            else
            {
                Console.WriteLine("Periodo de tiempo no válido. El inicio no puede ser después del fin.");
                return;
            }

            decimal ingresosTotales = 0;

            // Para partir en meses, tomamos el mes de la primera y agrupamos todas las facturas que compartan ese mes en una nueva lista,
            // Al encontrarnos con una factura con un nuevo mes, la guardamos, terminamos de recorrer la lista y luego volvemos a buscar
            // facturas que compartan mes con la nueva, agrupándolos en una nueva lista.
            List<Facturacion> fInRange = allf.Where(f => f.fechaFacturacion >= fechaInicio && f.fechaFacturacion <= fechaFin).ToList();
            Dictionary<int, List<Facturacion>> facturasPorMes = fInRange.GroupBy(f => f.fechaFacturacion.Month).ToDictionary(g => g.Key, g => g.ToList());

            // Inicializamos pointers para el mes con mayores ingresos y el mes con menores ingresos, y sus respectivos ingresos.
            // Pico empieza en 0 para que cualquier mes con ingresos mayores a 0 lo actualice, y valle empieza en el valor máximo posible para que cualquier mes con ingresos menores a ese valor lo actualice.
            int mesPico = 0;
            int mesValle = int.MaxValue;
            decimal ingresosPico = 0;
            decimal ingresosValle = decimal.MaxValue;

            // Luego recorremos cada mes calculando los ingresos totales de ese mes.

            foreach (KeyValuePair<int, List<Facturacion>> entry in facturasPorMes)
            {
                Console.WriteLine($"Ingresos de Mes #{entry.Key}");

                //Calcula ingresos del mes.
                decimal d = CalcularIngresos(entry.Value);
                Console.WriteLine("$" + d);
                ingresosTotales += d;

                // A medida que aparecen meses nuevos, vamos comparando sus ingresos con los ingresos del mes pico y del mes valle, actualizando los pointers si es necesario.
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

        // Hace la sumatoria de los costos totales de cada factura en la lista recibida, y devuelve el resultado.
        public decimal CalcularIngresos(List<Facturacion> f)
        {
            decimal ingresosTotales = 0;
            foreach (Facturacion factura in f)
            {
                // casting a decimal para evitar error de tipo.
                ingresosTotales += (decimal)factura.costoTotal;
            }
            return ingresosTotales;
        }

        //Método para administrar promociones.
        public void GestionarPromociones(List<Promocion> promociones)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("------------------------------------------------------------------------------------------");
                Console.WriteLine("GESTIÓN DE PROMOCIONES");
                // Muestra la lista de promociones o indica que no hay ninguna según los contenidos de promociones.
                if (promociones.Count == 0)
                {
                    Console.WriteLine("No hay promociones disponibles.");
                }
                else
                {
                    Console.WriteLine("Lista de promociones actuales:");
                    for (int i = 0; i < promociones.Count; i++)
                    {
                        // Importante notar que al índice se le suma uno con tal de que la lista en consola empiece en 1 en vez de 0.
                        Console.WriteLine($"{i + 1}. {promociones[i].nombre} - {promociones[i].descripcion} - Descuento: {promociones[i].Descuento}% - Validez: {promociones[i].periodoValidez.Item1.ToShortDateString()} a {promociones[i].periodoValidez.Item2.ToShortDateString()}");
                    }
                }
                Console.WriteLine("------------------------------------------------------------------------------------------");

                // Menu de gestión de promociones.
                int opcion = AskInt("1. Agregar nueva promoción \n 2. Modificar promoción existente \n 3. Eliminar promoción \n 0. Salir");
                switch (opcion)
                {
                    case 1:
                        Promocion? nuevaPromocion = NuevaPromocion();
                        if (nuevaPromocion != null)
                        {
                            promociones.Add(nuevaPromocion);
                            Console.WriteLine("Promoción agregada exitosamente.");
                        }
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.WriteLine();
                        int numPromocion = AskInt("Ingresa el número de la promoción a modificar:");
                        // Como el usuario elige basado en la lista de base 1, se le resta uno a su input para evitar errores de índice fuera de rango.
                        ModificarPromocion(promociones[numPromocion - 1]);
                        Console.WriteLine("Promoción modificada exitosamente.");
                        Console.ReadLine();
                        break;
                    case 3:
                        int numPromocionEliminar = AskInt("Ingresa el número de la promoción a eliminar:");
                        // Aplica lo mismo que en el caso anterior. RemoveAt elimina el elemento en el índice dado en base 0.
                        promociones.RemoveAt(numPromocionEliminar - 1);
                        Console.WriteLine("Promoción eliminada exitosamente.");
                        Console.ReadLine();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Por favor, selecciona una opción del 0 al 3.");
                        Console.ReadLine();
                        break;
                }
            }

        }

        //Método privado encargado de crear una nueva promoción
        private Promocion? NuevaPromocion()
        {
            // Obtenemos datos.
            string nombre = AskString("Nombre de la promoción:");
            string descripcion = AskString("Descripción de la promoción:");
            decimal descuento = (decimal)AskInt("Descuento de la promoción (sin el %):");
            DateTime fechaInicio = AskDate("Fecha de inicio de la promoción (dd/mm/yyyy):");
            DateTime fechaFin = AskDate("Fecha de fin de la promoción (dd/mm/yyyy):");

            // Validamos el periodo de validez.
            if (!isValidPeriod(fechaInicio, fechaFin))
            {
                Console.WriteLine("Periodo de validez no válido. El inicio no puede ser después del fin.");
                return null;
            }

            // Intentamos crear la promoción, y si ocurre un error de validación en el constructor, lo atrapamos e imprimimos el mensaje de error.
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
            // Menú para elegir qué modificar.
            string rta = AskString("1. Modificar nombre \n 2. Modificar descripción \n 3. Modificar descuento \n 4. Modificar periodo de validez \n 0. Salir");
            switch (rta)
            {
                case "0":
                    return;
                case "1":
                    p.nombre = AskString("Nuevo nombre:");
                    Console.WriteLine("Nombre modificado exitosamente.");
                    break;
                case "2":
                    p.descripcion = AskString("Nueva descripción:");
                    Console.WriteLine("Descripción modificada exitosamente.");
                    break;
                case "3":
                    p.Descuento = (decimal)AskInt("Nuevo descuento (sin el %):");
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
                default:
                    Console.WriteLine("Opción no válida. Por favor, selecciona una opción del 0 al 4.");
                    break;
            }
        }
        // Métodos para pedir datos al usuario con validación.
        // AskDate pide al usuario una fecha con el formato yyyy-MM-dd, y se asegura de que el formato sea correcto. Si el formato no es correcto, le indica al usuario y vuelve a pedir la fecha.
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

        // AskInt pide al usuario un número entero y se asegura de que el valor ingresado sea válido. Si no es válido, le indica al usuario y vuelve a pedir el número.
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
        // AskString pide al usuario un texto y se asegura de que no esté vacío. Si el usuario ingresa un texto vacío, le indica que no puede estar vacío y vuelve a pedir el texto.
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

        // Valida si el tiempo de inicio es anterior o igual al tiempo de fin. Retorna true si el periodo es válido, y false si no lo es.
        private bool isValidPeriod(DateTime start, DateTime end)
        {
            //Retorna true si la fecha inicio es menor o igual a la fecha fin.
            return start <= end; // Puede ser un solo día.
        }
    
    }
}
