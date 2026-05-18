namespace GanadoresRetoEmpresarial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = new HotelData();
            MenuCliente menuCliente = new MenuCliente();
            MenuAdmin menuAdmin = new MenuAdmin();

            //Base de Admins y Recepcionistas
            data.admins = new List<Admin>
            {
                new Admin("admin1", "adminpass1"),
                new Admin("admin2", "adminpass2")
            };

            data.recepcionistas = new List<Recepcionista>
            {
                new Recepcionista("recep1", "receppass1"),
                new Recepcionista("recep2", "receppass2")
            };

            // Base de habitaciones
            data.habitaciones = new List<Habitacion>
            {
                // numero, precioNoche, tipo (Sencilla o Doble)
                new Habitacion(101, 75, TipoHabitacion.Sencilla),
                new Habitacion(102, 75, TipoHabitacion.Sencilla),
                new Habitacion(103, 150, TipoHabitacion.Doble),
            };

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Sistema de administración de Velisse Hotel ===");
                string nombreUsuario = Admin.AskString("Ingresa tu nombre de usuario:");

                // Primero, buscamos un usuario que coincida con el nombre ingresado en las listas de clientes, recepcionistas y admins
                Usuario? usuario = null;
                if (data.clientes.Find(m => m.nombre == nombreUsuario) is Cliente cliente)
                {
                    usuario = cliente;
                }
                else if (data.recepcionistas.Find(r => r.nombre == nombreUsuario) is Recepcionista recepcionista)
                {
                    usuario = recepcionista;
                }
                else if (data.admins.Find(a => a.nombre == nombreUsuario) is Admin admin)
                {
                    usuario = admin;
                }

                // Si no se encontró ningún usuario, mostramos un mensaje de error y volvemos a pedir el nombre de usuario
                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado, presiona cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }
                // Si se encontró un usuario, pedimos la contraseña y verificamos si es correcta
                else
                {
                    string contraseña = Admin.AskString("Ingresa tu contraseña:");
                    if (usuario.contraseña == contraseña)
                    {
                        switch (usuario)
                        {
                            case Cliente c:
                                menuCliente.MostrarMenu(c, data);
                                break;
                            case Recepcionista r:
                                //MenuRecepcionista menuRecepcionista = new MenuRecepcionista(r, data);
                                //menuRecepcionista.MostrarMenu();
                                break;
                            case Admin a:
                                menuAdmin.Mostrar(a, data);
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Contraseña incorrecta, presiona cualquier tecla...");
                        Console.ReadKey();
                        continue;
                    }
                }

                
            }
            Console.WriteLine("Adiós!");
        }
    }
}
