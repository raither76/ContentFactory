using ContentFactory.Data;
using ContentFactory.Models;
using ContentFactory.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ContentFactory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private SignInManager<User> _signManager;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        private IHttpContextAccessor _accessor;
        private readonly IUserStore<User> _userStore;
        private readonly SignInManager<User> _signInManager;
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IWebHostEnvironment appEnvironment,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signManager, IHttpContextAccessor accessor,
             IUserStore<User> userStore, SignInManager<User> signInManager)
        {
            _userStore = userStore;
            _signInManager = signInManager;
            _accessor = accessor;
            _signManager = signManager;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            User user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Администратор"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }

            IndexViewModel vm = new IndexViewModel(_context);
            await vm.GetTitleModels(3);
            return View(vm);

        }
        public async Task<IActionResult> Login(string nick)
        {
            User user = await _context.Users.FirstOrDefaultAsync(z => z.HashLink == nick);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, true);
                if (await _userManager.IsInRoleAsync(user, "Администратор"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return RedirectToAction("Index");


        }

        public IActionResult Privacy()
        {
            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}