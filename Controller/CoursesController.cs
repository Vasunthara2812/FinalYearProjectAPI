using FinalYearProjectAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace FinalYearProjectAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
         private readonly IConfiguration _configuration;

        public CoursesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("by-user/{userId}")]
        public IActionResult GetCoursesByUserId(int userId)
        {
            List<CourseDto> courses = new List<CourseDto>();

            string connectionString = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT gc.course_id, gc.course_name
                    FROM generated_courses_json gc
                    INNER JOIN requirements r 
                        ON gc.requirement_id = r.RequirementId
                    WHERE r.UserId = @UserId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new CourseDto
                            {
                                CourseId = Convert.ToInt32(reader["course_id"]),
                                CourseName = reader["course_name"].ToString()
                            });
                        }
                    }
                }
            }

            return Ok(courses);
        }
    
    }
}