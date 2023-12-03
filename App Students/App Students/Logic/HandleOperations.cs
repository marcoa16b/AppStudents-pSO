using App_Students.Logic.Data;
using App_Students.Logic.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Students.Logic
{
    public class HandleOperations
    {
        static HandlerDB dbHandler;

        public static Frame MainFrame { get; set; }

        public static User LoggedUser { get; set; }

        public static List<Course> RegisteredCourses { get; set; }

        public static HandlerDB DBHandler
        {
            get
            {
                if (dbHandler == null)
                {
                    dbHandler = new HandlerDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Students.db3"));
                }
                return dbHandler;
            }
        }
        
        public static Response Register(string name, string email, string password)
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                var user = DBHandler.GetUserAsync(email).Result;
                if (user != null)
                {
                    throw new Exception("El usuario ya existe");
                    // response.Message = "User already exists";
                }
                else
                {
                    User newUser = new Models.User()
                    {
                        Name = name,
                        Email = email,
                        Password = PasswordService.HashPasswordArgon2(password),
                        Cedula = "",
                        Apellido1 = "",
                        Apellido2 = "",
                        FechaNacimiento = DateTime.Now,
                        TelefonoContacto = ""
                    };
                    int res = DBHandler.SaveUserAsync(newUser).Result;
                    response.Success = true;
                    response.Message = "Usuario registrado correctamente: " + res;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response LogIn(string email, string password)
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                var user = DBHandler.GetUserAsync(email).Result;
                if (user == null)
                {
                    throw new Exception("El usuario no existe");
                }
                else
                {
                    if (PasswordService.VerifyPasswordArgon2(password, user.Password))
                    {
                        LoggedUser = user;
                        response.Success = true;
                        response.Message = "Inicio de sesión exitoso";
                    }
                    else
                    {
                        throw new Exception("Contraseña incorrecta");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response DeleteUser(string email)
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                var user = DBHandler.GetUserAsync(email).Result;
                if (user == null)
                {
                    throw new Exception("El usuario no existe");
                }
                else
                {
                    DBHandler.DeleteUserAsync(user);
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static void UpdateDataOfLoggedUser()
        {
            var user = DBHandler.GetUserAsync(LoggedUser.Email).Result;
            if (user == null)
            {
                throw new Exception("No se ha podido actualizar la información");
            }
            else
            {
                LoggedUser = user;
            }
        }

        public static Response UpdateUser(string name, string email, string identification, string firstLastName, string secondLastName, string phone, DateTime date)
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                User user = new Models.User()
                {
                    ID = LoggedUser.ID,
                    Name = name,
                    Email = email,
                    Password = LoggedUser.Password,
                    Cedula = identification,
                    Apellido1 = firstLastName,
                    Apellido2 = secondLastName,
                    TelefonoContacto = phone,
                    FechaNacimiento = date
                };
                int res = DBHandler.SaveUserAsync(user).Result;
                UpdateDataOfLoggedUser();
                response.Success = true;
                response.Message = "Usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response CreateTestingCoursesData()
        {
            Response response = new Response()
            {
                Success = false
            };
            try
            {
                List<Course> coursesList = DBHandler.GetCoursesAsync().Result;

                if (coursesList.Count > 0)
                {
                    throw new Exception("Ya existen cursos en la base de datos");
                }

                List<Course> courses = new List<Course>()
                {
                    new Course()
                    {
                        Nombre = "Curso de Calculo 1",
                        Codigo = "100001"
                    },
                    new Course()
                    {
                        Nombre = "Curso de Calculo 2",
                        Codigo = "100002"
                    },
                    new Course()
                    {
                        Nombre = "Curso de Ingles para informatica",
                        Codigo = "100003"
                    },
                    new Course()
                    {
                        Nombre = "Curso de Programacion 1",
                        Codigo = "100004"
                    },
                    new Course()
                    {
                        Nombre = "Curso de Programacion 2",
                        Codigo = "100005"
                    },
                    new Course()
                    {
                        Nombre = "Curso de literatura",
                        Codigo = "100006"
                    },
                };

                foreach (var course in courses)
                {
                    int res = DBHandler.SaveCourseAsync(course).Result;
                    if (res != 1)
                    {
                        throw new Exception("No se pudo guardar el curso: " + course.Nombre);
                    }
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static void UpdateTestingCoursesData()
        {
            List<Course> coursesList = DBHandler.GetCoursesAsync().Result;
            RegisteredCourses = coursesList;
        }

        public static Response RegisterCourse(string courseCode, string courseName)
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                var course = DBHandler.GetCourseAsync(courseCode).Result;
                if (course != null)
                {
                    throw new Exception("Ya existe un curso con el mismo código");
                }
                else
                {
                    Course newCourse = new Course()
                    {
                        Nombre = courseName,
                        Codigo = courseCode
                    };
                    int res = DBHandler.SaveCourseAsync(newCourse).Result;
                    if (res == 1)
                    {
                        response.Success = true;
                        response.Message = "Curso registrado correctamente";
                    }
                    else
                    {
                        throw new Exception("No se pudo registrar el curso");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response DeleteCourse(string courseCode)
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                var course = DBHandler.GetCourseAsync(courseCode).Result;
                if (course == null)
                {
                    throw new Exception("El curso no existe");
                }
                else
                {
                    DBHandler.DeleteCourseAsync(course);
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response DeleteAllCourses()
        {
            Response response = new Response()
            {
                Success = false
            };

            try
            {
                List<Course> courses = DBHandler.GetCoursesAsync().Result;
                foreach (var course in courses)
                {
                    DBHandler.DeleteCourseAsync(course);
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response UpdateUserCourse(string courseCode)
        {
            Response response = new Response()
            {
                Success = false
            };
            try
            {
                // Verificar si el curso ya esta registrado
                if (LoggedUser.CourseID1 == courseCode || LoggedUser.CourseID2 == courseCode || LoggedUser.CourseID3 == courseCode)
                {
                    throw new Exception("El curso ya esta registrado");
                }

                // Verificar si el usuario ya tiene 3 cursos registrados
                if (!string.IsNullOrEmpty(LoggedUser.CourseID1) && !string.IsNullOrEmpty(LoggedUser.CourseID2) && !string.IsNullOrEmpty(LoggedUser.CourseID3))
                {
                    throw new Exception("El usuario ya tiene 3 cursos registrados");
                }

                // registrar el curso en el usuario local
                if (string.IsNullOrEmpty(LoggedUser.CourseID1))
                {
                    LoggedUser.CourseID1 = courseCode;
                }
                else if (string.IsNullOrEmpty(LoggedUser.CourseID2))
                {
                    LoggedUser.CourseID2 = courseCode;
                }
                else if (string.IsNullOrEmpty(LoggedUser.CourseID3))
                {
                    LoggedUser.CourseID3 = courseCode;
                }

                // Actualizar el usuario en la base de datos
                int res = DBHandler.SaveUserAsync(LoggedUser).Result;
                if (res == 1)
                {
                    response.Success = true;
                    response.Message = "Curso registrado correctamente";
                }
                else
                {
                    throw new Exception("No se pudo registrar el curso");
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response DeleteUserCourseRegistered(string courseCode)
        {
            Response response = new Response()
            {
                Success = false
            };
            try
            {
                // Verificar si el curso ya esta registrado
                if (LoggedUser.CourseID1 == courseCode)
                {
                    LoggedUser.CourseID1 = "";
                }
                else if (LoggedUser.CourseID2 == courseCode)
                {
                    LoggedUser.CourseID2 = "";
                }
                else if (LoggedUser.CourseID3 == courseCode)
                {
                    LoggedUser.CourseID3 = "";
                }
                else
                {
                    throw new Exception("El curso no esta matriculado");
                }

                // Actualizar el usuario en la base de datos
                int res = DBHandler.SaveUserAsync(LoggedUser).Result;
                if (res == 1)
                {
                    response.Success = true;
                    response.Message = "Curso eliminado correctamente";
                }
                else
                {
                    throw new Exception("No se pudo eliminar el curso");
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public static Response DeleteAllUserCourseRegistered()
        {
            Response response = new Response()
            {
                Success = false
            };
            try
            {
                LoggedUser.CourseID1 = "";
                LoggedUser.CourseID2 = "";
                LoggedUser.CourseID3 = "";
                
                // Actualizar el usuario en la base de datos
                int res = DBHandler.SaveUserAsync(LoggedUser).Result;
                if (res == 1)
                {
                    response.Success = true;
                    response.Message = "Cursos eliminados correctamente";
                }
                else
                {
                    throw new Exception("No se pudo eliminar el curso");
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

    }
}
