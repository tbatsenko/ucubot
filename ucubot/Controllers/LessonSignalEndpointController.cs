using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Database;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;
        
        private readonly LessonSignalRepository<LessonSignalDto> _repository;

        public LessonSignalEndpointController(IConfiguration configuration, LessonSignalRepository<LessonSignalDto> repository)
        {
            _configuration = configuration;
            _repository = _repository;
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

                return _repository.GetSignals(myConnection);
                   
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

                return _repository.GetSignal(myConnection, id);

            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
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

                var result = _repository.CreateSignal(myConnection, message);
                
                switch (result)
                {
                    case 400:
                        // Student with this ID doesn't exist
                        return BadRequest();
                    case 409:
                        // Student with this ID already has a record
                        return StatusCode(409);
                        
                    case 200:
                        return Accepted();
                        
                    default: StatusCode(result);
                        break;
                }

                return null;
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
                

                var result = _repository.DeleteSignal(myConnection, id);

                switch (result)
                {
                    case 400:
                        return BadRequest();
                    case 200:
                        return Accepted();
                        
                    default: StatusCode(result);
                        break;
                }
            }

            return null;
        }
    }
}
