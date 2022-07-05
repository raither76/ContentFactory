using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContentFactory.Components
{
    public class ModelViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        private List<Model> model = new List<Model>();
        public ModelViewComponent(ApplicationDbContext context)
        {
            _context = context;

        }
        public IViewComponentResult Invoke()
        {

            model = _context.Models.OrderBy(c => c.WorkDate).Include(x => x.ModelImages).ToList();

            return View(model);// 
        }
    }

}
