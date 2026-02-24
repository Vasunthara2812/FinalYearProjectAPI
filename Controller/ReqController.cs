using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using FinalYearProjectAPI.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace FinalYearProjectAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReqController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ReqController(IConfiguration configuration)
        {
            _configuration   = configuration;
            _connectionString = configuration.GetConnectionString("MySqlConnection")!;
        }

        // GET: api/req
        [HttpGet]
        public async Task<IActionResult> GetAllRequirements(int userId)
        {
            var requirements = new List<RequirementDto>();

            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    r.RequirementId,
                    r.UserId,
                    r.Role,
                    r.Domain,
                    r.Interest,
                    r.SkillLevel,
                    r.Goal,
                    r.CourseGenerated,
                    g.course_id AS CourseId
                FROM requirements r
                LEFT JOIN generated_courses_json g 
                    ON r.RequirementId = g.requirement_id
                WHERE r.UserId = @UserId
                ORDER BY r.RequirementId DESC";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                requirements.Add(MapRequirement(reader));
            }

            return Ok(requirements);
        }

        // GET: api/req/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequirement(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = @"
                SELECT 
                    r.RequirementId,
                    r.UserId,
                    r.Role,
                    r.Domain,
                    r.Interest,
                    r.SkillLevel,
                    r.Goal,
                    r.CourseGenerated,
                    g.course_id AS CourseId
                FROM requirements r
                LEFT JOIN generated_courses_json g 
                    ON r.RequirementId = g.requirement_id
                WHERE r.RequirementId = @id
                LIMIT 1";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return NotFound(new { message = $"Requirement {id} not found." });

            return Ok(MapRequirement(reader));
        }

        // -------------------------------------------------------
        // Helper – maps a reader row → RequirementDto
        // Handles tinyint(1) → bool safely
        // -------------------------------------------------------
        private static RequirementDto MapRequirement(DbDataReader reader)
        {
            // tinyint(1) comes back as sbyte/byte/int depending on driver version
            // Convert.ToBoolean handles all numeric types correctly
            bool courseGenerated = Convert.ToBoolean(reader["CourseGenerated"]);

            return new RequirementDto
            {
                RequirementId   = reader.GetInt32("RequirementId"),
                UserId          = reader.IsDBNull(reader.GetOrdinal("UserId"))
                                    ? null
                                    : reader.GetInt32("UserId"),
                Role            = reader.GetString("Role"),
                Domain          = reader.GetString("Domain"),
                Interest        = reader.GetString("Interest"),
                SkillLevel      = reader.IsDBNull(reader.GetOrdinal("SkillLevel"))
                                    ? null
                                    : reader.GetString("SkillLevel"),
                Goal            = reader.IsDBNull(reader.GetOrdinal("Goal"))
                                    ? null
                                    : reader.GetString("Goal"),
                CourseGenerated = courseGenerated,
                CourseId        = reader.IsDBNull(reader.GetOrdinal("CourseId"))
                                    ? null
                                    : reader.GetInt32("CourseId")
            };
        }
    }
}