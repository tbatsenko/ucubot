using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Database
{
    public interface ILessonSignalRepository
    {
        IEnumerable<LessonSignalDto> GetSignals();

        LessonSignalDto GetSignal(long id);

        int CreateSignal(SlackMessage message);

        int DeleteSignal(long id);
    
    }
}