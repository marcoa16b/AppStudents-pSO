using App_Students.Logic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App_Students
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Auth : Page
    {
        public Auth()
        {
            this.InitializeComponent();
        }

        public enum NivelSeguridad
        {
            MuyDebil,
            Debil,
            Moderada,
            Fuerte,
            MuyFuerte
        }

        public NivelSeguridad EvaluarSeguridadContrasena(string contrasena)
        {
            int puntaje = 0;

            // Reglas para evaluar la contraseña
            if (contrasena.Length < 6)
                puntaje += 1;

            if (contrasena.Length >= 8)
                puntaje += 1;

            if (System.Text.RegularExpressions.Regex.IsMatch(contrasena, "[a-z]"))
                puntaje += 1;

            if (System.Text.RegularExpressions.Regex.IsMatch(contrasena, "[A-Z]"))
                puntaje += 1;

            if (System.Text.RegularExpressions.Regex.IsMatch(contrasena, "[0-9]"))
                puntaje += 1;

            if (System.Text.RegularExpressions.Regex.IsMatch(contrasena, "[^a-zA-Z0-9]"))
                puntaje += 1;

            // Determinar el nivel de seguridad basado en el puntaje obtenido
            if (puntaje < 3)
                return NivelSeguridad.MuyDebil;
            else if (puntaje == 3)
                return NivelSeguridad.Debil;
            else if (puntaje == 4)
                return NivelSeguridad.Moderada;
            else if (puntaje == 5)
                return NivelSeguridad.Fuerte;
            else
                return NivelSeguridad.MuyFuerte;
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = InputEmail.Text;
            string password = InputPassword.Password;
            ButtonLogin.Content = "Procesando...";
            ButtonLogin.IsEnabled = false;

            Response res = HandleOperations.LogIn(email, password);
            ContentDialog dialogLogin;

            if (res.Success)
            {
                ButtonLogin.Content = "Ingresado...";
                ButtonLogin.IsEnabled = false;
                HandleOperations.MainFrame = this.Frame;
                this.Frame.Navigate(typeof(Home));
            }
            else
            {
                ButtonLogin.Content = "Ingresar";
                ButtonLogin.IsEnabled = true;
                dialogLogin = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Fallo el inicio de sesión",
                    Content = res.Message,
                    CloseButtonText = "Ok"
                };
                await dialogLogin.ShowAsync();
            }

        }

        private async void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            string name = InputName.Text;
            string email = InputEmailRegister.Text;
            string password = InputPasswordRegister.Password;
            ButtonRegister.Content = "Procesando...";
            ButtonRegister.IsEnabled = false;

            ContentDialog dialog;

            NivelSeguridad nivel = EvaluarSeguridadContrasena(password);
            if (nivel == NivelSeguridad.MuyDebil)
            {
                dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Registro fallido",
                    Content = "La contraseña es muy debil",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
                ButtonRegister.Content = "Registrarme";
                ButtonRegister.IsEnabled = true;
                return;
            }
            else if (nivel == NivelSeguridad.Debil)
            {
                dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Registro fallido",
                    Content = "La contraseña es debil",
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
                ButtonRegister.Content = "Registrarme";
                ButtonRegister.IsEnabled = true;
                return;
            }

            Response res = HandleOperations.Register(name, email, password);


            if (res.Success)
            {
                dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Registro exitoso",
                    Content = res.Message,
                    CloseButtonText = "Ok"
                };
                ButtonRegister.Content = "Registrado...";
                ButtonRegister.IsEnabled = false;
            }
            else
            {
                dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Registro fallido",
                    Content = res.Message,
                    CloseButtonText = "Ok"
                };
                ButtonRegister.Content = "Registrarme";
                ButtonRegister.IsEnabled = true;
            }
            await dialog.ShowAsync();

        }

        private void InputPasswordRegister_PasswordChanging(PasswordBox sender, PasswordBoxPasswordChangingEventArgs args)
        {
            string password = sender.Password;
            NivelSeguridad nivel = EvaluarSeguridadContrasena(password);
            if (nivel == NivelSeguridad.MuyDebil)
            {
                PasswordSecurity.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
            else if (nivel == NivelSeguridad.Debil)
            {
                PasswordSecurity.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 111, 0));// Windows.UI.Colors.Orange);
            }
            else if (nivel == NivelSeguridad.Moderada)
            {
                PasswordSecurity.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 234, 0)); // Windows.UI.Colors.Yellow);
            }
            else if (nivel == NivelSeguridad.Fuerte)
            {
                PasswordSecurity.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 112, 0)); // Windows.UI.Colors.Green);
            }
            else
            {
                PasswordSecurity.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)); // Windows.UI.Colors.Blue);
            }
            PasswordSecurity.Text = "Nivel de seguridad: " + nivel.ToString();
        }

        private void InputPasswordRegisterConfirm_PasswordChanging(PasswordBox sender, PasswordBoxPasswordChangingEventArgs args)
        {
            string firstPassword = InputPasswordRegister.Password;
            string secondPassword = InputPasswordRegisterConfirm.Password;

            if (firstPassword == secondPassword)
            {
                PasswordConfirm.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 112, 0)); // Windows.UI.Colors.Green);
                PasswordConfirm.Text = "Las contraseñas coinciden";
            }
            else
            {
                PasswordConfirm.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Windows.UI.Colors.Red);
                PasswordConfirm.Text = "Las contraseñas no coinciden";
            }
        }
    }
}
