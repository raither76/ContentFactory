using System.ComponentModel.DataAnnotations;
namespace ContentFactory.Models
{
    public class Catalog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Категория товаров")]
        public string Name { get; set; }
        [Display(Name = "Иконка")]
        public string? Icon { get; set; }
        [Display(Name = "Раздел категории товаров")]
        public int CategoryId { get; set; }

        public double Price3 { get; set; }
        public double Price6 { get; set; }
        public double Price9 { get; set; }
        public double Price12 { get; set; }
        public double Price15 { get; set; }
        public virtual Category Category { get; set; }
        public int Level { get; set; }
        [Display(Name = "Показывать в каталоге")]
        public bool isVisible { get; set; }
        [Display(Name = "Сортировка")]
        public int? Order { get; set; }  // порядок следования пункта в подменю
        public int? ParentId { get; set; }  // ссылка на id родительского меню
        public Catalog Parent { get; set; }    // родительское меню
        public ICollection<Catalog> Children { get; set; }   // дочерние пункты меню
        public Catalog()
        {
            Children = new List<Catalog>();
        }
        public List<CatalogImage> CatalogImages { get; set; } = new List<CatalogImage>();
        public double GetPrice(int numPhoto)
        {
            switch (numPhoto)
            {

                case 3: return Price3; break;
                case 6: return Price6; break;
                case 9: return Price9; break;
                case 12: return Price12; break;
                case 15: return Price15; break;
                default: return 0; break;
            }

        }
        public Catalog RefreshList(int Id, string direction)
        {
            for (int i = 0; i < CatalogImages.Count; i++)
            {
                CatalogImages[i].Level = i;
            }
            int currentpos = CatalogImages.FirstOrDefault(x => x.Id == Id).Level;
            switch (direction)
            {
                case "up":
                    if (currentpos != 0)
                    {
                        CatalogImages[currentpos].Level = CatalogImages[currentpos].Level - 1;
                        CatalogImages[currentpos - 1].Level = CatalogImages[currentpos - 1].Level + 1;
                    }

                    break;
                case "down":
                    if (currentpos != (CatalogImages.Count - 1))
                    {
                        CatalogImages[currentpos].Level = CatalogImages[currentpos].Level + 1;
                        CatalogImages[currentpos + 1].Level = CatalogImages[currentpos + 1].Level - 1;
                    }
                    break;

                default:
                    break;
            }
            return this;

        }
    }

    public class CatalogImage
    {


        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int CatalogId { get; set; }
        public int Level { get; set; }
        public virtual Catalog Catalog { get; set; }

    }
}
