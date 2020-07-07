using System;
using System.Text;
using System.Linq;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            string result = DeleteProjectById(context);
            Console.WriteLine(result);
        }
        //Problem 03:
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees.Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary

            }).ToList();


            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
            }
            return sb.ToString().TrimEnd();

        }

        //Problem 04:
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees.Where(e => e.Salary > 50000).Select(e => new
            {
                e.FirstName,
                e.Salary

            }).OrderBy(e => e.FirstName).ToList();


            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
            }
            return sb.ToString().TrimEnd();

        }

        //Problem 05:
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary

                }).OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName).ToList();


            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 06:
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address newAdress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            Employee employeeNakov = context.Employees.First(e => e.LastName == "Nakov");

            employeeNakov.Address = newAdress;
            context.SaveChanges();

            var adresses = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList();

            foreach (var a in adresses)
            {
                sb.AppendLine(a);
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 07:
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeesProjects
            .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Project = e.EmployeesProjects
                           .Select(ep => new
                           {
                               ProjectName = ep.Project.Name,
                               StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                               EndDate = ep.Project.EndDate.HasValue ? ep.Project
                               .EndDate
                               .Value
                               .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"
                           })
                           .ToList()
                }).ToList();


            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Project)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();


        }

        //Problem 08:
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var adresses = context
                .Addresses
                .OrderByDescending(a => (a.Employees).Count)
                .ThenBy(a=>a.Town.Name)
                .ThenBy(a=>a.AddressText)
                .Take(10)
                .Select(a => new
                {
                    a.AddressText,
                    a.Town.Name,
                    a.Employees.Count
                })
                .ToList();

            foreach (var a in adresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.Name} - {a.Count} employees");
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 09:
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects=e.EmployeesProjects.Select(ep => new
                    {
                        ep.Project.Name
                    }).OrderBy(ep=>ep.Name)
                    .ToList()

                }).ToList();

           

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                foreach (var p in e.Projects)
                {
                    sb.AppendLine(p.Name);
                }
            }


            return sb.ToString().TrimEnd();
        }


        //Problem 10:
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();


            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employee = d.Employees.Select(de => new
                    {
                        de.FirstName,
                        de.LastName,
                        de.JobTitle
                    }).OrderBy(de=>de.FirstName)
                    .ThenBy(de=>de.LastName)
                    .ToList()
                })
                .ToList();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");
                foreach (var e in d.Employee)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 11:
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context
                .Projects                
                .OrderByDescending(p => p.StartDate)                                          
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                }).OrderBy(p=>p.Name)
                .ThenByDescending(p=>p.Name.Length).ToList();


            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }


            return sb.ToString().TrimEnd();
        }

        //Problem 12:
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" || e.Department.Name == "Marketing" || e.Department.Name == "Information Services");
                

            foreach (var e in employees)
            {
                e.Salary= e.Salary * 1.12M;              
            }
            context.SaveChanges();

            var employeesData=employees.Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Salary
            })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employeesData)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }
            return sb.ToString().TrimEnd();
        }


        //Problem 13:
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))             
                .Select(e=> new 
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e=>e.FirstName)
                .ThenBy(e=>e.LastName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }
            return sb.ToString().TrimEnd();

        }

        //Problem 14:
        public static string DeleteProjectById(SoftUniContext context)
        {
            var deletingIdFromEmployeeProjects = context
               .EmployeesProjects
               .Where(x => x.ProjectId == 2)
               .ToList();

            context.EmployeesProjects.RemoveRange(deletingIdFromEmployeeProjects);

            var deletingIdFromProjects = context
                .Projects
                .Where(x => x.ProjectId == 2)
                .FirstOrDefault();

            context.Projects.RemoveRange(deletingIdFromProjects);
            
            StringBuilder sb = new StringBuilder();
                   
            context.SaveChanges();

            var projects = context.Projects        
                .Select(p=>p.Name)
                .Take(10)
                .ToList();
            
            foreach (var p in projects)
            {
                sb.AppendLine(p);
            }
            return sb.ToString().TrimEnd();
        }

        //Problem 15:
        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDel = context.Towns.First(t => t.Name == "Seattle");

            IQueryable<Address> addressesToDel = context.Addresses.Where(a => a.TownId == townToDel.TownId);


            int addressesCount = addressesToDel.Count();
            IQueryable<Employee> employeesOnDelAddreses = context
                .Employees
                .Where(e => addressesToDel
                .Any(a => a.AddressId == e.AddressId));

            foreach (var e in employeesOnDelAddreses)
            {
                e.AddressId = null;
            }
            foreach (var a in addressesToDel)
            {
                context.Addresses.Remove(a);
            }

            context.Towns.Remove(townToDel);
            context.SaveChanges();

            return $"{addressesCount} addresses in Seattle were deleted";
        }
    }

}
