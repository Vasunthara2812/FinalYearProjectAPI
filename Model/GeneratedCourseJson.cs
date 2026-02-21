using System;

namespace GenerateCourse.Models
{
    public class GeneratedCourseJson
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public required string CourseJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}