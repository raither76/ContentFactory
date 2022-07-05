using System.ComponentModel.DataAnnotations;

namespace ContentFactory.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Раздел категории товаров")]
        public string Name { get; set; }
        public virtual List<Catalog> Catalogs { get; set; } = new List<Catalog>();


    }


}
