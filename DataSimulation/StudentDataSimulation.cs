using StudentApi.Models;

namespace StudentApi.DataSimulation
{
    public class StudentDataSimulation
    {

        public static readonly List<Student> StudentsList= new List<Student>
        {
            new Student { Id = 1, Name = "Albert Einstein", Age = 23,Grade=88 },
            new Student { Id = 2, Name = "Ahmad Abdullah", Age = 22,Grade=100 },
            new Student { Id = 3, Name = "Jon Snow", Age = 21 , Grade = 66},
            new Student { Id = 4, Name = "Micheal Scoffield", Age = 19,Grade=44 }
        };

    }
}
