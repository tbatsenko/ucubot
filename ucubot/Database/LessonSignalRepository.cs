using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace ucubot.Database
{
    public class LessonSignalRepository : ILessonSignalRepository
    {
        private readonly IConfiguration _configuration;
        
        public LessonSignalRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IEnumerable<LessonSignalDto> GetSignals()
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

                return myConnection.Query<LessonSignalDto>(
                        "SELECT lesson_signal.Id Id, lesson_signal.Timestamp Timestamp, lesson_signal.SignalType Type, student.user_id UserId FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id);")
                    .ToList();
            }
        }




        public LessonSignalDto GetSignal(long id)
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

        
        public int CreateSignal(SlackMessage message)
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
                
                var userId = message.user_id;
                var signalType = (int) message.text.ConvertSlackMessageToSignalType();


            
                var checkUserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=?userID;" , myConnection);
                checkUserId.Parameters.AddWithValue("?userID", userId);

                var userExist = (long) checkUserId.ExecuteScalar();


                if(userExist < 1)
                {
                    // Student with this ID doesn't exist
                    return 400;
                }
            
                var getStudentId = new MySqlCommand("SELECT id FROM student WHERE user_id=?userID;" , myConnection);
                getStudentId.Parameters.AddWithValue("?userID", userId);
            
                var studentId = getStudentId.ExecuteScalar();
            

                // Student with this ID exist
                const string mysqlCmdString =
                    "INSERT INTO lesson_signal (student_id, SignalType) VALUES (?param1, ?param2);";
                var cmd = new MySqlCommand(mysqlCmdString, myConnection);
                cmd.Parameters.Add("?param1", MySqlDbType.Int32).Value = studentId;
                cmd.Parameters.Add("?param2", MySqlDbType.Int32).Value = (int) signalType;
                cmd.ExecuteNonQuery();

                return 200;
            }
        }
        

        public int DeleteSignal(long id)
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
                

                var checkUserId = new MySqlCommand
                (
                    "SELECT COUNT(*) FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id) WHERE lesson_signal.Id=@ID;", myConnection
                );
            
                checkUserId.Parameters.AddWithValue("@ID", id);
            
                var userExist = (long) checkUserId.ExecuteScalar();

                if(userExist < 1)
                {
                    // Student with this ID doesn't exist
                    return 400;
                }
                
 
                var myCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE Id=" + id + ";", myConnection);

                myCommand.ExecuteNonQuery();

                return 200;
            }
        }
    }
    

}