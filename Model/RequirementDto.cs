using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalYearProjectAPI.Model
{
    public class RequirementDto
    {
          public int  RequirementId   { get; set; }
        public int? UserId          { get; set; }
        public string Role          { get; set; } = string.Empty;
        public string Domain        { get; set; } = string.Empty;
        public string Interest      { get; set; } = string.Empty;
        public string? SkillLevel   { get; set; }
        public string? Goal         { get; set; }
        public bool CourseGenerated { get; set; }
        public int? CourseId        { get; set; }
    }
}