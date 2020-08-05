namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using TeisterMask.DataProcessor.ImportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ExportProjectsDto[]), new XmlRootAttribute("Projects"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

         
            
            //using (StringWriter stringWriter = new StringWriter(sb))
           // {



                var projectsDto = context
                    .Projects
                    .ToArray()
                    .Where(p => p.Tasks.Any())

                    .Select(p => new ExportProjectsDto
                    {
                        TasksCount = p.Tasks.Count,
                        ProjectName = p.Name,
                        HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                        Tasks = p.Tasks.Select(t => new UserTasksDto
                        {
                            Name = t.Name,
                            Label = t.LabelType.ToString()
                        })
                        .OrderBy(t => t.Name)
                        .ToArray()

                    })
                    .OrderByDescending(t => t.TasksCount)
                    .ThenBy(t => t.ProjectName)
                    .ToArray();

            // xmlSerializer.Serialize(stringWriter, projectsDto, namespaces);

            //  }
            //OR
            xmlSerializer.Serialize(new StringWriter(sb), projectsDto, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {

            var employees = context
                  .Employees
                  .ToArray()
                  .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                  .Select(e => new
                  {
                      Username = e.Username,
                      Tasks = e.EmployeesTasks
                      .Where(et=> et.Task.OpenDate>= date)
                      .OrderByDescending(et => et.Task.DueDate)
                      .ThenBy(et => et.Task.Name)
                     .Select(et => new
                     {
                         TaskName = et.Task.Name,
                         OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                         DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                         LabelType = et.Task.LabelType.ToString(),
                         ExecutionType = et.Task.ExecutionType.ToString()
                     })
                     .ToArray()
                  })
                  .OrderByDescending(e => e.Tasks.Length)
                  .ThenBy(e => e.Username)
                  .Take(10)
                  .ToArray() ;

            string json = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return json;
        }
    }
}