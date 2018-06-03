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
        private readonly IConfiguration _configuration;
        
        private readonly IStudentRepository _repository;

        public StudentEndpointController(IConfiguration configuration, IStudentRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }
        
        [HttpGet]
        public IEnumerable<Student> ShowRecords()
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
                
                return _repository.GetStudents(myConnection);

            }

        }

        [HttpGet("{id}")]
        public Student ShowRecord(int id)
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

                // Write a query, which selects all data from the table lesson signal
                //                                                             and stores it in a DataTable object
                
                return _repository.GetStudent(myConnection, id);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecord(Student student)
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
                    return BadRequest();
                }

                /*
                    Implement function to update single record. Use attribute [HttpPut].
                    Use class Student as an argument. Use student.Id as update WHERE predicate.
                */

                if (_repository.Create(myConnection, student) == 409)
                {
                    return StatusCode(409);
                }

                return Accepted();
            }
            
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateRecord(Student student)
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
                    return BadRequest();
                }


                if (_repository.Update(myConnection, student) == 400)
                {
                    return BadRequest();
                }

                return Accepted();

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
                    return BadRequest();
                }

                if (_repository.Delete(myConnection, id) == 409)
                {
                    return StatusCode(409);
                }

                return Accepted();
            }
        }
    }
}