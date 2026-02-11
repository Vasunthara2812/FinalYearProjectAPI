using System.ComponentModel.DataAnnotations;

public class Course
{
    public int CourseId { get; set; }

    [Required]
    public string CourseName { get; set; } = string.Empty;

    public int RequirementId { get; set; }
}
