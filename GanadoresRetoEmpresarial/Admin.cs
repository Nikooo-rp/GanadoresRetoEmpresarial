using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GanadoresRetoEmpresarial
{
    public class Admin : Usuario
    {
        string idAdmin = string.Empty;

        public Admin(string idAdmin, string nombre, string contraseña) : base(nombre, contraseña)
        {
            this.idAdmin = idAdmin;
            this.nombre = nombre;
            this.contraseña = contraseña;
        }
        // -----------------------------------------------------------------------------------------
        public void ModificarCosto(Habitacion h, int nuevoCosto)
        {
            h.SetPrecioNoche(nuevoCosto);
            Console.WriteLine($"Costo de la habitación {h.numero} modificado a {nuevoCosto}");
        }
        public void GenerarReporte(List<Facturacion> allf)
        {
            DateTime fechaInicio = AskDate("Ingresa el inicio del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            DateTime fechaFin = AskDate("Ingresa el fin del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            decimal ingresosTotales = 0;

            // Para partir en meses, tomamos el mes de la primera y agrupamos todas las facturas que compartan ese mes en una nueva lista,
            // Al encontrarnos con una factura con un nuevo mes, la guardamos, terminamos de recorrer la lista y luego volvemos a buscar
            // facturas qeu compartan mes con la nueva, agrupándolos en una nueva lista.
            List<Facturacion> fInRange = allf.Where(f => f.fechaFacturacion >= fechaInicio && f.fechaFacturacion <= fechaFin).ToList();
            Dictionary<int, List<Facturacion>> facturasPorMes = fInRange.GroupBy(f => f.fechaFacturacion.Month).ToDictionary(g => g.Key, g => g.ToList());

            int mesPico = 0;
            int mesValle = int.MaxValue;
            decimal ingresosPico = 0;
            decimal ingresosValle = decimal.MaxValue;

            foreach (KeyValuePair<int, List<Facturacion>> entry in facturasPorMes)
            {
                Console.WriteLine($"Ingresos de Mes #{entry.Key}");
                decimal d = CalcularIngresos(entry.Value);
                Console.WriteLine("$" + d);
                ingresosTotales += d;

                if (d > ingresosPico)
                {
                    mesPico = entry.Key;
                    ingresosPico = d;
                }
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

        public decimal CalcularIngresos(List<Facturacion> f)
        {
            decimal ingresosTotales = 0;
            foreach (Facturacion factura in f)
            {
                ingresosTotales += (decimal)factura.costoTotal;
            }
            return ingresosTotales;
        }
        public void GestionarPromociones(List<Promocion> promociones)
        {

            if (promociones.Count == 0)
            {
                Console.WriteLine("No hay promociones disponibles.");
            }
            else
            {
                Console.WriteLine("Lista de promociones actuales:");
                for (int i = 0; i < promociones.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {promociones[i].nombre} - {promociones[i].descripcion} - Descuento: {promociones[i].descuento}% - Validez: {promociones[i].periodoValidez.Item1.ToShortDateString()} a {promociones[i].periodoValidez.Item2.ToShortDateString()}");
                }
            }
            Console.WriteLine("------------------------------------------------------------------------------------------");
            int opcion = AskInt("1. Agregar nueva promoción \n 2. Modificar promoción existente \n 3. Eliminar promoción \n 4. Salir");
            switch (opcion)
            {
                case 1:
                    Promocion? nuevaPromocion = NuevaPromocion();
                    if (nuevaPromocion != null)
                    {
                        promociones.Add(nuevaPromocion);
                        Console.WriteLine("Promoción agregada exitosamente.");
                    }
                    break;
                case 2:
                    Console.WriteLine();
                    int numPromocion = AskInt("Ingresa el número de la promoción a modificar:");
                    ModificarPromocion(promociones[numPromocion - 1]);
                    Console.WriteLine("Promoción modificada exitosamente.");
                    break;
                case 3:
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
        private Promocion? NuevaPromocion()
        {
            string nombre = AskString("Nombre de la promoción:");
            string descripcion = AskString("Descripción de la promoción:");
            decimal descuento = (decimal)AskInt("Descuento de la promoción (sin el %):");
            DateTime fechaInicio = AskDate("Fecha de inicio de la promoción (dd/mm/yyyy):");
            DateTime fechaFin = AskDate("Fecha de fin de la promoción (dd/mm/yyyy):");
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
        private void ModificarPromocion(Promocion p)
        {
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

                        validDate = isValidPeriod(fechaInicio, fechaFin);
                        if (!validDate)
                            Console.WriteLine("Ingresa un periodo válido: el inicio no puede ser después de la fecha fin");
                        else
                        {
                            p.periodoValidez = (fechaInicio, fechaFin);
                            Console.WriteLine("Periodo de validez modificado exitosamente.");
                        }
                    }
                    break;
            }

        }

        private DateTime AskDate(string prompt)
        {
            DateTime value;
            do
            {
                Console.Write(prompt);
                if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                    null, System.Globalization.DateTimeStyles.None, out value))
                    Console.WriteLine("  ⚠ Usa el formato yyyy-MM-dd.");
            } while (value == default);
            return value;
        }
        private int AskInt(string prompt)
        {
            int value;
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.WriteLine("  ⚠ Debe ser un número.");
                Console.Write(prompt);
            }
            return value;
        }
        private string AskString(string prompt)
        {
            string? value;
            do
            {
                Console.Write(prompt);
                value = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(value))
                    Console.WriteLine("  ⚠ Can't be empty, try again.");
            } while (string.IsNullOrEmpty(value));
            return value;
        }
        private bool isValidPeriod(DateTime start, DateTime end)
        {
            return start <= end; // Puede ser un solo día.
        }
    }
}
