using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContentFactory.Components
{
    public class CategoryViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        private List<Category> model = new List<Category>();
        public CategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;

        }
        public IViewComponentResult Invoke()
        {

            model = _context.Categories.Include(x => x.Catalogs.OrderBy(z => z.Name)).ToList();

            return View(model);// 
        }
    }

}
