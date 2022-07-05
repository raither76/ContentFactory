using ContentFactory.Code;
using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ContentFactory.ViewModels
{
    public class CatalogViewModel
    {
        private readonly ApplicationDbContext _context;

        public CatalogViewModel()
        {

        }
        public CatalogViewModel(ApplicationDbContext context, int CatId)
        {

            Catalog = context.Catalogs.Include(x => x.CatalogImages.OrderBy(z => z.Level)).Include(q => q.Category).Include(w => w.Parent).FirstOrDefault(z => z.Id == CatId);
            CategoryId = Catalog.CategoryId;
            CategoryName = Catalog.Category.Name;
            ParentId = Catalog.Parent.Id;
            CatalogName = Catalog.Parent.Name;
            Price3 = Catalog.Price3;
            Price6 = Catalog.Price6;
            Price9 = Catalog.Price9;
            Price12 = Catalog.Price12;
            Price15 = Catalog.Price15;





        }
        public void UpdateModel(CatalogViewModel model, ApplicationDbContext context)
        {
            Catalog item = context.Catalogs.FirstOrDefault(z => z.Id == model.Catalog.Id);
            item.Name = model.Catalog.Name;
            item.isVisible = model.Catalog.isVisible;
            item.Price3 = model.Catalog.Price3;
            item.Price6 = model.Catalog.Price6;
            item.Price9 = model.Catalog.Price9;
            item.Price12 = model.Catalog.Price12;
            item.Price15 = model.Catalog.Price15;
            context.Catalogs.Update(item);

            if (model.isChange)
            {
                Category category = context.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                category.Name = model.CategoryName;
                context.Categories.Update(category);
                Catalog cat = context.Catalogs.FirstOrDefault(x => x.Id == model.ParentId);
                cat.Name = model.CatalogName;
                context.Catalogs.Update(cat);
            }
            context.SaveChanges();
        }
        public Catalog Catalog { get; set; }
        public int CategoryId { get; set; }
        public int ParentId { get; set; }
        [Display(Name = "Наименование категории")]
        public string CategoryName { get; set; }
        [Display(Name = "Наименование подкатегории")]
        public string CatalogName { get; set; }
        public bool isChange { get; set; }
        [Display(Name = "Цена[3шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price3 { get; set; }
        [Display(Name = "Цена[6шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price6 { get; set; }
        [Display(Name = "Цена[9шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price9 { get; set; }
        [Display(Name = "Цена[12шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price12 { get; set; }
        [Display(Name = "Цена[15шт]")]
        [ModelBinder(BinderType = typeof(CustomDoubleBinder))]
        public double Price15 { get; set; }


    }
}
