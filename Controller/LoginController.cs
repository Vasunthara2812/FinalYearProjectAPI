using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using LearningApi.Models;

namespace LearningApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password required");

            string connStr = _configuration.GetConnectionString("MySqlConnection");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string loginQuery = @"SELECT Id, UserName
                                FROM users
                                WHERE UserName = @u AND PassWord = @p
                                LIMIT 1";

            using var cmd = new MySqlCommand(loginQuery, conn);
            cmd.Parameters.AddWithValue("@u", request.Username.Trim());
            cmd.Parameters.AddWithValue("@p", request.Password.Trim());

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return Ok(new
                {
                    success = true,
                    userId = reader.GetInt32("Id"),
                    username = reader.GetString("UserName")
                });
            }

            return Unauthorized(new { success = false });
        }


        [HttpPost("signup")]
        public IActionResult Signup([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password required");

            string connStr = _configuration.GetConnectionString("MySqlConnection");

            using var conn = new MySqlConnection(connStr);
            conn.Open();
            string insertQuery = "INSERT INTO users(UserName, PassWord) VALUES(@u, @p)";
            using var insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@u", request.Username.Trim());
            insertCmd.Parameters.AddWithValue("@p", request.Password.Trim());
            insertCmd.ExecuteNonQuery();
        

            // 3️⃣ Validate login (after insert or if already exists)
            string loginQuery = "SELECT COUNT(*) FROM users WHERE UserName=@u AND PassWord=@p";
            using var loginCmd = new MySqlCommand(loginQuery, conn);
            loginCmd.Parameters.AddWithValue("@u", request.Username.Trim());
            loginCmd.Parameters.AddWithValue("@p", request.Password.Trim());
            int count = Convert.ToInt32(loginCmd.ExecuteScalar());

            if (count == 1)
                return Ok(new { success = true });

            return StatusCode(500, new { success = false });
        }
        [HttpPost("savetraining")]
        public IActionResult SaveTraining([FromBody] TrainingRequirement model)
        {
            if (string.IsNullOrEmpty(model.Role) ||
                string.IsNullOrEmpty(model.Domain) ||
                string.IsNullOrEmpty(model.Interest))
            {
                return BadRequest("Required fields missing");
            }

            string connStr = _configuration.GetConnectionString("MySqlConnection");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = @"INSERT INTO training_requirements
                             (Role, Domain, Interest, SkillLevel, Goal)
                             VALUES (@r, @d, @i, @s, @g)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@r", model.Role);
            cmd.Parameters.AddWithValue("@d", model.Domain);
            cmd.Parameters.AddWithValue("@i", model.Interest);
            cmd.Parameters.AddWithValue("@s", model.SkillLevel);
            cmd.Parameters.AddWithValue("@g", model.Goal);

            cmd.ExecuteNonQuery();

            return Ok(new { success = true });
        }
        [HttpPost("savecourse")]
        public IActionResult SaveCourse([FromBody] Courses model)
        {
            int requirementid = 13;
            string connStr = _configuration.GetConnectionString("MySqlConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = @"INSERT INTO course (CourseName, requirement_id)
                             VALUES (@name, @requirementid)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", model.CourseName);
            cmd.Parameters.AddWithValue("@requirementid", requirementid);
            cmd.ExecuteNonQuery();

            return Ok();
        }
        


    }
}
