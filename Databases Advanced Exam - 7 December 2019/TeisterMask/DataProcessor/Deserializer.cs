namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using System.Xml.Serialization;
    using System.IO;
    using TeisterMask.DataProcessor.ImportDto;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Linq;
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";


        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ImportProjectDto[]), new XmlRootAttribute("Projects"));


             var projectDtos = (ImportProjectDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            //                        OR
            //using (StringReader stringReader = new StringReader(xmlString))
            //{
            //    var projectDtos = (ImportProjectDto[])xmlSerializer.Deserialize(stringReader); using goes up to Return Sb.
            //}


            List<Project> projects = new List<Project>();

          

            foreach (var projectDto in projectDtos)
            {
                if (!IsValid(projectDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime projectOpenDate;
               
                bool isProjectOpenDateValid = DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out projectOpenDate);

                if (!isProjectOpenDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
               
                
                DateTime? projectDueDate;

                if (!string.IsNullOrEmpty(projectDto.DueDate) && !string.IsNullOrWhiteSpace(projectDto.DueDate))
                {
                    projectDueDate = DateTime.ParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    projectDueDate = null;
                }


               

                Project validProject = new Project()
                {
                    Name = projectDto.Name,
                    OpenDate = projectOpenDate,
                    DueDate = projectDueDate
                };

                int taskCount = 0;

                foreach (var taskDto in projectDto.Tasks)
                {
                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime taskOpenDate;
                    bool isTaskOpenDateValid = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpenDate);

                    DateTime taskDueDate;
                    bool isTaskDueDateValid = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDueDate);

                    if (!isTaskOpenDateValid || !isTaskDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpenDate < projectOpenDate)
                    {
                       sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (projectDueDate.HasValue)
                    {
                        if (taskDueDate > projectDueDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                    }
                   

                    Task validTask = new Task()
                    {
                        Name = taskDto.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType,
                        Project = validProject
                    };

                    validProject.Tasks.Add(validTask);
                    taskCount++;
                }

                projects.Add(validProject);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, validProject.Name, validProject.Tasks.Count));
            }

            context.Projects.AddRange(projects);           
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportEmployeeDto[] employeeDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            var employees = new List<Employee>();

            foreach (var employeeDto in employeeDtos)
            {
                if (!IsValid(employeeDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }
                if (!IsUsernameValid(employeeDto.Username))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee()
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };

                

                foreach (var taskId in employeeDto.Tasks.Distinct())
                {
                    

                    Task realTask = context.Tasks.FirstOrDefault(t => t.Id == taskId);

                    if (realTask==null)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }
                    employee.EmployeesTasks.Add( new EmployeeTask()
                    {
                        Employee = employee,
                        Task = realTask
                    });

                   
                }
                employees.Add(employee);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }

            context.Employees.AddRange(employees);            
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }


        private static bool IsUsernameValid(string username)
        {
            foreach (char ch in username)
            {
                if (!Char.IsLetterOrDigit(ch))
                {
                    return false;
                }

            }

            return true;
        }



        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}