using System;
using System.IO;
using System.Windows;
using ControlMastologiaV2;
using Microsoft.EntityFrameworkCore;

namespace ControlMastologiaV2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // 1. Definir la ruta de la carpeta de la App en Documentos
                string baseDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string carpetaApp = Path.Combine(baseDocs, "ControlMastologia");
                string carpetaInformes = Path.Combine(carpetaApp, "Informes");

                // 2. Crear las carpetas si no existen
                if (!Directory.Exists(carpetaInformes))
                {
                    Directory.CreateDirectory(carpetaInformes);
                }

                // 3. Asegurar la BD y parchar la columna nueva automáticamente
                using (var db = new AppDbContext())
                {
                    db.Database.EnsureCreated();

                    try
                    {
                        // Intenta agregar la columna a la fuerza si la tabla ya existía
                        db.Database.ExecuteSqlRaw("ALTER TABLE Pacientes ADD COLUMN Cesareas INTEGER DEFAULT 0;");
                        db.Database.ExecuteSqlRaw("ALTER TABLE Pacientes ADD COLUMN EmbarazoEctopico INTEGER DEFAULT 0;");
                    }
                    catch
                    {
                        // Si la columna ya fue creada antes, SQLite dará error y el catch lo ignora para que la app abra normal
                    }
                }

                // Continuar con el inicio normal de la aplicación
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error crítico al iniciar la aplicación:\n" + ex.Message,
                                "Error de Inicio", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}