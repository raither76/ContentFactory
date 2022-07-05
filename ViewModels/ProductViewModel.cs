using ContentFactory.Code;
using ContentFactory.Data;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ContentFactory.ViewModels
{
    public class ProductViewModel
    {

        public ProductViewModel()
        {

        }
        public ProductViewModel(ApplicationDbContext context, int Category, int? Parent)
        {


            CategoryId = Category;
            if (Parent != null) CatalogId = (int)Parent;

            if (Parent != null)
            {
                Bread = $"{context.Categories.FirstOrDefault(x => x.Id == Category).Name}/{context.Catalogs.FirstOrDefault(x => x.Id == Parent).Name}/";
            }
            else
            {
                Bread = $"{context.Categories.FirstOrDefault(x => x.Id == Category).Name}/";
            }
        }


        public int CategoryId { get; set; }
        public int CatalogId { get; set; }
        public string Bread { get; set; }
        [Required]
        [Display(Name = "Наименование категории")]
        public string ProductName { get; set; }

        [Display(Name = "Цена[3шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price3 { get; set; }
        [Display(Name = "Цена[6шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price6 { get; set; }
        [Display(Name = "Цена9")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price9 { get; set; }

        [Display(Name = "Цена12")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price12 { get; set; }
        [Display(Name = "Цена15")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price15 { get; set; }

        [Display(Name = "Показывать на сайте")]
        public bool isVisible { get; set; }


    }
}
