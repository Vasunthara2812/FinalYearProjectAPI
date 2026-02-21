using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using GenerateCourse.Models;
using System.Data;

namespace GenerateCourse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneratedCoursesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GeneratedCoursesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("MySqlConnection");
        }

        // ✅ GET: api/GeneratedCourses
        [HttpGet]
        public IActionResult GetAll()
        {
            List<GeneratedCourseJson> courses = new List<GeneratedCourseJson>();

            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT * FROM generated_courses_json";
                MySqlCommand cmd = new MySqlCommand(query, con);

                con.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new GeneratedCourseJson
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            RequirementId = Convert.ToInt32(reader["requirement_id"]),
                            CourseId = Convert.ToInt32(reader["course_id"]),
                            CourseName = reader["course_name"].ToString(),
                            CourseJson = reader["course_json"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }

            return Ok(courses);
        }

        // ✅ POST: api/GeneratedCourses
        [HttpPost]
        public IActionResult Create([FromBody] GeneratedCourseJson model)
        {
            using (MySqlConnection con = new MySqlConnection(GetConnectionString()))
            {
                string query = @"INSERT INTO generated_courses_json 
                                (requirement_id, course_id, course_name, course_json, created_at)
                                VALUES 
                                (@RequirementId, @CourseId, @CourseName, @CourseJson, @CreatedAt)";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@RequirementId", model.RequirementId);
                cmd.Parameters.AddWithValue("@CourseId", model.CourseId);
                cmd.Parameters.AddWithValue("@CourseName", model.CourseName);
                cmd.Parameters.AddWithValue("@CourseJson", model.CourseJson);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Record inserted successfully" });
        }
    }
}