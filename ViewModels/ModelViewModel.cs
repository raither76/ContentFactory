using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContentFactory.ViewModels
{
    public class ModelViewModel
    {
        public ModelViewModel()
        {

        }
        public ModelViewModel(ApplicationDbContext context)
        {
            Categories = context.Categories.OrderBy(x => x.Name).ToList();


        }
        public ModelViewModel(ApplicationDbContext context, int Id)
        {
            _model = context.Models.Include(x => x.ModelImages).FirstOrDefault(z => z.Id == Id);
            Categories = context.Categories.OrderBy(x => x.Name).ToList();


        }
        [BindProperty]
        public Model _model { get; set; }




        public virtual List<Category> Categories { get; set; } = new List<Category>();


    }
    public class Limage
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
