using App_Students.Logic;
using App_Students.Logic.Models;
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
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            UpdateCoursesData();
        }

        private void UpdateCoursesData()
        {
            HandleOperations.UpdateTestingCoursesData();

            List<Course> courses = HandleOperations.RegisteredCourses;

            CoursesList.Items.Clear();
            foreach (Course course in courses)
            {
                string courseData = course.Codigo + " - " + course.Nombre;
                CoursesList.Items.Add(courseData);
            }

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Response res = HandleOperations.CreateTestingCoursesData();

            if (res.Success)
            {
                LoadButton.IsEnabled = false;
                LoadText.Text = "Cursos cargados correctamente";
                UpdateCoursesData();
            }
            else
            {
                LoadText.Text = "Error al cargar cursos: " + res.Message;
            }
        }

        private async void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            Response res = HandleOperations.DeleteAllCourses();
            if (res.Success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Cursos eliminados",
                    Content = "Todos los cursos han sido eliminados correctamente",
                    CloseButtonText = "Ok"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                UpdateCoursesData();
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error al eliminar cursos",
                    Content = "Error al eliminar cursos: " + res.Message,
                    CloseButtonText = "Ok"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string code = InputCode.Text;
            string name = InputTitle.Text;

            Response res = HandleOperations.RegisterCourse(code, name);

            if (res.Success)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Curso registrado",
                    Content = "El curso ha sido registrado correctamente",
                    CloseButtonText = "Ok"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
                UpdateCoursesData();
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error al registrar curso",
                    Content = "Error al registrar curso: " + res.Message,
                    CloseButtonText = "Ok"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }
    }
}
