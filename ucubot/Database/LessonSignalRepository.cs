using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;



namespace ucubot.Database
{
    public abstract class LessonSignalRepository<T> : ILessonSignalRepository<T> where T : LessonSignalDto
    {
        public IEnumerable<LessonSignalDto> GetSignals(MySqlConnection connection)
        {
            return connection.Query<LessonSignalDto>("SELECT lesson_signal.Id Id, lesson_signal.Timestamp Timestamp, lesson_signal.SignalType Type, student.user_id UserId FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id);").ToList();
        }
        

        public LessonSignalDto GetSignal(MySqlConnection connection, long id)
        {
            var queryResult = connection.Query<LessonSignalDto>("SELECT lesson_signal.Id Id, lesson_signal.Timestamp Timestamp, lesson_signal.SignalType Type, student.user_id UserId  FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id) WHERE lesson_signal.Id=" + id + ";").ToList();

                
            return queryResult.Count > 0 ? queryResult[0] : null;
        }

        
        public int CreateSignal(MySqlConnection connection, SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = (int) message.text.ConvertSlackMessageToSignalType();


            
            var checkUserId = new MySqlCommand("SELECT COUNT(*) FROM student WHERE user_id=?userID;" , connection);
            checkUserId.Parameters.AddWithValue("?userID", userId);

            var userExist = (long) checkUserId.ExecuteScalar();


            if(userExist < 1)
            {
                // Student with this ID doesn't exist
                return 400;
            }
            
            var getStudentId = new MySqlCommand("SELECT id FROM student WHERE user_id=?userID;" , connection);
            getStudentId.Parameters.AddWithValue("?userID", userId);
            
            var studentId = getStudentId.ExecuteScalar();
            
            
            var checkUserId2 = new MySqlCommand("SELECT COUNT(*) FROM lesson_signal WHERE student_id=?student_id;" , connection);
            checkUserId2.Parameters.AddWithValue("?student_id", studentId);

            var userExist2 = (long) checkUserId2.ExecuteScalar();

            if(userExist2 > 0)
            {
                // Student with this ID already has a record
                return 409;
            }

            // Student with this ID exist
            const string mysqlCmdString =
                "INSERT INTO lesson_signal (student_id, SignalType) VALUES (?param1, ?param2);";
            var cmd = new MySqlCommand(mysqlCmdString, connection);
            cmd.Parameters.Add("?param1", MySqlDbType.Int32).Value = studentId;
            cmd.Parameters.Add("?param2", MySqlDbType.Int32).Value = (int) signalType;
            cmd.ExecuteNonQuery();  
            
            return 200;
        }
        

        public int DeleteSignal(MySqlConnection connection, long id)
        {
            var checkUserId = new MySqlCommand
            (
                "SELECT COUNT(*) FROM lesson_signal LEFT JOIN student ON (lesson_signal.student_id = student.id) WHERE lesson_signal.Id=@ID;",
                connection
            );
            
            checkUserId.Parameters.AddWithValue("@ID", id);
            
            var userExist = (long) checkUserId.ExecuteScalar();

            if(userExist < 1)
            {
                // Student with this ID doesn't exist
                return 400;
            }
                
 
            var myCommand = new MySqlCommand("DELETE FROM lesson_signal WHERE Id=" + id + ";", connection);

            myCommand.ExecuteNonQuery();

            return 200;
        }
    }
    

}