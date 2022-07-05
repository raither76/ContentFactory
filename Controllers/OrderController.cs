using ContentFactory.Code;
using ContentFactory.Data;
using ContentFactory.Models;
using ContentFactory.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime;

namespace ContentFactory.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<Models.User> _userManager;
        private SignInManager<Models.User> _signManager;
        private ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        private IHttpContextAccessor _accessor;
        private readonly IUserStore<Models.User> _userStore;
        private readonly SignInManager<Models.User> _signInManager;



        public delegate void ThreadStart();
        public OrderController(ApplicationDbContext context, ILogger<HomeController> logger, IWebHostEnvironment appEnvironment,
            UserManager<Models.User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Models.User> signManager, IHttpContextAccessor accessor,
             IUserStore<Models.User> userStore, SignInManager<Models.User> signInManager)
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

        public async Task<IActionResult> AddOrder(int modelId)
        {
            Models.User ee = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (ee == null)
            {
                ee = new Models.User();
                await ee.CreateUserAsync(_accessor, _userManager, _userStore, _signInManager);
            }
            Model vm = await _context.Models.Include(x => x.ModelImages.Where(z => z.FaceImg == false)).FirstOrDefaultAsync(z => z.Id == modelId);
            return View(vm);

        }

        public async Task<IActionResult> Register(int OrderId, string err)
        {
            Models.User ee = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (ee == null)
            {
                ee = new Models.User();
                await ee.CreateUserAsync(_accessor, _userManager, _userStore, _signInManager);

            }
            RegisterViewModel vm = new RegisterViewModel(ee, OrderId);
            vm.ErrMessage = err;
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
           
            Models.User ee = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (ee == null)
            {
                ee = new Models.User();
                await ee.CreateUserAsync(_accessor, _userManager, _userStore, _signInManager);
            }

            ee = await vm.PutUser(ee);

            if (ee.Phone != null && ee.Phone.Length > 15 && ee.Nick.Length > 3)
            {
                if (ee.TelegramId == null) ee.TelegramId = "";
                if (ee.Brend == null) ee.Brend = "";
                if (ee.CompanyName == null) ee.CompanyName = "";
                if (ee.INN == null) ee.INN = "";
                if (ee.INN1 == null) ee.INN1 = "";
                if (ee.KPP == null) ee.KPP = "";
                if (ee.HeadOfCompany == null) ee.HeadOfCompany = "";

                _context.Users.Update(ee);
                await _context.SaveChangesAsync();
                Order _order = new Order();
                while (_order.Id == 0)
                {
                    _order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == vm.OrderId);

                }

                _order.IsComplete = true;
                _order.OrderType = vm.PaymentType;
                _order.OrderNumber = _context.Orders.Max(o => o.OrderNumber) + 1;
                _context.Orders.Update(_order);

                await _context.SaveChangesAsync();
                Thread.Sleep(10000);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                _context.Users.Update(ee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Register", new { orderId = vm.OrderId, err = "Заполните необходимую для оформления заказа информацию." });
            }




        }



        public async Task<IActionResult> EditRow(int Id)
        {
            OrderViewModel vm = await new OrderViewModel(_context, _appEnvironment).GetPos(Id);
            return View("Order", vm);


        }

        public async Task<IActionResult> Order(int ModelId, int orderId)
        {
            Models.User _user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (_user == null)
            {
                _user = new Models.User();
                await _user.CreateUserAsync(_accessor, _userManager, _userStore, _signInManager);
            }
            Order order = _context.Orders.Where(x => x.UserId == _user.Id && x.ModelId == ModelId && x.IsComplete == false).FirstOrDefault();
            if (order != null) orderId = order.Id;
            if (orderId == 0)
            {
                OrderViewModel vm = await new OrderViewModel(_context, _appEnvironment).StartPos(ModelId, _user);
                return View(vm);
            }
            else
            {

                OrderViewModel vm = await new OrderViewModel(_context, _appEnvironment).FillOrder(orderId);
                return View(vm);
            }



        }


        public async Task<ActionResult> GetProducts(int Id)
        {
            OrderViewModel model = new OrderViewModel(_context, _appEnvironment).GetProducts(Id);
            return PartialView("_order", model.Products);

        }

        private static int removingItemOrderId = 0;
        public async Task<ActionResult> RemoveOrderItem(int Id)
        {
            OrderItem oi = _context.OrderItems.FirstOrDefault(x => x.Id == Id);

            if (oi != null)
            {
                removingItemOrderId = oi.OrderId;
                _context.OrderItems.Remove(oi);
                await _context.SaveChangesAsync();
                List<OrderItem> vm = _context.OrderItems.Where(x => x.OrderId == oi.OrderId).ToList();
                var json = new
                {
                    GoodsCount = vm.Where(x => x.IsSave == true).Sum(z => z.Quontity),
                    PhotosCount = vm.Where(x => x.IsSave == true).Sum(x => x.photoNumber * x.Quontity),
                    VideosCount = vm.Where(x => x.VideoId !=9999 && x.IsSave == true).Sum(z => z.Quontity),
                    ITOG = String.Format("{0:C0}", vm.Where(x => x.IsSave == true).Sum(x => x.Price))
                };


                return Json(json);
            }
            return Json(null);

        }
        public async Task<ActionResult> GetStyle(int Id)
        {
            ModelStyle st = _context.ModelStyles.FirstOrDefault(x => x.Id == Id);
            if (st != null && st.Price > 0)
            {
                var json = new
                {
                    price = st.Price
                };
                return Json(json);
            }
            else
            {
                var json = new
                {
                    price = 0
                };
                return Json(json);
            }
        }
        public void PDF()
        {
            string dc = "";
            // dc.SheetOne(_appEnvironment);
        }



        public async Task<ActionResult> CopyRow(int Id)
        {
            OrderItem oi = _context.OrderItems.FirstOrDefault(x => x.Id == Id);
            if (oi != null)
            {
                removingItemOrderId = oi.OrderId;
                OrderItem item = new OrderItem();
                item = oi;
                item.Id = 0;

                _context.OrderItems.Add(item);
                await _context.SaveChangesAsync();
                List<OrderItem> vm = _context.OrderItems.Where(x => x.OrderId == oi.OrderId && x.IsSave == true).ToList();
                var json = new
                {
                    GoodsCount = vm.Where(x => x.IsSave == true).Sum(z => z.Quontity),
                    PhotosCount = vm.Where(x => x.IsSave == true).Sum(x => x.photoNumber * x.Quontity),
                    VideosCount = vm.Where(x => x.VideoId !=9999 && x.IsSave == true).Sum(z => z.Quontity),
                    ITOG = String.Format("{0:C0}", vm.Where(x => x.IsSave == true).Sum(x => x.Price))
                };

                return Json(json);

            }
            return Json(null);
        }


        public async Task<ActionResult> GetTable(int Id)
        {
            OrderItem oi = _context.OrderItems.FirstOrDefault(x => x.Id == Id);
            if (oi != null)
            {
                List<OrderItem> vm = _context.OrderItems.Where(x => x.OrderId == oi.OrderId && x.IsSave == true).ToList();
                return PartialView("_orderTable", vm);
            }

            return null;


        }
        public async Task<ActionResult> GetFooter(int Id)
        {


            Order order = _context.Orders.Where(x => x.Id == removingItemOrderId).FirstOrDefault();
            if (order != null)
            {
                OrderViewModel vm = await new OrderViewModel(_context, _appEnvironment).FillOrder(order.Id);

                if (vm != null)
                {

                    return PartialView("_orderFooter", vm);
                }
            }


            return null;


        }

        public async Task<ActionResult> FillRow(int Id, int Row)
        {
            FillOrderViewModel model = await new FillOrderViewModel(_context, _appEnvironment).GetRow(Id, Row);
            return PartialView("_fillOrder", model);

        }




        [HttpPost]

        public async Task<ActionResult> SaveData(OrderViewModel model)
        {
            Models.User _user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            int res = await model.SaveOrderRow(_user, _context);

            return RedirectToAction("Order", new { ModelId = model.ModelId, orderId = model.OrderId });
        }



        [HttpPost]

        public async Task<ActionResult> AddOrderItemImage(IFormFile im, int itemId, int photoNum)
        {
            string str = await im.SaveUserPhoto(itemId, photoNum, _appEnvironment, _context);
            imgVM vm = new imgVM() { number = photoNum, name = str };
            if (photoNum == 0) return PartialView("_myPhoto", str);
            else return PartialView("_mylistPhoto", vm);

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}