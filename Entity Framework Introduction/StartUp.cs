using System;

using SoftUni.Data;
using System.Text;
using System.Linq;
using SoftUni.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace SoftUni
{
	public class StartUp
	{
		public static void Main(string[] args)
		{
			SoftUniContext context = new SoftUniContext();

			Console.WriteLine(RemoveTown(context));
		}

		// Problem 03
		public static string GetEmployeesFullInformation(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var employees = context
				.Employees
				.OrderBy(e => e.EmployeeId)
				.Select(e => new
				{
					e.FirstName,
					e.LastName,
					e.MiddleName,
					e.JobTitle,
					e.Salary
				});

			foreach (var e in employees)
			{
				sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 04
		public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var employees = context
				.Employees
				.OrderBy(e => e.FirstName)
				.Select(e => new
				{
					e.FirstName,
					e.Salary
				})
				.Where(s => s.Salary > 50000);

			foreach (var e in employees)
			{
				sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 05
		public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var employeesInDepartment = context
				.Employees
				.OrderBy(s => s.Salary)
				.ThenByDescending(e => e.FirstName)
				.Select(e => new
				{
					e.FirstName,
					e.LastName,
					e.Department.Name,
					e.Salary
				})
				.Where(d => d.Name == "Research and Development");

			foreach (var eid in employeesInDepartment)
			{
				sb.AppendLine($"{eid.FirstName} {eid.LastName} from {eid.Name} - ${eid.Salary:f2}");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 06
		public static string AddNewAddressToEmployee(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var newAddress = new Address()
			{
				AddressText = "Vitoshka 15",
				TownId = 4
			};

			context.Addresses.Add(newAddress);

			Employee nakov = context
				.Employees
				.First(e => e.LastName == "Nakov");

			nakov.Address = newAddress;

			context.SaveChanges();

			var employees = context
				.Employees
				.OrderByDescending(a => a.AddressId)
				.Select(e => e.Address.AddressText)
				.Take(10);

			foreach (var employee in employees)
			{
				sb.AppendLine(employee);
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 07
		public static string GetEmployeesInPeriod(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var certainEmployees = context
				.Employees
				.Where(e => e.EmployeesProjects
							.Any(ep => ep.Project.StartDate.Year >= 2001 &&
							ep.Project.StartDate.Year <= 2003))
				.Select(e => new
				{
					e.FirstName,
					e.LastName,
					ManagerFirstName = e.Manager.FirstName,
					ManagerLastName = e.Manager.LastName,
					Project = e.EmployeesProjects.Select(ep => new
					{
						ep.Project.Name,
						ep.Project.StartDate,
						ep.Project.EndDate
					})
				})
				.Take(10)
				.ToList();

			foreach (var employee in certainEmployees)
			{
				sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

				foreach (var project in employee.Project)
				{
					sb.AppendLine($"--{project.Name} - {project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - " +
						$"{(project.EndDate != null ? project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished")}");
				}
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 08
		public static string GetAddressesByTown(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var addresses = context
				.Addresses
				.Include(t => t.Town)
				.Include(e => e.Employees)
				.OrderByDescending(e => e.Employees.Count)
				.ThenBy(t => t.Town.Name)
				.ThenBy(a => a.AddressText)
				.Take(10)
				.ToList();

			foreach (var address in addresses)
			{
				sb.AppendLine($"{address.AddressText}, {address.Town.Name} - {address.Employees.Count} employees");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 09
		public static string GetEmployee147(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			Employee employee = context
				.Employees
				.Include(x => x.EmployeesProjects)
				.ThenInclude(x => x.Project)
				.First(e => e.EmployeeId == 147);


			sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

			foreach (var project in employee.EmployeesProjects.OrderBy(n => n.Project.Name))
			{
				sb.AppendLine(string.Join(Environment.NewLine, project.Project.Name));
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 10
		public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var departments = context
				.Departments
				.OrderBy(e => e.Employees.Count)
				.ThenBy(d => d.Name)
				.Where(e => e.Employees.Count > 5)
				.Select(m => new
				{
					m.Manager.FirstName,
					m.Manager.LastName,
					m.Manager.JobTitle,
					EmployeesInDepartment = m.Employees
					.Select(e => new
					{
						e.FirstName,
						e.LastName,
						e.JobTitle
					})
					.OrderBy(e => e.FirstName)
					.ThenBy(e => e.LastName)
				});


			foreach (var d in departments)
			{
				sb.AppendLine($"{d.JobTitle} - {d.FirstName} {d.LastName}");

				foreach (var eid in d.EmployeesInDepartment)
				{
					sb.AppendLine($"{eid.FirstName} {eid.LastName} - {eid.JobTitle}");
				}
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 11
		public static string GetLatestProjects(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var latestProjects = context
				.Projects
				.OrderByDescending(p => p.StartDate)
				.Take(10);

			var neededProjects = latestProjects
				.OrderBy(p => p.Name)
				.Select(p => new
				{
					p.Name,
					p.Description,
					p.StartDate
				});

			foreach (var project in neededProjects)
			{
				sb.AppendLine($"{project.Name}")
					.AppendLine($"{project.Description}")
					.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 12
		public static string IncreaseSalaries(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var employeesToIncreaseSalary = context
				.Employees
				.Where(d => d.Department.Name == "Engineering" ||
							d.Department.Name == "Tool Design" ||
							d.Department.Name == "Marketing" ||
							d.Department.Name == "Information Services");

			foreach (var emp in employeesToIncreaseSalary)
			{
				emp.Salary *= 1.12m;
			}
			context.SaveChanges();


			var printEmployees = employeesToIncreaseSalary
				.OrderBy(e => e.FirstName)
				.ThenBy(e => e.LastName)
				.Select(e => new
				{
					e.FirstName,
					e.LastName,
					e.Salary
				});

			foreach (var emp in printEmployees)
			{
				sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:f2})");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 13
		public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			var employees = context
				.Employees
				.OrderBy(e => e.FirstName)
				.ThenBy(e => e.LastName)
				.Where(e => e.FirstName.StartsWith("Sa"))
				.Select(e => new
				{
					e.FirstName,
					e.LastName,
					e.JobTitle,
					e.Salary
				})
				.ToList();

			foreach (var emp in employees)
			{
				sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:f2})");
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 14
		public static string DeleteProjectById(SoftUniContext context)
		{
			StringBuilder sb = new StringBuilder();

			Project project = context.Projects.Find(2);

			var projectsToDelete = context
				.EmployeesProjects
				.Where(p => p.ProjectId == 2)
				.ToList();

			context.EmployeesProjects.RemoveRange(projectsToDelete);

			context.Projects.Remove(project);
			context.SaveChanges();

			var projectsToPrint = context
				.Projects.Take(10);

			foreach (var proj in projectsToPrint)
			{
				sb.AppendLine(proj.Name);
			}

			return sb.ToString().TrimEnd();
		}

		// Problem 15
		public static string RemoveTown(SoftUniContext context)
		{
			Town seattle = context
				.Towns
				.First(t => t.Name == "Seattle");

			var addressesInTown = context
				.Addresses
				.Where(t => t.Town == seattle);

			var employeesToRemoveAddress = context
				.Employees
				.Where(a => addressesInTown.Contains(a.Address));

			foreach (var emp in employeesToRemoveAddress)
			{
				emp.AddressId = null;
			}

			context.Addresses.RemoveRange(addressesInTown);

			context.Towns.Remove(seattle);

			context.SaveChanges();

			return $"{addressesInTown.Count()} addresses in Seattle were deleted";
		}
	}
}
