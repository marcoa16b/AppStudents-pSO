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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App_Students
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyData : Page
    {
        public MyData()
        {
            this.InitializeComponent();

            UpdateData();
        }

        private void UpdateData()
        {
            InputName.Text = HandleOperations.LoggedUser.Name;
            InputEmail.Text = HandleOperations.LoggedUser.Email;
            InputIdentification.Text = HandleOperations.LoggedUser.Cedula;
            InputFLastname.Text = HandleOperations.LoggedUser.Apellido1;
            InputSLastname.Text = HandleOperations.LoggedUser.Apellido2;
            InputPhone.Text = HandleOperations.LoggedUser.TelefonoContacto;
            InputDate.Date = HandleOperations.LoggedUser.FechaNacimiento;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string newName = InputName.Text;
            string newEmail = InputEmail.Text;
            string newIdentification = InputIdentification.Text;
            string newFLastname = InputFLastname.Text;
            string newSLastname = InputSLastname.Text;
            string newPhone = InputPhone.Text;
            DateTime newDate = InputDate.Date.DateTime;

            Response res = HandleOperations.UpdateUser(newName, newEmail, newIdentification, newFLastname, newSLastname, newPhone, newDate);
            if (res.Success)
            {
                ContentDialog dialog;
                dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Datos actualizados",
                    Content = res.Message,
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
                UpdateData();
            }
            else
            {
                ContentDialog dialog;
                dialog = new ContentDialog()
                {
                    XamlRoot = this.Content.XamlRoot,
                    Title = "Actualizacion fallida",
                    Content = res.Message,
                    CloseButtonText = "Ok"
                };
                await dialog.ShowAsync();
            }
        }
    }
}
