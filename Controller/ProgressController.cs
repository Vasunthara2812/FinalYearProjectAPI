using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalYearProjectAPI.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace FinalYearProjectAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly string _conn;

        public ProgressController(IConfiguration config)
        {
            _conn = config.GetConnectionString("MySqlConnection")!;
        }

        // GET api/Progress/{userId}
        [HttpGet("{userId}")]
        public IActionResult GetAll(string userId)
        {
            var result = new Dictionary<string, bool>();
            using var con = new MySqlConnection(_conn);
            con.Open();
            using var cmd = new MySqlCommand(
                "SELECT subtopic_key, is_done FROM user_progress WHERE user_id = @uid", con);
            cmd.Parameters.AddWithValue("@uid", userId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                result[rdr.GetString("subtopic_key")] = rdr.GetBoolean("is_done");
            return Ok(result);
        }

        // POST api/Progress
        // Body: { "userId": "...", "subtopicKey": "...", "isDone": true/false }
        [HttpPost]
        public IActionResult Upsert([FromBody] UserProgress model)
        {
            using var con = new MySqlConnection(_conn);
            con.Open();
            using var cmd = new MySqlCommand(@"
                INSERT INTO user_progress (user_id, subtopic_key, is_done)
                VALUES (@uid, @key, @done)
                ON DUPLICATE KEY UPDATE is_done = @done, updated_at = NOW()", con);
            cmd.Parameters.AddWithValue("@uid", model.UserId);
            cmd.Parameters.AddWithValue("@key", model.SubtopicKey);
            cmd.Parameters.AddWithValue("@done", model.IsDone ? 1 : 0);
            cmd.ExecuteNonQuery();
            return Ok();
        }
    }
}