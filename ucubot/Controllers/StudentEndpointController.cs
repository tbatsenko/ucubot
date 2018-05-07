using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

                return  myConnection.Query<Student>("SELECT id Id, first_name FirstName, second_name LastName, user_id UserId FROM student;").ToList();

            }

        }

        [HttpGet("{id}")]
        public Student ShowRecord(int id)
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

                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

                var queryResult = myConnection.Query<Student>("SELECT id Id, first_name FirstName, second_name LastName, user_id UserId FROM student WHERE id=" + id + ";").ToList();

                return queryResult.Count > 0 ? queryResult[0] : null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecord(Student student)
        {
            var userId = student.UserId;
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
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=?userID;" , myConnection);
                check_UserId.Parameters.AddWithValue("?userID", userId);

                var UserExist = (long) check_UserId.ExecuteScalar();


                if(UserExist > 0)
                {
                    // Student with this userID already exist
                    return StatusCode(409);
                }

                // Student with this userID doesn't exist
                string mysqlCmdString =
                    "INSERT INTO student (first_name, second_name, user_id) VALUES (?param2, ?param3, ?param4);";
                var cmd = new MySqlCommand(mysqlCmdString, myConnection);

                cmd.Parameters.Add("?param2", MySqlDbType.VarChar).Value = firstName;
                cmd.Parameters.Add("?param3", MySqlDbType.VarChar).Value = lastName;
                cmd.Parameters.Add("?param4", MySqlDbType.VarChar).Value = userId;
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
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=(@userID);" , myConnection);
                check_UserId.Parameters.AddWithValue("@userID", userId);

                var UserExist = (long) check_UserId.ExecuteScalar();


                if(UserExist > 0)
                {
                    // Student with this userID already exist
                    var mysqlCmdString = "UPDATE student SET id=@id, first_name=@first_name, second_name=@last_name, user_id=@user_id WHERE id=@that_id;";  
                    var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    cmd.Parameters.Add("@that_id", MySqlDbType.Int32).Value = id;
                    cmd.Parameters.Add("@first_name", MySqlDbType.VarChar).Value = firstName;
                    cmd.Parameters.Add("@last_name", MySqlDbType.VarChar).Value = lastName;
                    cmd.Parameters.Add("@user_id", MySqlDbType.VarChar).Value = userId;
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
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE id=?Id;" , myConnection);
                check_UserId.Parameters.AddWithValue("?Id", id);

                var UserExist = (long) check_UserId.ExecuteScalar();
                
                var check_UserId2 = new MySqlCommand("SELECT COUNT(*) FROM student WHERE id=?Id;" , myConnection);
                check_UserId2.Parameters.AddWithValue("?Id", id);

                var UserExist2 = (long) check_UserId.ExecuteScalar();


                if(UserExist > 0)
                {
                    var myCommand = new MySqlCommand("DELETE FROM student WHERE id=" + id + ";", myConnection);
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch
                    {
                        return StatusCode(409);
                    }
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