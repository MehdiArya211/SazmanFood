namespace DTO.Entities.QoutaPerson;

/// <summary>
/// مدل نمایش پرسنل یگان برای ثبت غذا در سهمیه
/// </summary>
public class QoutaPersonSelectableDTO
{
    public long Id { get; set; }

    public long PersonId { get; set; }

    public string FName { get; set; }

    public string LName { get; set; }

    public string PersonalCode { get; set; }

    public long PersonalTypeId { get; set; }

    public string PersonalTypeTitle { get; set; }

    public string FoodTitle { get; set; }

    public long? QoutaPersonId { get; set; }

    public bool IsRegistered { get; set; }
}