using System;
using ucubot.Model;
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
using ucubot.Database;


namespace ucubot.Controllers
{
    [Route("api/[controller]")]

    public class StudentSignalsEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly SignalRepository<StudentSignal> _repository;

        public StudentSignalsEndpointController(IConfiguration configuration, SignalRepository<StudentSignal> repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<StudentSignal> ShowSignals()
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

                return _repository.GetSignals(myConnection);

            }

        }
    }
}