using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/requirement")]
public class RequirementController : ControllerBase
{
    private readonly string _connectionString;

    public RequirementController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // GET: api/requirement/user/1
    [HttpGet("user/{userId}")]
    public IActionResult GetRequirementsByUser(int userId)
    {
        var requirements = new List<Requirement>();

        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            @"SELECT RequirementId, UserId, Role, Domain, Interest, SkillLevel, Goal
              FROM requirements
              WHERE UserId = @userId", con);

        cmd.Parameters.AddWithValue("@userId", userId);

        con.Open();
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            requirements.Add(new Requirement
            {
                RequirementId = reader.GetInt32("RequirementId"),
                UserId = reader.GetInt32("UserId"),
                Role = reader.GetString("Role"),
                Domain = reader.GetString("Domain"),
                Interest = reader.GetString("Interest"),
                SkillLevel = reader["SkillLevel"] as string,
                Goal = reader["Goal"] as string,
                
            });
        }

        return Ok(requirements);
    }

    // POST: api/requirement
    [HttpPost]
    public IActionResult AddRequirement([FromBody] Requirement requirement)
    {
        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            @"INSERT INTO requirements 
              (UserId, Role, Domain, Interest, SkillLevel, Goal)
              VALUES 
              (@userId, @role, @domain, @interest, @skillLevel, @goal)", con);

        cmd.Parameters.AddWithValue("@userId", requirement.UserId);
        cmd.Parameters.AddWithValue("@role", requirement.Role);
        cmd.Parameters.AddWithValue("@domain", requirement.Domain);
        cmd.Parameters.AddWithValue("@interest", requirement.Interest);
        cmd.Parameters.AddWithValue("@skillLevel", requirement.SkillLevel);
        cmd.Parameters.AddWithValue("@goal", requirement.Goal);

        con.Open();
        cmd.ExecuteNonQuery();

        return Ok("Requirement added successfully");
    }
}
