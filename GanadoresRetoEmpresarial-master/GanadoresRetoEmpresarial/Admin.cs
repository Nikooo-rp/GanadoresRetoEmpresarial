using System;
using System.Collections.Generic;
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
            h.precioNoche = nuevoCosto;
            Console.WriteLine($"Costo de la habitación {h.numero} modificado a {nuevoCosto}");
        }
        public void GenerarReporte(List<Facturacion> allf)
        {
            Console.WriteLine("Ingresa el inicio del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            string inicio = Console.ReadLine();
            Console.WriteLine("Ingresa el fin del periodo de tiempo para el reporte (formato: yyyy-MM-dd):");
            string fin = Console.ReadLine();

            DateTime fechaInicio = DateTime.Parse(inicio);
            DateTime fechaFin = DateTime.Parse(fin);
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
                ingresosTotales += factura.costoTotal;
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
            Console.WriteLine("1. Agregar nueva promoción \n 2. Modificar promoción existente \n 3. Eliminar promoción \n 4. Salir");
            string rta = Console.ReadLine();
            if (!int.TryParse(rta, out int opcion))
            {
                Console.WriteLine("Opción no válida. Por favor, selecciona una opción del 1 al 4.");
                return;
            }
            switch (opcion)
            {
                case 1:
                    Promocion nuevaPromocion = NuevaPromocion();
                    if (nuevaPromocion != null)
                    {
                        promociones.Add(nuevaPromocion);
                        Console.WriteLine("Promoción agregada exitosamente.");
                    }
                    break;
                case 2:
                    Console.WriteLine("Ingresa el número de la promoción a modificar:");
                    int numPromocion = int.Parse(Console.ReadLine());
                    ModificarPromocion(promociones[numPromocion - 1]);
                    Console.WriteLine("Promoción modificada exitosamente.");
                    break;
                case 3:
                    Console.WriteLine("Ingresa el número de la promoción a eliminar:");
                    int numPromocionEliminar = int.Parse(Console.ReadLine());
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
        private Promocion NuevaPromocion()
        {
            Console.WriteLine("Nombre de la promoción:");
            string nombre = Console.ReadLine();
            Console.WriteLine("Descripción de la promoción:");
            string descripcion = Console.ReadLine();
            Console.WriteLine("Descuento de la promoción (sin el %):");
            decimal descuento = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Fecha de inicio de la promoción (dd/mm/yyyy):");
            DateTime fechaInicio = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Fecha de fin de la promoción (dd/mm/yyyy):");
            DateTime fechaFin = DateTime.Parse(Console.ReadLine());
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
            Console.WriteLine("1. Modificar nombre \n 2. Modificar descripción \n 3. Modificar descuento \n 4. Modificar periodo de validez");
            string rta = Console.ReadLine();
            switch (rta)
            {
                case "1":
                    Console.WriteLine("Nuevo nombre:");
                    p.nombre = Console.ReadLine();
                    Console.WriteLine("Nombre modificado exitosamente.");
                    break;
                case "2":
                    Console.WriteLine("Nueva descripción:");
                    p.descripcion = Console.ReadLine();
                    Console.WriteLine("Descripción modificada exitosamente.");
                    break;
                case "3":
                    Console.WriteLine("Nuevo descuento (sin el %):");
                    p.descuento = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Descuento modificado exitosamente.");
                    break;
                case "4":
                    Console.WriteLine("Nueva fecha de inicio (dd/mm/yyyy):");
                    DateTime fechaInicio = DateTime.Parse(Console.ReadLine());
                    Console.WriteLine("Nueva fecha de fin (dd/mm/yyyy):");
                    DateTime fechaFin = DateTime.Parse(Console.ReadLine());
                    p.periodoValidez = (fechaInicio, fechaFin);
                    Console.WriteLine("Periodo de validez modificado exitosamente.");
                    break;
            }

        }
    }
}
