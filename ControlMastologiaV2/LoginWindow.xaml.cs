using System.Windows;

namespace ControlMastologiaV2
{
    public partial class LoginWindow : Window
    {
        public LoginWindow() { InitializeComponent(); }

        private void BtnIngresar_Click(object sender, RoutedEventArgs e)
        {
            // Cambia "mastologia2026" por la contraseña que tú quieras
            if (txtPass.Password == "mastologia2026")
            {
                // Abrimos el programa principal
                PatientRegistrationWindow main = new PatientRegistrationWindow();
                main.Show();
                this.Close(); // Cerramos la ventana de Login
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta, intente de nuevo.");
                txtPass.Clear();
            }
        }
    }
}