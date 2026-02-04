using StudentApi.Models;

namespace StudentApi.DataSimulation
{
    public class StudentDataSimulation
    {

        public static readonly List<Student> StudentsList= new List<Student>
        {
            new Student { Id = 1, Name = "Albert Einstein", Age = 23,Grade=88,Email = "albert@einstein.com",PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass1"),Role = "Student" },
            new Student { Id = 2, Name = "Ahmad Abdullah", Age = 22,Grade=100,Email = "ahmad@abdullah.com",PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass2"),Role = "Admin" },
            new Student { Id = 3, Name = "John Snow", Age = 21 , Grade = 66,Email = "john@snow.com",PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass3"),Role = "Student"},
            new Student { Id = 4, Name = "Micheal Scoffield", Age = 19,Grade=44,Email = "micheal@scoffield.com",PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass4"),Role = "Student" }
        };

    }
}
