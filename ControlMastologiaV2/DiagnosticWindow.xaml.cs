using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControlMastologiaV2
{
    public partial class DiagnosticWindow : Window
    {
        private int pacienteIdActual;
        private List<Consulta> listaConsultas = new List<Consulta>();

        public DiagnosticWindow(Paciente p)
        {
            InitializeComponent();
            pacienteIdActual = p.Id;
            CargarHistorial();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var p = db.Pacientes.Find(pacienteIdActual);
                    string tipo = (cmbTipoInforme.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Informe Médico";

                    var nuevaConsulta = new Consulta
                    {
                        PacienteId = pacienteIdActual,
                        Fecha = DateTime.Now,
                        TipoInforme = tipo,
                        PacienteNombreSnapshot = p.Nombre,
                        PacienteEdadSnapshot = p.Edad,
                        PacienteCedulaSnapshot = p.Cedula,
                        HistorialMotivo = p.MotivoConsulta,
                        HistorialEnfActual = p.EnfermedadActual,
                        HistorialAntPersonales = p.AntecedentesPersonales,
                        HistorialAntFamiliares = p.AntecedentesFamiliares,
                        EsEmbarazada = p.embarazada,
                        HistorialGestas = p.gestas,
                        HistorialPartos = p.partos,
                        HistorialAbortos = p.abortos,
                        HistorialCesareas = p.Cesareas,
                        HistorialEmbarazoEctopico = p.EmbarazoEctopico,

                        Diagnostico = txtDiagnostico.Text ?? "",
                        Tratamiento = txtTratamiento.Text ?? ""
                    };

                    if (tipo == "Eco Mamario")
                    {
                        nuevaConsulta.ExamenFisico = txtExamenFisicoMamario.Text;
                        nuevaConsulta.HallazgosEcograficos = txtHallazgosMamario.Text;
                        nuevaConsulta.ConclusionesRecomendaciones = txtConclusionesMamario.Text;
                    }
                    else if (tipo == "Eco Ginecológico")
                    {
                        nuevaConsulta.Hallazgos = txthallazgos.Text;
                        nuevaConsulta.HallazgosEcograficos = txtHallazgosGineco.Text;
                        nuevaConsulta.ConclusionesRecomendaciones = txtConclusionesGineco.Text;
                    }

                    db.Consultas.Add(nuevaConsulta);
                    db.SaveChanges();
                }
                MessageBox.Show("Guardado exitosamente.");
                CargarHistorial();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void CargarHistorial()
        {
            using (var db = new AppDbContext())
            {
                listaConsultas = db.Consultas
                                   .Where(c => c.PacienteId == pacienteIdActual)
                                   .OrderByDescending(c => c.Fecha)
                                   .ToList();
                // 1. Asignar la lista como fuente de datos
                cmbConsultas.ItemsSource = listaConsultas;

                // 2. Decirle que muestre la fecha (o lo que prefieras)
                cmbConsultas.DisplayMemberPath = "Fecha";

                // Tu lógica de texto del historial sigue igual...

                txtHistorial.Text = string.Join("\n\n", listaConsultas.Select(c =>
                {
                    // Bloque común a todos los informes
                    string detalle = $"Fecha de nacimiento: {c.Fecha.ToString("dd/MM/yyyy HH:mm")} | Tipo: {c.TipoInforme}\n" +
                    $"Paciente: {c.PacienteNombreSnapshot} | Edad: {c.PacienteEdadSnapshot} años\n" +
                    $"Motivo de Consulta: {c.HistorialMotivo}\n" +
                    $"Antecedentes Personales: {c.HistorialAntPersonales}\n" +
                    $"Antecedentes Familiares: {c.HistorialAntFamiliares}\n" +
                    $"Enfermedad Actual: {c.HistorialEnfActual}\n" +
                    $"Embarazada: {(c.EsEmbarazada ? "Sí" : "No")}\n" +
                    $"Gestas: {c.HistorialGestas} | Partos: {c.HistorialPartos} | Abortos: {c.HistorialAbortos}\n" +
                    $"Cesareas: {c.HistorialCesareas} | Embarazo Ectópico: {c.HistorialEmbarazoEctopico}\n";



                    // Bloque condicional según el tipo de informe
                    if (c.TipoInforme == "Eco Mamario")
                    {
                        detalle += $"--- TIPO: ECO MAMARIO ---\n" +
                                   $"Examen Físico: {c.ExamenFisico}\n" +
                                   $"Hallazgos Ecográficos: {c.HallazgosEcograficos}\n" +
                                   $"Conclusiones: {c.ConclusionesRecomendaciones}";
                    }
                    else if (c.TipoInforme == "Eco Ginecológico")
                    {
                        detalle += $"--- TIPO: ECO GINECOLÓGICO ---\n" +
                                   $"Hallazgos: {c.Hallazgos}\n" +
                                   $"Ecosonograma: {c.HallazgosEcograficos}\n" +
                                   $"Conclusiones: {c.ConclusionesRecomendaciones}";
                    }
                    else // Informe Médico estándar
                    {
                        detalle += $"--- TIPO: INFORME MÉDICO ---\n" +
                                   $"Diagnóstico: {c.Diagnostico}\n" +
                                   $"Tratamiento: {c.Tratamiento}";
                    }

                    return detalle + "\n-------------------------------------------";
                }));
            }
        }

        private void CmbConsultas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbConsultas.SelectedItem is Consulta c)
            {
                txtDiagnostico.Text = c.Diagnostico;
                txtTratamiento.Text = c.Tratamiento;
                txtExamenFisicoMamario.Text = c.ExamenFisico;
                txtHallazgosMamario.Text = c.HallazgosEcograficos;
                txtConclusionesMamario.Text = c.ConclusionesRecomendaciones;
                txthallazgos.Text = c.Hallazgos;
                txtHallazgosGineco.Text = c.HallazgosEcograficos;
                txtConclusionesGineco.Text = c.ConclusionesRecomendaciones;
            }
        }

        private void CmbTipoInforme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipoInforme.SelectedItem is not ComboBoxItem item) return;
            string tipo = item.Content.ToString();
            pnlInformeMedico.Visibility = (tipo == "Informe Médico") ? Visibility.Visible : Visibility.Collapsed;
            pnlEcoMamario.Visibility = (tipo == "Eco Mamario") ? Visibility.Visible : Visibility.Collapsed;
            pnlEcoGinecologico.Visibility = (tipo == "Eco Ginecológico") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnGenerar_Informe_Click(object sender, RoutedEventArgs e)
        {
            if (cmbConsultas.SelectedItem is Consulta c) GenerarPDF(c);
            else MessageBox.Show("Seleccione una consulta.");
        }
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            var main = new PatientRegistrationWindow();
            main.Show();
            this.Close();
        }

        private void GenerarPDF(Consulta c)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // 1. Definir la carpeta "Informes" junto al ejecutable y crearla si no existe
            string carpetaInformes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Informes");
            if (!Directory.Exists(carpetaInformes))
            {
                Directory.CreateDirectory(carpetaInformes);
            }

            // 2. Obtenemos el nombre del paciente (priorizando el Snapshot)
            string nombrePaciente = !string.IsNullOrWhiteSpace(c.PacienteNombreSnapshot)
                ? c.PacienteNombreSnapshot
                : (c.Paciente?.Nombre ?? "Paciente");

            // 3. Limpiamos caracteres que Windows no permite en nombres de archivos
            foreach (char cInvalid in Path.GetInvalidFileNameChars())
            {
                nombrePaciente = nombrePaciente.Replace(cInvalid, '_');
            }

            // 4. Armamos la ruta completa del archivo dentro de la carpeta Informes
            string nombreArchivo = $"Informe_{nombrePaciente}_{DateTime.Now:ddMMyy_HHmmss}.pdf";
            string rutaCompletaPdf = Path.Combine(carpetaInformes, nombreArchivo);

            // Obtenemos el logo del ensamblado
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var logoStream = assembly.GetManifestResourceStream("ControlMastologiaV2.logo.jpeg"))
            {
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(2, Unit.Centimetre);

                        // --- ENCABEZADO ---
                        page.Header().Column(column =>
                        {
                            column.Item().Row(row =>
                            {
                                // 1. Logo a la izquierda
                                if (logoStream != null)
                                {
                                    row.ConstantItem(80).Image(logoStream).FitArea();
                                    row.ConstantItem(10); // Espacio entre logo y texto
                                }

                                // 2. Texto a la derecha
                                row.RelativeItem().Column(textCol =>
                                {
                                    textCol.Item().Text("Consulta Especializada en Mastología").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                                    textCol.Item().Text("Ginecólogo - Mastólogo").FontSize(14).FontColor(Colors.Grey.Medium);
                                    textCol.Item().Text("Control Clínico de Pacientes").FontSize(12).FontColor(Colors.Grey.Medium);   
                                });
                            });

                            // Línea divisoria debajo del encabezado
                            column.Item().PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        });

                        // --- CONTENIDO ---
                        page.Content().PaddingVertical(20).Column(column =>
                        {
                            // Título
                            column.Item().Text(c.TipoInforme?.ToUpper() ?? "INFORME MÉDICO").FontSize(18).SemiBold().AlignCenter();
                            column.Item().PaddingTop(20);

                            // Datos del Paciente
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text(x => { x.Span("Paciente: ").SemiBold(); x.Span(c.PacienteNombreSnapshot ?? "N/A"); });
                                row.RelativeItem().Text(x => { x.Span("Edad: ").SemiBold(); x.Span($"{c.PacienteEdadSnapshot} años"); });
                                row.RelativeItem().Text(x => { x.Span("Cédula: ").SemiBold(); x.Span(c.PacienteCedulaSnapshot ?? "N/A"); });
                                row.RelativeItem().Text(x => { x.Span("Fecha: ").SemiBold(); x.Span(c.Fecha.ToShortDateString()); });
                            });

                            column.Item().PaddingTop(15);

                            void AddSection(string title, string content)
                            {
                                column.Item().PaddingTop(10).Text(title).SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);
                                column.Item().Text(content ?? "No registrado").FontSize(11);
                            }

                            AddSection("Antecedentes Personales:", c.HistorialAntPersonales);
                            AddSection("Antecedentes Familiares:", c.HistorialAntFamiliares);
                            AddSection("Motivo de Consulta:", c.HistorialMotivo);

                            if (c.TipoInforme == "Eco Mamario")
                            {
                                AddSection("Examen Físico:", c.ExamenFisico);
                                AddSection("Hallazgos:", c.HallazgosEcograficos);
                                AddSection("Conclusiones y Recomendaciones:", c.ConclusionesRecomendaciones);
                            }
                            else if (c.TipoInforme == "Eco Ginecológico")
                            {
                                AddSection("Hallazgos:", c.Hallazgos);
                                AddSection("Ecosonograma:", c.HallazgosEcograficos);
                                AddSection("Conclusiones y Recomendaciones:", c.ConclusionesRecomendaciones);
                            }
                            else
                            {
                                // Informe Médico o defecto
                                AddSection("Enfermedad Actual:", c.HistorialEnfActual);
                                AddSection("Diagnóstico:", c.Diagnostico);
                                AddSection("Tratamiento:", c.Tratamiento);
                            }
                        });

                        // --- PIE DE PÁGINA ---
                        page.Footer().PaddingTop(20).Column(footerColumn =>
                        {
                            footerColumn.Item().AlignCenter().PaddingBottom(10).Width(200).BorderBottom(1).BorderColor(Colors.Black);
                            footerColumn.Item().AlignCenter().Text("Firma y Sello del Especialista").FontSize(10).SemiBold();
                            footerColumn.Item().AlignCenter().Text("Ginecólogo - Mastólogo").FontSize(10).FontColor(Colors.Grey.Medium);
                            footerColumn.Item().AlignCenter().Text("Registro Médico / MPPS").FontSize(10).FontColor(Colors.Grey.Medium);
                            footerColumn.Item().PaddingTop(10).BorderTop(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().Text(x =>
                            {
                                x.Span("Centro Médico de Atención Especializada | Consulta Externa");
                            });
                        });
                    });
                }).GeneratePdf(rutaCompletaPdf);

                // Abre automáticamente el PDF recién generado desde la nueva carpeta
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaCompletaPdf) { UseShellExecute = true });
            }
        }
    }
}
               