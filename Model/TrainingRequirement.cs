namespace LearningApi.Models
{
    public class TrainingRequirement
    {
        public required string Role { get; set; }
        public required string Domain { get; set; }
        public required string Interest { get; set; }
        public required string SkillLevel { get; set; }
        public required string Goal { get; set; }
    }
}
