using LearningApi.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

[ApiController]
[Route("api/course")]
public class CourseController : ControllerBase
{
    private readonly string _connectionString;

    public CourseController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("MySqlConnection");
    }

    // GET: api/course
    // [HttpGet]
    // public IActionResult GetCourses()
    // {
    //     var courses = new List<Course>();

    //     using var con = new MySqlConnection(_connectionString);
    //     var cmd = new MySqlCommand("SELECT course_id, course_name, requirement_id FROM course", con);

    //     con.Open();
    //     var rdr = cmd.ExecuteReader();

    //     while (rdr.Read())
    //     {
    //         courses.Add(new Course
    //         {
    //             CourseId = rdr.GetInt32("course_id"),
    //             CourseName = rdr.GetString("course_name"),
    //             RequirementId = rdr.GetInt32("requirement_id")
    //         });
    //     }

    //     return Ok(courses);
    // }

    [HttpGet]
    public IActionResult GetCourses(int userId, int? courseId, string? search)
    {
        var list = new List<Courses>();

        using var conn = new MySqlConnection(_connectionString);
        conn.Open();

        using var cmd = new MySqlCommand();
        cmd.Connection = conn;

        // Base Query (User filter is mandatory)
        var query = @"
            SELECT 
                c.course_id,
                c.course_name,
                c.requirement_id
            FROM users u
            INNER JOIN requirements r 
                ON u.Id = r.UserId
            INNER JOIN course c 
                ON r.RequirementId = c.requirement_id
            WHERE u.Id = @UserId
        ";

        cmd.Parameters.AddWithValue("@UserId", userId);

        // Search Filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query += " AND c.course_name LIKE @search";
            cmd.Parameters.AddWithValue("@search", $"%{search}%");
        }

        // CourseId Filter
        if (courseId.HasValue)
        {
            query += " AND c.course_id = @courseId";
            cmd.Parameters.AddWithValue("@courseId", courseId.Value);
        }

        cmd.CommandText = query;

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new Courses
            {
                CourseId = Convert.ToInt32(reader["course_id"]),
                CourseName = reader["course_name"]?.ToString() ?? "",
                RequirementId = Convert.ToInt32(reader["requirement_id"])
            });
        }

        return Ok(list);
    }


    // POST: api/course
    [HttpPost]
    public IActionResult AddCourse([FromBody] Courses course)
    {
        using var con = new MySqlConnection(_connectionString);
        var cmd = new MySqlCommand(
            "INSERT INTO course(course_name, requirement_id) VALUES(@name, @reqId)", con);

        cmd.Parameters.AddWithValue("@name", course.CourseName);
        cmd.Parameters.AddWithValue("@reqId", course.RequirementId);

        con.Open();
        cmd.ExecuteNonQuery();

        return Ok("Course added successfully");
    }
}
