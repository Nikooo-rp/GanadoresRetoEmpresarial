namespace GanadoresRetoEmpresarial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = new HotelData();
            MenuCliente menuCliente = new MenuCliente();
            MenuAdmin menuAdmin = new MenuAdmin();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Sistema de administración del hotel ===");
                string nombreUsuario = Admin.AskString("Ingresa tu nombre de usuario:");

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
