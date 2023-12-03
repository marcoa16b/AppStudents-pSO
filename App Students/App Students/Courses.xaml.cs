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
    public sealed partial class Courses : Page
    {
        public Courses()
        {
            this.InitializeComponent();

            UpdateUserData();
            UpdateCoursesList();
        }

        private void UpdateUserData()
        {
            Curso1.Text = "";
            Curso2.Text = "";
            Curso3.Text = "";

            HandleOperations.UpdateTestingCoursesData();
            HandleOperations.UpdateDataOfLoggedUser();
            List<Course> courses = HandleOperations.RegisteredCourses;
            User user = HandleOperations.LoggedUser;

            if (!string.IsNullOrEmpty(user.CourseID1))
            {
                Curso1.Text = "Curso 1: " + user.CourseID1;
            }
            if (!string.IsNullOrEmpty(user.CourseID2))
            {
                Curso2.Text = "Curso 2: " + user.CourseID2;
            }
            if (!string.IsNullOrEmpty(user.CourseID3))
            {
                Curso3.Text = "Curso 3: " + user.CourseID3;
            }
        }

        private void UpdateCoursesList()
        {
            AllCourses.Items.Clear();
            HandleOperations.UpdateTestingCoursesData();
            List<Course> courses = HandleOperations.RegisteredCourses;
            foreach (Course course in courses)
            {
                string courseInfo = course.Codigo + " - " + course.Nombre;
                AllCourses.Items.Add(courseInfo);
            }
        }

        private void CourseCode_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            List<Course> courses = HandleOperations.RegisteredCourses;
            // Filtrar cursos segun el codigo
            if (CourseCode.Text != "")
            {
                AllCourses.Items.Clear();
                foreach (Course course in courses)
                {
                    if (course.Codigo.Contains(CourseCode.Text))
                    {
                        string courseInfo = course.Codigo + " - " + course.Nombre;
                        AllCourses.Items.Add(courseInfo);
                    }
                }
            }
            else
            {
                AllCourses.Items.Clear();
                foreach (Course course in courses)
                {
                    string courseInfo = course.Codigo + " - " + course.Nombre;
                    AllCourses.Items.Add(courseInfo);
                }
            }
        }

        private async void Enroll_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si el usuario ha seleccionado un curso
            if (AllCourses.SelectedItem != null)
            {
                string courseInfo = AllCourses.SelectedItem.ToString();
                string[] courseInfoSplit = courseInfo.Split('-');
                string courseCode = courseInfoSplit[0].Trim(' ');

                Response res = HandleOperations.UpdateUserCourse(courseCode);

                if (res.Success)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Registrado exitosamente",
                        Content = "El curso se ha registrado con exito",
                        CloseButtonText = "Ok"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    await dialog.ShowAsync();
                    UpdateUserData();
                    UpdateCoursesList();
                }
                else
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Error al registrar el curso: " + res.Message,
                        CloseButtonText = "Ok"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    await dialog.ShowAsync();
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Debe seleccionar un curso",
                    CloseButtonText = "Ok"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AllCourses.SelectedItem != null)
            {
                string courseInfo = AllCourses.SelectedItem.ToString();
                string[] courseInfoSplit = courseInfo.Split('-');
                string courseCode = courseInfoSplit[0].Trim(' ');

                Response res = HandleOperations.DeleteUserCourseRegistered(courseCode);

                if (res.Success)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Eliminado exitosamente",
                        Content = "El curso se ha eliminado con éxito",
                        CloseButtonText = "Ok"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    await dialog.ShowAsync();
                    UpdateUserData();
                    UpdateCoursesList();
                }
                else
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Error al eliminar el curso: " + res.Message,
                        CloseButtonText = "Ok"
                    };
                    dialog.XamlRoot = this.Content.XamlRoot;
                    await dialog.ShowAsync();
                }

            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Debe seleccionar un curso",
                    CloseButtonText = "Ok"
                };
                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
        }
    }
}
