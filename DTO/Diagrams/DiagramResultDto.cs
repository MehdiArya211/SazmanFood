namespace DTO.Diagrams
{
    public class DiagramResultDto<T>
    {
        public List<T> Data { get; set; }

        public List<string>? Ids { get; set; }

        public List<string> Label { get; set; }

        public List<string> Color { get; set; }
    }
}
