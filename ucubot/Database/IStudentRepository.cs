using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Database
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetStudents();

        Student GetStudent(int id);

        int Create(Student entity);
        
        int Update(Student entity);

        int Delete(long id);
    
    }
}