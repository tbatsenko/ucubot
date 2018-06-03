using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;



namespace ucubot.Database
{
    public abstract class SignalRepository<T> : ISignalRepository<T> where T : StudentSignal
    {
        public IEnumerable<StudentSignal> GetSignals(MySqlConnection connection)
        {
            return connection.Query<StudentSignal>("SELECT student_signals.Timestamp first_name, student_signals.last_name LastName, student_signals.signal_type  SignalType, student_signals.count Count FROM student_signals").ToList();            
        }
    }
}