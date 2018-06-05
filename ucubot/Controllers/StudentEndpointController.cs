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
    
    public class StudentEndpointController : Controller
    {
        
        private readonly IStudentRepository _repository;

        public StudentEndpointController(IStudentRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        public IEnumerable<Student> ShowRecords()
        {
            return _repository.GetStudents();          
        }

        [HttpGet("{id}")]
        public Student ShowRecord(int id)
        {
                return _repository.GetStudent(id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecord(Student student)
        {
            var res = _repository.Create(student);
            
            if (res == 409)
            {
                return StatusCode(409);
            }

            if (res == 400)
            {
                return BadRequest();
            }

            return Accepted();
            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRecord(Student student)
        {
            if (_repository.Update(student) == 400)
            {
                return BadRequest();
            }

            return Accepted();

        }
    

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
                if (_repository.Delete(id) == 409)
                {
                    return StatusCode(409);
                }

                return Accepted();
            
        }
    }
}