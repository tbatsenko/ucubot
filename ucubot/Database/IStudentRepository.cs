using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Database
{
    public interface IStudentRepository <T> where T : Student
    {
        IEnumerable<Student> GetStudents(MySqlConnection connection);

        Student GetStudent(MySqlConnection connection, int id);

        int Create(MySqlConnection connection, T entity);
        
        int Update(MySqlConnection connection, T entity);

        int Delete(MySqlConnection connection, long id);
    
    }
}