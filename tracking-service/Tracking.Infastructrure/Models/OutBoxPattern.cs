using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tracking_service.Tracking.Infastructrure.Models
{
    [Table("OutBoxPattern")]
    public class OutBoxPattern
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public int EventType { get; set; } // (có thể đổi sang enum sau)

        [Required]
        public string Payload { get; set; } = null!;

        public int Status { get; set; } = 0; // Pending

        public int RetryCount { get; set; } = 0;

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }
    }
}
