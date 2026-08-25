using Utilities.Extentions;

namespace DTO.Entities
{
    public class GuestExtraFoodRequestAttachmentDTO0
    {
        public long Id { get; set; }

        public long GuestExtraFoodRequestId { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public string Description { get; set; }

        public string UploaderFullName { get; set; }

        public DateTime UploadDate { get; set; }

        public bool CanDelete { get; set; }

        public string UploadDateFa
        {
            get
            {
                return UploadDate
                    .ToPersianDateTime()
                    .ToString();
            }
        }

        public string FileSizeTitle
        {
            get
            {
                if (FileSize < 1024)
                    return FileSize + " B";

                if (FileSize < 1024 * 1024)
                    return (FileSize / 1024D).ToString("0.##") + " KB";

                return (FileSize / (1024D * 1024D)).ToString("0.##") + " MB";
            }
        }
    }
}