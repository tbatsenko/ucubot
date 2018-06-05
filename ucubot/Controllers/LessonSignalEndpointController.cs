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
        
        private readonly ILessonSignalRepository _repository;

        public LessonSignalEndpointController(ILessonSignalRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            return _repository.GetSignals();
        }

        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            return _repository.GetSignal(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var result = _repository.CreateSignal(message);
            
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            var result = _repository.DeleteSignal(id);

            switch (result)
            {
                case 400:
                    return BadRequest();
                case 200:
                    return Accepted();

                default:
                    StatusCode(result);
                    break;
            }

            return null;
        }
    }
}
