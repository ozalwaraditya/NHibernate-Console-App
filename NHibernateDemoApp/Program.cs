using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernateDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new Configuration();

            // Correct connection string format
            string connectionString = "Data Source=P2014-PC00300\\SQL01;Initial Catalog=NHibernate;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = connectionString;
                x.Driver<NHibernate.Driver.MicrosoftDataSqlClientDriver>();
                x.Dialect<MsSql2008Dialect>();
            });

            // Add assembly mappings
            cfg.AddAssembly(Assembly.GetExecutingAssembly());

            // Build session factory
            var sessionFactory = cfg.BuildSessionFactory();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("======== Student Management Menu ========");
                Console.WriteLine("1. Create New Student");
                Console.WriteLine("2. View All Students");
                Console.WriteLine("3. Update Student by ID");
                Console.WriteLine("4. Delete Student by ID");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateStudent(sessionFactory);
                        break;
                    case "2":
                        ViewAllStudents(sessionFactory);
                        break;
                    case "3":
                        UpdateStudentById(sessionFactory);
                        break;
                    case "4":
                        DeleteStudentById(sessionFactory);
                        break;
                    case "5":
                        Console.WriteLine("Exiting the program...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Please try again.");
                        break;
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        // Method to Create a New Student
        static void CreateStudent(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                Console.Write("Enter First Name: ");
                string firstName = Console.ReadLine();
                Console.Write("Enter Last Name: ");
                string lastName = Console.ReadLine();

                var newStudent = new Student
                {
                    FirstMidName = firstName,
                    LastName = lastName
                };

                using (var tx = session.BeginTransaction())
                {
                    session.Save(newStudent);
                    tx.Commit();
                    Console.WriteLine("New student added successfully!");
                }
            }
        }

        // Method to View All Students
        static void ViewAllStudents(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                IList<Student> students = session.CreateQuery("from Student").List<Student>();

                if (students.Count == 0)
                {
                    Console.WriteLine("No students found!");
                    return;
                }

                Console.WriteLine("\n==== Student List ====");
                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.ID}, Name: {student.FirstMidName} {student.LastName}");
                }
            }
        }

        // Method to Update a Student by ID
        static void UpdateStudentById(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                Console.Write("Enter the Student ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int studentId))
                {
                    Console.WriteLine("Invalid ID format!");
                    return;
                }

                var student = session.Get<Student>(studentId);
                if (student == null)
                {
                    Console.WriteLine("Student not found!");
                    return;
                }

                Console.WriteLine($"Current Name: {student.FirstMidName} {student.LastName}");
                Console.Write("Enter new First Name (or press Enter to keep current): ");
                string newFirstName = Console.ReadLine();
                Console.Write("Enter new Last Name (or press Enter to keep current): ");
                string newLastName = Console.ReadLine();

                using (var tx = session.BeginTransaction())
                {
                    if (!string.IsNullOrWhiteSpace(newFirstName))
                        student.FirstMidName = newFirstName;

                    if (!string.IsNullOrWhiteSpace(newLastName))
                        student.LastName = newLastName;

                    session.Update(student);
                    tx.Commit();
                    Console.WriteLine("Student updated successfully!");
                }
            }
        }

        // Method to Delete a Student by ID
        static void DeleteStudentById(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                Console.Write("Enter the Student ID to delete: ");
                if (!int.TryParse(Console.ReadLine(), out int studentId))
                {
                    Console.WriteLine("Invalid ID format!");
                    return;
                }

                var student = session.Get<Student>(studentId);
                if (student == null)
                {
                    Console.WriteLine("Student not found!");
                    return;
                }

                using (var tx = session.BeginTransaction())
                {
                    session.Delete(student);
                    tx.Commit();
                    Console.WriteLine("Student deleted successfully!");
                }
            }
        }
    }
}
