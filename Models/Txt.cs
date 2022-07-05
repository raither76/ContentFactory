using System.ComponentModel.DataAnnotations;

namespace ContentFactory.Models
{
    public class Txt
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

    }


}
