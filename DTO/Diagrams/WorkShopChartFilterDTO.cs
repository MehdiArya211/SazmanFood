namespace DTO.Diagrams
{
    public class WorkShopChartFilterDTO
    {

        public WorkShopType ChartType { get; set; }
    }

    public enum WorkShopType
    {
        SkillType,
        Field,
    }
}
