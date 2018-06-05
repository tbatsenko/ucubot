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
    public class SignalRepository : ISignalRepository
    {
        private readonly IConfiguration _configuration;
        
        public SignalRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<StudentSignal> GetSignals(MySqlConnection connection)
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

                return connection
                    .Query<StudentSignal>(
                        "SELECT student_signals.first_name first_name, student_signals.second_name LastName, student_signals.signal_type SignalType, student_signals.count Count FROM student_signals")
                    .ToList();
            }
        }

    }
}