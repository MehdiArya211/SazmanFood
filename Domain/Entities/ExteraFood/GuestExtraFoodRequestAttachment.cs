using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class GuestExtraFoodRequestAttachment : EntityBase
    {
        [Required]
        public long GuestExtraFoodRequestId { get; set; }

        [Required]
        [MaxLength(260)]
        public string OriginalFileName { get; set; }

        [Required]
        [MaxLength(150)]
        public string ContentType { get; set; }

        public long FileSize { get; set; }

        [Required]
        public byte[] FileContent { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public long UploaderId { get; set; }

        [MaxLength(200)]
        public string UploaderFullName { get; set; }

        public DateTime UploadDate { get; set; }

        public GuestExtraFoodRequest GuestExtraFoodRequest
        {
            get;
            set;
        }
    }
}