using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Database
{
    public interface ILessonSignalRepository <T> where T : LessonSignalDto
    {
        IEnumerable<LessonSignalDto> GetSignals(MySqlConnection connection);

        LessonSignalDto GetSignal(MySqlConnection connection, long id);

        int CreateSignal(MySqlConnection connection, SlackMessage message);

        int DeleteSignal(MySqlConnection connection, long id);
    
    }
}