namespace GanadoresRetoEmpresarial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Folder Path para guardar datos.
            // La primera linea obtiene el directorio del escritorio del usuario.
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // La segunda linea combina el path del escritorio con el nombre de la carpeta donde se guardarán los datos, y luego crea esa carpeta.
            string fullPath = Path.Combine(desktopPath, "VelisseHotelData");
            Directory.CreateDirectory(fullPath);

            //En caso de que ya exista la carpeta, CreateDirectory no hará nada y seguirá adelante, así que no hay riesgo de perder datos existentes.


            // Instanciamos datos y menús.
            var data = new HotelData();
            MenuCliente menuCliente = new MenuCliente();
            MenuAdmin menuAdmin = new MenuAdmin();

            // Base de datos inicial de usuarios y habitaciones.
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
            // Los clientes los registran los recepcionistas.

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Sistema de administración de Velisse Hotel ===");
                string nombreUsuario = AskTypes.AskString("Ingresa tu nombre de usuario o 0 para salir:");


                // Si el usuario ingresa "0", se cierra el programa. running se vuelve false y se continúa al siguiente ciclo, que termina inmediatamente.
                if (nombreUsuario == "0")
                {
                    running = false;
                    continue;
                }
                // Creamos un usuario que empieza en null, luego se recorren las listas de usuarios en busca de un nombre que coincida con el ingresado.
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

                // Si se encontró un usuario, se habrá asignado, sino, seguirá siendo null y se repetirá el ciclo.
                if (usuario == null)
                {
                    Console.WriteLine("Usuario no encontrado, presiona cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }
                // Si el usuario se encontró, se pide su contraseña. Si es válida, se muestra el menú correspondiente pasando el usuario actual y los datos.
                else
                {
                    string contraseña = AskTypes.AskString("Ingresa tu contraseña:");
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
