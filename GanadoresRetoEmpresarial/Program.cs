namespace GanadoresRetoEmpresarial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = new HotelData();
            MenuCliente menuCliente = new MenuCliente();
            MenuAdmin menuAdmin = new MenuAdmin();

            data.admins.AddRange(new List<Admin>
            {
                new Admin("admin1", "adpass1"),
                new Admin("admin2", "adpass2")
            });

            data.recepcionistas.AddRange(new List<Recepcionista>
            {
                new Recepcionista("recep1", "repass1"),
                new Recepcionista("recep2", "repass2")
            });

            data.habitaciones.AddRange(new List<Habitacion>
            {
                new Habitacion(101, 75,  TipoHabitacion.Sencilla),
                new Habitacion(102, 75, TipoHabitacion.Sencilla),
                new Habitacion(201, 150, TipoHabitacion.Doble),
                new Habitacion(202, 150, TipoHabitacion.Doble)
            });
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Sistema de administración de Velisse Hotel ===");
                string nombreUsuario = Admin.AskString("Ingresa tu nombre de usuario o 0 para salir:");
                
                if (nombreUsuario == "0")
                {
                    running = false;
                    continue;
                }
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

                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado, presiona cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }
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
