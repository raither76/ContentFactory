using ContentFactory.Code;
using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ContentFactory.ViewModels
{
    public class PricesViewModel
    {


        public PricesViewModel()
        {

        }
        public PricesViewModel(ApplicationDbContext context)
        {

            List<Category> cat = context.Categories.OrderBy(y => y.Id).ToList();
            foreach (var item in cat)
            {
                List<Catalog> tmp = context.Catalogs.Where(c => c.CategoryId == item.Id).Include(x => x.Children).OrderBy(n => n.Name).ToList();
                CategoryName row = new CategoryName();
                row.Name = item.Name;
                foreach (var it in tmp)
                {
                    if (!it.Children.Any())
                    {

                        PriceViewModel rowitems = new PriceViewModel();

                        rowitems.CatalogId = it.Id;
                        rowitems.RowTitle = it.Name;
                        rowitems.CategoryName = it.Parent.Name;
                        rowitems.Price3 = it.Price3;
                        rowitems.Price6 = it.Price6;
                        rowitems.Price9 = it.Price9;
                        rowitems.Price12 = it.Price12;
                        rowitems.Price15 = it.Price15;
                        row.price.Add(rowitems);



                    }

                }
                lst.Add(row);

            }

        }
        public List<CategoryName> lst = new List<CategoryName>();



    }
    public class CategoryName
    {
        public string Name { get; set; }
        public List<PriceViewModel> price { get; set; } = new List<PriceViewModel>();
    }
    public class PriceViewModel
    {
        public void UpdateModel(ApplicationDbContext context)
        {
            Catalog cat = context.Catalogs.FirstOrDefault(x => x.Id == CatalogId);
            cat.Price3 = Price3;
            cat.Price6 = Price6;
            cat.Price9 = Price9;
            cat.Price12 = Price12;
            cat.Price15 = Price15;
            context.Update(cat);
            context.SaveChanges();
        }
        public string RowTitle { get; set; }
        public int CatalogId { get; set; }
        public string CategoryName { get; set; }
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



    }
}
