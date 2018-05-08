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
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
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

                return myConnection.Query<LessonSignalDto>("SELECT lesson_signal.Id Id, lesson_signal.Timestamp Timestamp, lesson_signal.SignalType Type, student.user_id UserId FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id);").ToList();
            }

        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
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

                var queryResult = myConnection.Query<LessonSignalDto>("SELECT lesson_signal.Id Id, lesson_signal.Timestamp Timestamp, lesson_signal.SignalType Type, student.user_id UserId  FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id) WHERE lesson_signal.Id=" + id + ";").ToList();

                
                return queryResult.Count > 0 ? queryResult[0] : null;
               
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            int signalType = (int) message.text.ConvertSlackMessageToSignalType();


            //TODO: add code to store above values
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
                
//                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.user_id) WHERE student_id=@userID" , myConnection);
//                check_UserId.Parameters.AddWithValue("@userID", userId);
//                var UserExist = check_UserId.ExecuteScalar();

                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=?userID;" , myConnection);
                check_UserId.Parameters.AddWithValue("?userID", userId);

                long UserExist = (long) check_UserId.ExecuteScalar();


                if(UserExist < 1)
                {
                    // Student with this ID doesn't exist
                    return BadRequest();
                }
                
                var getStudentId = new MySqlCommand("SELECT id FROM student WHERE user_id=?userID;" , myConnection);
                getStudentId.Parameters.AddWithValue("?userID", userId);
                
                var student_id = getStudentId.ExecuteScalar();
                
                
                var check_UserId2 = new MySqlCommand("SELECT COUNT(*) FROM lesson_signal WHERE student_id=?student_id;" , myConnection);
                check_UserId2.Parameters.AddWithValue("?student_id", student_id);

                long UserExist2 = (long) check_UserId2.ExecuteScalar();

                if(UserExist2 > 0)
                {
                    // Student with this ID already has a record
                    return StatusCode(409);
                }

                // Student with this ID exist
                const string mysqlCmdString =
                    "INSERT INTO lesson_signal (student_id, SignalType) VALUES (?param1, ?param2);";
                var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                cmd.Parameters.Add("?param1", MySqlDbType.Int32).Value = student_id;
                cmd.Parameters.Add("?param2", MySqlDbType.Int32).Value = (int) signalType;
                cmd.ExecuteNonQuery();  
                
                return Accepted();
            }
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
  
                }
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id) WHERE lesson_signal.Id=@ID;" , myConnection);
                check_UserId.Parameters.AddWithValue("@ID", id);
                var UserExist = (long) check_UserId.ExecuteScalar();

                if(UserExist < 1)
                {
                    // Student with this ID doesn't exist
                    return BadRequest();
                }
                
 
                var myCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE Id=" + id + ";", myConnection);

                myCommand.ExecuteNonQuery();

                return Accepted();
            }
        }
    }
}
