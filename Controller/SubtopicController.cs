using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/subtopic")]
public class SubtopicController : ControllerBase
{
    private readonly string _connectionString;

    public SubtopicController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // GET: api/subtopic/chapter/1
    [HttpGet("chapter/{chapterId}")]
    public IActionResult GetSubtopicsByChapter(int chapterId)
    {
        var subtopics = new List<Subtopic>();

        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            "SELECT subtopic_id, course_id, chapter_id, subtopic_name FROM subtopics WHERE chapter_id = @id",
            con);

        cmd.Parameters.AddWithValue("@id", chapterId);

        con.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            subtopics.Add(new Subtopic
            {
                SubtopicId = reader.GetInt32("subtopic_id"),
                CourseId = reader.GetInt32("course_id"),
                ChapterId = reader.GetInt32("chapter_id"),
                SubtopicName = reader.GetString("subtopic_name")
            });
        }

        return Ok(subtopics);
    }

    // POST: api/subtopic
    [HttpPost]
    public IActionResult AddSubtopic([FromBody] Subtopic subtopic)
    {
        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            @"INSERT INTO subtopics (course_id, chapter_id, subtopic_name)
              VALUES (@courseId, @chapterId, @name)", con);

        cmd.Parameters.AddWithValue("@courseId", subtopic.CourseId);
        cmd.Parameters.AddWithValue("@chapterId", subtopic.ChapterId);
        cmd.Parameters.AddWithValue("@name", subtopic.SubtopicName);

        con.Open();
        cmd.ExecuteNonQuery();

        return Ok("Subtopic added successfully");
    }
}
