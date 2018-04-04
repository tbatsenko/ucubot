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
                var queryResult = myConnection.Query<LessonSignalDto>("SELECT [Timestamp], [SignalType],[student_id] FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.user_id)");

                return queryResult;
            }

        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");

            var dataTable = new DataTable();
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

                var queryResult = myConnection.Query<LessonSignalDto>("SELECT [Timestamp], [SignalType],[student_id] FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.user_id) WHERE Id=" + id);

                
                return queryResult.AsList()[0];
               
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();


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
                    return BadRequest();
                }
                
                var check_UserId = new MySqlCommand("SELECT COUNT(*) FROM lesson_signal WHERE student_id=(@userID)" , myConnection);
                check_UserId.Parameters.AddWithValue("@userID", userId);
                var UserExist = (int)check_UserId.ExecuteScalar();

                if(UserExist > 0)
                {
                    // Student with this ID already exist
                    return BadRequest();
                }

                // Student with this ID doesn't exist
                const string mysqlCmdString =
                    "INSERT INTO lesson_signal (user_id, SignalType) VALUES (@param1, @param2)";
                var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                cmd.Parameters.Add("@param1", MySqlDbType.Text).Value = userId;
                cmd.Parameters.Add("@param2", MySqlDbType.Int32).Value = signalType;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();   
            }
            return Accepted();
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
                
                var myCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE Id=" + id + ";", myConnection);

                myCommand.ExecuteNonQuery();

                return Accepted();
            }
        }
    }
}
