using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/result")]
public class ResultController : ControllerBase
{
    private readonly string _connectionString;

    public ResultController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // GET: api/result/course/1
    [HttpGet("course/{courseId}")]
    public IActionResult GetResultsByCourse(int courseId)
    {
        var results = new List<Result>();

        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            @"SELECT result_id, course_id, chapter_id, subtopic_id, result 
              FROM results 
              WHERE course_id = @courseId", con);

        cmd.Parameters.AddWithValue("@courseId", courseId);

        con.Open();
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            results.Add(new Result
            {
                ResultId = reader.GetInt32("result_id"),
                CourseId = reader.GetInt32("course_id"),
                ChapterId = reader.GetInt32("chapter_id"),
                SubtopicId = reader.GetInt32("subtopic_id"),
                ResultText = reader.GetString("result")
            });
        }

        return Ok(results);
    }

    // POST: api/result
    [HttpPost]
    public IActionResult AddResult([FromBody] Result result)
    {
        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            @"INSERT INTO results (course_id, chapter_id, subtopic_id, result)
              VALUES (@courseId, @chapterId, @subtopicId, @resultText)", con);

        cmd.Parameters.AddWithValue("@courseId", result.CourseId);
        cmd.Parameters.AddWithValue("@chapterId", result.ChapterId);
        cmd.Parameters.AddWithValue("@subtopicId", result.SubtopicId);
        cmd.Parameters.AddWithValue("@resultText", result.ResultText);

        con.Open();
        cmd.ExecuteNonQuery();

        return Ok("Result added successfully");
    }
}
