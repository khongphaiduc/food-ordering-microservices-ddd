using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace food_service.ProductService.Infrastructure.Models
{
    [Table("OutBoxMessage")]
    public class OutBoxMessage
    {
        [Key]
        public Guid Id { get; set; }

        public string Type { get; set; }

        public string Payload { get; set; }

        public bool IsProcessd { get; set; }

        public DateTime CreateAt { get; set; }
    }

}
