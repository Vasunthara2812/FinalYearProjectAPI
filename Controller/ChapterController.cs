using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/chapter")]
public class ChapterController : ControllerBase
{
    private readonly string _connectionString;

    public ChapterController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // GET: api/chapter/{courseId} - get all chapters for a course
    [HttpGet("{courseId}")]
    public IActionResult GetChapters(int courseId)
    {
        var chapters = new List<Chapter>();

        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            "SELECT chapter_id, course_id, chapter_name FROM chapters WHERE course_id = @id", con);
        cmd.Parameters.AddWithValue("@id", courseId);

        con.Open();
        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            chapters.Add(new Chapter
            {
                ChapterId = reader.GetInt32("chapter_id"),
                CourseId = reader.GetInt32("course_id"),
                ChapterName = reader.GetString("chapter_name")
            });
        }

        return Ok(chapters);
    }

    // POST: api/chapter - add a new chapter
    [HttpPost]
    public IActionResult AddChapter([FromBody] Chapter chapter)
    {
        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            "INSERT INTO chapters(course_id, chapter_name) VALUES(@courseId, @name)", con);

        cmd.Parameters.AddWithValue("@courseId", chapter.CourseId);
        cmd.Parameters.AddWithValue("@name", chapter.ChapterName);

        con.Open();
        cmd.ExecuteNonQuery();

        return Ok("Chapter added successfully");
    }
}
