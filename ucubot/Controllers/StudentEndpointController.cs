using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class StudentEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet]
        public IEnumerable<Student> ShowRecords()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                // Write a query, which selects all data from the table lesson signal
                //                                                             and stores it in a DataTable object
               // conn is a SqlConnection
                var queryResult = myConnection.Query<Student>("SELECT [Id], [FirstName],[LastName], [UserId] FROM student");

                return queryResult;
            }

        }

        [HttpGet("{id}")]
        public Student ShowRecord(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                // Write a query, which selects all data from the table lesson signal
                //                                                             and stores it in a DataTable object

                var queryResult = myConnection.Query<Student>("SELECT [Id], [FirstName],[LastName], [UserId] FROM student WHERE id=" + id);

                
                return queryResult.AsList()[0];
               
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecord(Student student)
        {
            var userId = student.UserId;
            var id = student.Id;
            var firstName = student.FirstName;
            var lastName = student.LastName;

            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return BadRequest();
                }
                
                /*
                    Implement function to update single record. Use attribute [HttpPut].
                    Use class Student as an argument. Use student.Id as update WHERE predicate.
                */
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=(@userID)" , myConnection);
                check_UserId.Parameters.AddWithValue("@userID", userId);
                var UserExist = (int)check_UserId.ExecuteScalar();

                if(UserExist > 0)
                {
                    // Student with this userID already exist
                    return StatusCode(409);
                }

                // Student with this userID doesn't exist
                const string mysqlCmdString =
                    "INSERT INTO student (user_id, SignalType) VALUES (@id, @first_name, @last_name, @user_id)";
                var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = userId;
                cmd.Parameters.Add("@first_name", MySqlDbType.Text).Value = firstName;
                cmd.Parameters.Add("@last_name", MySqlDbType.Text).Value = lastName;
                cmd.Parameters.Add("@user_id", MySqlDbType.Text).Value = userId;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();   
            }
            return Accepted();
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateRecord(Student student)
        {
            var userId = student.UserId;
            var id = student.Id;
            var firstName = student.FirstName;
            var lastName = student.LastName;

            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return BadRequest();
                }
                
                /*
                    Implement function to update single record. Use attribute [HttpPut].
                    Use class Student as an argument. Use student.Id as update WHERE predicate.
                */
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=(@userID)" , myConnection);
                check_UserId.Parameters.AddWithValue("@userID", userId);
                var UserExist = (int)check_UserId.ExecuteScalar();

                if(UserExist > 0)
                {
                    // Student with this userID already exist
                    var mysqlCmdString = "UPDATE student SET id=(@id), first_name=(@first_name), last_name=(@last_name), user_id=(@user_id) WHERE id=" + id;  
                    var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = userId;
                    cmd.Parameters.Add("@first_name", MySqlDbType.Text).Value = firstName;
                    cmd.Parameters.Add("@last_name", MySqlDbType.Text).Value = lastName;
                    cmd.Parameters.Add("@user_id", MySqlDbType.Text).Value = userId;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();   
                    return Accepted();
                }

   
                 return BadRequest();

                // Student with this userID doesn't exist

            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            //TODO: add code to delete a record with the given id
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            using (var myConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    myConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return BadRequest();
                }
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE id=(@Id)" , myConnection);
                check_UserId.Parameters.AddWithValue("@Id", id);
                var UserExist = (int)check_UserId.ExecuteScalar();

                if(UserExist > 0)
                {
                    var myCommand = new MySqlCommand("DELETE FROM  WHERE Id=" + id + ";", myConnection);
                    myCommand.ExecuteNonQuery();
                }
                else
                {
                    return StatusCode(409);
                }

                return Accepted();
            }
        }
    }
}
