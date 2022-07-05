using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentFactory.ViewModels
{
    public class AdminIndexViewModel
    {
        private readonly ApplicationDbContext _context;

        public AdminIndexViewModel(ApplicationDbContext context)
        {
            _context = context;

            Users = _context.Users.ToList();
            Categories = _context.Categories.Include(x => x.Catalogs).ToList();
            Models = _context.Models.ToList();
        }


        public List<User> Users { get; set; } = new List<User>();
        public List<Model> Models { get; set; } = new List<Model>();
        public List<Category> Categories { get; set; } = new List<Category>();



    }
    public class Result
    {
        public string Error { get; set; }
        public List<string> Fls { get; set; }
    }
}
