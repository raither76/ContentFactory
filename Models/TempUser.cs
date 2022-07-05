using System.ComponentModel.DataAnnotations;

namespace ContentFactory.Models
{
    public class TempUser
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public DateTime DateCreationg { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();


    }


}
