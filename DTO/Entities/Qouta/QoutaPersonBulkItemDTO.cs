namespace DTO;

public class QoutaPersonBulkCreateDTO
{
    public long QoutaAllocationId { get; set; }

    public long FoodTokenTypeId { get; set; } = 1;

    public List<QoutaPersonBulkItemDTO> Items { get; set; }
}

public class QoutaPersonBulkItemDTO
{
    public long PersonId { get; set; }
    public string PersonalCode { get; set; }
    public long FoodId { get; set; }
}

