using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;



namespace ucubot.Database
{
    public class StudentRepository : IStudentRepository
    {
        public IEnumerable<Student> GetStudents(MySqlConnection connection)
        {
            return  connection.Query<Student>("SELECT id Id, first_name FirstName, second_name LastName, user_id UserId FROM student;").ToList();
        }

        public Student GetStudent(MySqlConnection connection, int id)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            var queryResult = connection.Query<Student>("SELECT id Id, first_name FirstName, second_name LastName, user_id UserId FROM student WHERE id=" + id + ";").ToList();

            return queryResult.Count > 0 ? queryResult[0] : null;
        }

        public int Create(MySqlConnection connection, Student entity)
        {
            
            var userId = entity.UserId;
            var firstName = entity.FirstName;
            var lastName = entity.LastName;
            
            var checkUserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=?userID;" , connection);
            checkUserId.Parameters.AddWithValue("?userID", userId);

            var userExist = (long) checkUserId.ExecuteScalar();


            if(userExist > 0)
            {
                // Student with this userID already exist
                return 409;
            }

            // Student with this userID doesn't exist
            const string mysqlCmdString =
                "INSERT INTO student (first_name, second_name, user_id) VALUES (?param2, ?param3, ?param4);";
            var cmd = new MySqlCommand(mysqlCmdString, connection);

            cmd.Parameters.Add("?param2", MySqlDbType.VarChar).Value = firstName;
            cmd.Parameters.Add("?param3", MySqlDbType.VarChar).Value = lastName;
            cmd.Parameters.Add("?param4", MySqlDbType.VarChar).Value = userId;
            cmd.CommandType = CommandType.Text;

            cmd.ExecuteNonQuery();   
        
            return 200;
        }

        public int Update(MySqlConnection connection, Student entity)
        {
            var id = entity.Id;
            var userId = entity.UserId;
            var firstName = entity.FirstName;
            var lastName = entity.LastName;
            
            var checkUserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=(@userID);" , connection);
            checkUserId.Parameters.AddWithValue("@userID", userId);

            var userExist = (long) checkUserId.ExecuteScalar();


            if (userExist <= 0) return 400;
            
            // Student with this userID already exist
            const string mysqlCmdString = "UPDATE student SET id=@id, first_name=@first_name, second_name=@last_name, user_id=@user_id WHERE id=@that_id;";  
            
            var cmd = new MySqlCommand(mysqlCmdString, connection);
            
            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
            cmd.Parameters.Add("@that_id", MySqlDbType.Int32).Value = id;
            cmd.Parameters.Add("@first_name", MySqlDbType.VarChar).Value = firstName;
            cmd.Parameters.Add("@last_name", MySqlDbType.VarChar).Value = lastName;
            cmd.Parameters.Add("@user_id", MySqlDbType.VarChar).Value = userId;
            
            cmd.CommandType = CommandType.Text;
            
            cmd.ExecuteNonQuery();   
            return 200;
        }

        public int Delete(MySqlConnection connection, long id)
        {
            var checkUserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE id=?Id;" , connection);
            checkUserId.Parameters.AddWithValue("?Id", id);

            var userExist = (long) checkUserId.ExecuteScalar();
                
            var checkUserId2 = new MySqlCommand("SELECT COUNT(*) FROM student WHERE id=?Id;" , connection);
            checkUserId2.Parameters.AddWithValue("?Id", id);

            if(userExist > 0)
            {
                var myCommand = new MySqlCommand("DELETE FROM student WHERE id=" + id + ";", connection);
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                catch
                {
                    return 409;
                }
            }
            else
            {
                return 409;
            }

            return 200;
        }
    }
}