using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Database
{
    public interface ISignalRepository <T> where T : StudentSignal
    {
        IEnumerable<StudentSignal> GetSignals(MySqlConnection connection);
    }
}