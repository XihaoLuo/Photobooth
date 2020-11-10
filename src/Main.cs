using System;
using Gtk;
 
namespace Photobooth
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();
            Window window = new MainWindow("Photobooth");
            Application.Run();
        }
    }
}