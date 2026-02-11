public class Requirement
{
    public int RequirementId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Interest { get; set; } = string.Empty;
    public string? SkillLevel { get; set; }
    public string? Goal { get; set; }
    
}
