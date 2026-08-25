namespace DTO.Diagrams
{
    public class TeacherChartFilterDto
    {
        public TeacherChartType ChartType { get; set; }
    }

    public enum TeacherChartType
    {
        SkillType,
        Field,
    }
}
