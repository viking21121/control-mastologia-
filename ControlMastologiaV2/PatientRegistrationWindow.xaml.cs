using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlMastologiaV2
{
    public partial class PatientRegistrationWindow : Window
    {
        public PatientRegistrationWindow() { InitializeComponent(); }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string cedulaIngresada = txtCedula.Text.Trim();
            string nombreIngresado = txtNombre.Text.Trim();

            // 1. Validar que al menos un campo tenga información
            if (string.IsNullOrWhiteSpace(cedulaIngresada) && string.IsNullOrWhiteSpace(nombreIngresado))
            {
                MessageBox.Show("Por favor, ingrese la cédula o el nombre para buscar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                // 2. Construir la consulta dinámicamente ("juntas o separadas")
                var query = db.Pacientes.AsQueryable();

                if (!string.IsNullOrWhiteSpace(cedulaIngresada))
                {
                    query = query.Where(p => p.Cedula == cedulaIngresada);
                }

                if (!string.IsNullOrWhiteSpace(nombreIngresado))
                {
                    // Usamos Contains para encontrar coincidencias parciales por nombre
                    query = query.Where(p => p.Nombre.Contains(nombreIngresado));
                }

                // Ejecutamos la búsqueda
                var pacientesEncontrados = query.ToList();

                // CASO A: Sin coincidencias
                if (pacientesEncontrados.Count == 0)
                {
                    MessageBox.Show("Paciente no encontrado. Puede proceder a registrar sus datos.", "Sin Coincidencias", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // CASO B: Un único paciente encontrado (Cargamos el formulario como tenías planeado)
                if (pacientesEncontrados.Count == 1)
                {
                    var pacienteExistente = pacientesEncontrados.First();

                    // Si la pantalla ya tenía un nombre escrito diferente, preguntamos antes de reemplazar
                    if (!string.IsNullOrWhiteSpace(nombreIngresado) &&
                        !pacienteExistente.Nombre.Trim().Equals(nombreIngresado, StringComparison.OrdinalIgnoreCase))
                    {
                        var confirmacion = MessageBox.Show(
                            $"La búsqueda coincide con '{pacienteExistente.Nombre}'.\n\n¿Desea cargar los datos de este paciente?",
                            "Paciente Encontrado",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (confirmacion == MessageBoxResult.No)
                        {
                            return; // Cancelamos para que el usuario corrija la información
                        }
                    }

                    // Cargar los datos de la BD a las cajas de texto
                    txtCedula.Text = pacienteExistente.Cedula; // Por si se buscó solo por nombre
                    txtNombre.Text = pacienteExistente.Nombre;
                    cmbSexo.Text = pacienteExistente.Sexo;
                    txtCorreo.Text = pacienteExistente.Correo;
                    txtEnfermedad.Text = pacienteExistente.EnfermedadActual;
                    txtAntPersonales.Text = pacienteExistente.AntecedentesPersonales;
                    txtAntFamiliares.Text = pacienteExistente.AntecedentesFamiliares;
                    txtMotivo.Text = pacienteExistente.MotivoConsulta;
                    txtAbortos.Text = pacienteExistente.abortos;
                    txtGestas.Text = pacienteExistente.gestas;
                    txtPartos.Text = pacienteExistente.partos;
                    txtCesareas.Text = pacienteExistente.Cesareas;
                    txtEmbarazoEctopico.Text = pacienteExistente.EmbarazoEctopico;
                    chkEmbarazada.IsChecked = pacienteExistente.embarazada;
                    datePicker.SelectedDate = pacienteExistente.FechaNacimiento;

                    // Verificar consultas previas del paciente cargado
                    var historial = db.Consultas.Where(c => c.PacienteId == pacienteExistente.Id).ToList();

                    if (historial.Count > 0)
                    {
                        MessageBox.Show($"Paciente encontrado: {pacienteExistente.Nombre}.\nTiene {historial.Count} consulta(s) previa(s).", "Paciente Encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Paciente encontrado: {pacienteExistente.Nombre}.\nNo tiene historial de consultas previas.", "Paciente Encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Preguntar si desea navegar al historial
                    var result = MessageBox.Show("¿Desea ir al historial médico del paciente?", "Acción completada", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var diag = new DiagnosticWindow(pacienteExistente);
                        diag.Show();
                        // Dependiendo de la navegación deseada, podrías cerrar la ventana actual:
                        // this.Close();
                    }
                }
                // CASO C: Múltiples resultados encontrados
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Se encontraron varios pacientes. Aquí está su historial:\n");

                    foreach (var p in pacientesEncontrados)
                    {
                        sb.AppendLine($"• {p.Nombre} (C.I: {p.Cedula})");

                        var consultas = db.Consultas
                                          .Where(c => c.PacienteId == p.Id)
                                          .ToList();

                        if (consultas.Any())
                        {
                            foreach (var c in consultas)
                            {
                                // NOTA: Cambia 'c.Fecha' por el nombre real de tu propiedad de fecha en tu modelo Consulta (ej. FechaConsulta)
                                // Utilizamos un bloque try-catch visual implícito asumiendo que tienes una propiedad de Fecha.
                                sb.AppendLine($"   - Consulta: {c.Fecha:dd/MM/yyyy}");
                            }
                        }
                        else
                        {
                            sb.AppendLine("   - Sin historial de consultas previas.");
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine("Por favor, ingrese la Cédula exacta para cargar el formulario.");
                    MessageBox.Show(sb.ToString(), "Múltiples Coincidencias", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Nombre y Cédula son obligatorios.");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var p = db.Pacientes.FirstOrDefault(pa => pa.Cedula == txtCedula.Text);

                    if (p != null)
                    {
                        p.Nombre = txtNombre.Text;
                        p.Sexo = cmbSexo.Text;

                        // Solo actualizamos si el usuario eligió una fecha nueva
                        if (datePicker.SelectedDate.HasValue)
                        {
                            p.FechaNacimiento = datePicker.SelectedDate.Value;
                        }

                        p.Correo = txtCorreo.Text;
                        p.EnfermedadActual = txtEnfermedad.Text;
                        p.AntecedentesPersonales = txtAntPersonales.Text;
                        p.AntecedentesFamiliares = txtAntFamiliares.Text;
                        p.MotivoConsulta = txtMotivo.Text;
                        p.abortos = txtAbortos.Text;
                        p.gestas = txtGestas.Text;
                        p.partos = txtPartos.Text;
                        p.Cesareas = txtCesareas.Text;
                        p.EmbarazoEctopico = txtEmbarazoEctopico.Text;
                        p.embarazada = chkEmbarazada.IsChecked ?? false;

                        db.SaveChanges();
                        MessageBox.Show("Perfil del paciente actualizado correctamente.");
                    }
                    else
                    {
                        p = new Paciente
                        {
                            Nombre = txtNombre.Text,
                            Cedula = txtCedula.Text,
                            Sexo = cmbSexo.Text,
                            FechaNacimiento = datePicker.SelectedDate ?? DateTime.Now,
                            Correo = txtCorreo.Text,
                            EnfermedadActual = txtEnfermedad.Text,
                            AntecedentesPersonales = txtAntPersonales.Text,
                            AntecedentesFamiliares = txtAntFamiliares.Text,
                            MotivoConsulta = txtMotivo.Text,
                            abortos = txtAbortos.Text,
                            gestas = txtGestas.Text,
                            partos = txtPartos.Text,
                            Cesareas = txtCesareas.Text,
                            EmbarazoEctopico = txtEmbarazoEctopico.Text,
                            embarazada = chkEmbarazada.IsChecked ?? false
                        };
                        db.Pacientes.Add(p);
                        db.SaveChanges();
                        MessageBox.Show("Nuevo paciente registrado.");
                    }

                    var result = MessageBox.Show("¿Desea ir al historial médico del paciente?", "Acción completada", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        var diag = new DiagnosticWindow(p);
                        diag.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            // 1. Creamos una instancia de la ventana principal
            LoginWindow main = new LoginWindow();

            // 2. Mostramos la ventana principal
            main.Show();

            // 3. Cerramos la ventana de diagnóstico actual
            this.Close();
        }
        private void BtnRespaldo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Guardar dentro de Mis Documentos/ControlMastologia/Respaldos
                string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string backupDir = Path.Combine(documentsFolder, "ControlMastologia", "Respaldos");

                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string dbPath = Path.Combine(appDataFolder, "ControlMastologia", "ControlMastologia.db");

                if (!File.Exists(dbPath))
                {
                    MessageBox.Show("No se encontró la base de datos para respaldar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string nombreArchivo = $"Respaldo_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                string destFile = Path.Combine(backupDir, nombreArchivo);

                File.Copy(dbPath, destFile, true);
                MessageBox.Show($"Respaldo realizado con éxito en:\n{destFile}", "Respaldo Completo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el respaldo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChkGestas_Checked(object sender, RoutedEventArgs e) => pnlGestas.Visibility = Visibility.Visible;
        private void ChkGestas_Unchecked(object sender, RoutedEventArgs e) => pnlGestas.Visibility = Visibility.Collapsed;
        private void ChkTieneHijos_Checked(object sender, RoutedEventArgs e) { }
        private void ChkTieneHijos_Unchecked(object sender, RoutedEventArgs e) { }
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}