using ContentFactory.Data;
using ContentFactory.Models;
using ContentFactory.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ContentFactory.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        public AdminController(ApplicationDbContext context, ILogger<HomeController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> Index()
        {


            List<Model> items = _context.Models.Include(z => z.ModelImages).Where(x => x.IsHide == false).OrderBy(c => c.WorkDate).ToList();
            return View(items);

        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        public IActionResult AddModel()
        {
            ModelViewModel vm = new ModelViewModel(_context);
            return View(vm);
        }
        public IActionResult EditTxt()
        {
            List<Txt> txt = _context.Txts.Take(5).OrderBy(x => x.Id).ToList();
            return View(txt);
        }
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3000)]
        public async Task<IActionResult> EditOrders()
        {
            List<OrderVM> vm = await _context.GetOrdersVM();
            return View(vm);
        }
        public IActionResult dowloadPdf(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);
            string file_path = Path.Combine(order.FilePath, $"{order.FileName}_2.pdf");

            // Тип файла - content-type
            string file_type = "application/pdf";
            // Имя файла - необязательно
            string file_name = order.FileName + "_2.pdf";
            if (System.IO.File.Exists(file_path))
            {
                byte[] mas = System.IO.File.ReadAllBytes(file_path);
                double d = mas.Length;
                return File(mas, file_type, file_name);
            }
            return new NoContentResult();

        }
        public IActionResult dowloadXlsx(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);
            string file_path = Path.Combine(order.FilePath, order.FileName + "_2.xlsx");

            // Тип файла - content-type
            string file_type = "application/xlsx";
            // Имя файла - необязательно
            string file_name = order.FileName + "_2.xlsx";
            if (System.IO.File.Exists(file_path))
            {
                byte[] mas = System.IO.File.ReadAllBytes(file_path);
                double d = mas.Length;
                return File(mas, file_type, file_name);
            }
            return new NoContentResult();

        }
        public async Task RemoveOrder(int Id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == Id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

        }
        [HttpPost]
        public async Task<IActionResult> EditTxt(List<Txt> _model)
        {
            _context.Txts.UpdateRange(_model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category model)
        {
            if (model != null)
            {
                await _context.Categories.AddAsync(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddModel(ModelViewModel model)
        {
            if (model != null)
            {
                DateTime d = model._model.WorkDate;
                model._model.WorkDate = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
                await _context.Models.AddAsync(model._model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public async Task<IActionResult> EditModel(int Id)
        {
            ModelViewModel model = new ModelViewModel(_context, Id);

            if (model != null)
            {
                return PartialView("_editModel", model);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Locations()
        {
            List<Location> vm = _context.Locations.OrderBy(x => x.Name).ToList();

            return View("Locations", vm);
        }
        public async Task<IActionResult> Videos()
        {
            List<Video> vm = _context.Videos.Take(2).OrderBy(x => x.Name).ToList();

            return View("Videos", vm);
        }
        public async Task<IActionResult> ModelStyles()
        {
            List<ModelStyle> vm = _context.ModelStyles.OrderBy(x => x.Name).ToList();

            return View("ModelStyles", vm);
        }
        public async Task<IActionResult> EditPrices()
        {
            PricesViewModel model = new PricesViewModel(_context);

            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EditPrices(List<PriceViewModel> price)
        {

            if (price != null)
            {
                foreach (var item in price)
                {
                    item.UpdateModel(_context);

                }

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddProduct(int Category, int? Parent)
        {
            ProductViewModel vm = new ProductViewModel(_context, Category, Parent);
            return View("AddProduct", vm);
        }
        [HttpGet]
        public IActionResult AddImages(int CatalogId)
        {

            CatalogViewModel vm = new CatalogViewModel(_context, CatalogId);

            return PartialView("_addImages", vm);
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel model, string ReturnUrl)
        {
            if (model != null)
            {
                Catalog item = new Catalog()
                {
                    Name = model.ProductName,
                    CategoryId = model.CategoryId,
                    Price3 = model.Price3,
                    Price6 = model.Price6,
                    Price9 = model.Price9,
                    Price12 = model.Price12,
                    Price15 = model.Price15
                };

                if (model.CatalogId != 0) item.ParentId = model.CatalogId;

                await _context.Catalogs.AddAsync(item);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(CatalogViewModel model, string ReturnUrl)
        {
            if (model != null)
            {
                model.UpdateModel(model, _context);

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EditModel(ModelViewModel model)
        {
            if (model != null)
            {
                DateTime d = model._model.WorkDate;
                model._model.WorkDate = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
            
                _context.Models.Update(model._model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task EditPartialModel(Model model)
        {
            if (model != null)
            {
                DateTime d = model.WorkDate;
        
                model.WorkDate = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);

                _context.Models.Update(model);
                await _context.SaveChangesAsync();

            }
     

        }
        public async Task RemoveModelImage(int Id)
        {
            ModelImage im = _context.ModelImages.FirstOrDefault(x => x.Id == Id);
            if (im != null)
            {
                string path = Path.Combine(im.FilePath, im.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                _context.ModelImages.Remove(im);
                await _context.SaveChangesAsync();

            }
        }
        public async Task RemoveLocation(int Id)
        {
            Location loc = _context.Locations.FirstOrDefault(x => x.Id == Id);
            if (loc != null)
            {
                string path = Path.Combine(loc.FilePath, loc.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                _context.Locations.Remove(loc);
                await _context.SaveChangesAsync();

            }
        }
        public async Task RemoveStyle(int Id)
        {
            ModelStyle st = _context.ModelStyles.FirstOrDefault(x => x.Id == Id);
            if (st != null)
            {
                string path = Path.Combine(st.FilePath, st.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                _context.ModelStyles.Remove(st);
                await _context.SaveChangesAsync();

            }
        }

        public async Task<IActionResult> RemoveModel(int Id)
        {
            Model model = _context.Models.Include(z => z.ModelImages).FirstOrDefault(x => x.Id == Id);
            if (model.ModelImages.Any())
            {
                foreach (var im in model.ModelImages)
                {
                    string path = Path.Combine(im.FilePath, im.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

            }

            _context.Models.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }





        public async Task RemoveCatalogImage(int Id)
        {
            CatalogImage im = _context.CatalogImages.FirstOrDefault(x => x.Id == Id);
            if (im != null)
            {
                string path = Path.Combine(im.FilePath, im.FileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                _context.CatalogImages.Remove(im);
                await _context.SaveChangesAsync();

            }
        }
        public async Task<IActionResult> RemoveCatalog(int Id)
        {
            Catalog model = _context.Catalogs.Include(z => z.CatalogImages).FirstOrDefault(x => x.Id == Id);
            if (model.CatalogImages.Any())
            {
                foreach (var im in model.CatalogImages)
                {
                    string path = Path.Combine(im.FilePath, im.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

            }
            _context.Catalogs.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RemoveParentCatalog(int Id)
        {
            Catalog model = _context.Catalogs.Include(a => a.Children).ThenInclude(c => c.CatalogImages).FirstOrDefault(x => x.Id == Id);
            foreach (var item in model.Children)
            {
                if (item.CatalogImages.Any())
                {
                    foreach (var im in item.CatalogImages)
                    {
                        string path = Path.Combine(im.FilePath, im.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                }

            }
            _context.Catalogs.RemoveRange(model.Children);
            _context.Catalogs.Remove(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task ChangeFaceImage(int id)
        {
            int mid = _context.ModelImages.FirstOrDefault(x => x.Id == id).ModelId;
            Model item = _context.Models.Include(x => x.ModelImages).FirstOrDefault(z => z.Id == mid);
            item.SetFacePhoto(id);
            _context.ModelImages.UpdateRange(item.ModelImages);
            await _context.SaveChangesAsync();

        }




        [HttpPost]

        public ActionResult Upload(List<IFormFile> files, int CatId)
        {
            int __maxSize = 10 * 1024 * 1024;
            List<string> mimes = new List<string>
            {
            "image/jpeg", "image/jpg", "image/png"
            };
            var result = new Result
            {
                Fls = new List<string>()
            };

            foreach (IFormFile file in files)
            {
                string fileName = file.FileName;
                if (file.Length > __maxSize)
                {
                    result.Error = "Размер файла не должен превышать 10 Мб";
                    break;
                }
                else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                {
                    result.Error = "Недопустимый формат файла";
                    break;
                }
                string path = _appEnvironment.WebRootPath + "\\uploads\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
                var fullPath = Path.Combine(path, renameFile);

                var fileStream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(fileStream);

                fileStream.Close();
                CatalogImage Image = new CatalogImage()
                {
                    CatalogId = CatId,
                    Level = 0,
                    FileName = renameFile,
                    FilePath = path
                };
                Catalog cat = _context.Catalogs.Include(c => c.CatalogImages).FirstOrDefault(x => x.Id == CatId);
                cat.CatalogImages.Add(Image);
                _context.Catalogs.Update(cat);
                _context.SaveChanges();

            }
            List<CatalogImage> imgs = _context.CatalogImages.Where(x => x.CatalogId == CatId).OrderBy(z => z.Level).ToList();

            return PartialView("_productCarusel", imgs);
        }
        public async Task RefreshCatalogImages(int imId, string direct)
        {

            Catalog cat = _context.Catalogs.Include(z => z.CatalogImages.OrderBy(q => q.Level)).FirstOrDefault(x => x.Id == _context.CatalogImages.FirstOrDefault(x => x.Id == imId).CatalogId);
            if (cat != null)
            {
                cat.RefreshList(imId, direct);
                _context.CatalogImages.UpdateRange(cat.CatalogImages);
                await _context.SaveChangesAsync();
            }


        }




        [HttpPost]
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3000)]
        public ActionResult UploadModel(List<IFormFile> files, int Id)
        {
            int __maxSize = 10 * 1024 * 1024;
            List<string> mimes = new List<string>
            {
            "image/jpeg", "image/jpg", "image/png"
            };
            var result = new Result
            {
                Fls = new List<string>()
            };

            foreach (IFormFile file in files)
            {
                string fileName = file.FileName;
                if (file.Length > __maxSize)
                {
                    result.Error = "Размер файла не должен превышать 10 Мб";
                    break;
                }
                else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                {
                    result.Error = "Недопустимый формат файла";
                    break;
                }
                string path = _appEnvironment.WebRootPath + "\\uploads\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
                var fullPath = Path.Combine(path, renameFile);

                var fileStream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(fileStream);
                fileStream.Close();
                ModelImage Image = new ModelImage()
                {
                    ModelId = Id,
                    FileName = renameFile,
                    FilePath = path
                };
                _context.ModelImages.Add(Image);
                _context.SaveChanges();
            }
            List<ModelImage> imgs = _context.ModelImages.Where(x => x.ModelId == Id).ToList();
            return PartialView("_modelCarusel", imgs);
        }
        public async Task SaveLocation(int Id, string Name, string Description, int Price, string Color)
        {
            Location loc = _context.Locations.FirstOrDefault(x => x.Id == Id);
            if (loc != null)
            {
                loc.Name = Name;
                loc.Description = Description == null ? "" : Description;
                loc.Price = Price;
                loc.Color = Color;
                _context.Locations.Update(loc);
                await _context.SaveChangesAsync();
            }


        }
        public async Task SaveStyle(int Id, string Name, string Description, int Price)
        {
            ModelStyle st = _context.ModelStyles.FirstOrDefault(x => x.Id == Id);
            if (st != null)
            {
                st.Name = Name;
                st.Description = Description == null ? "" : Description;
                st.Price = Price;

                _context.ModelStyles.Update(st);
                await _context.SaveChangesAsync();
            }


        }
        [HttpPost]
        public async Task<ActionResult> UpdateVideo(IFormFile video, VideoVM model)
        {
            Video vid = _context.Videos.FirstOrDefault(x => x.Id == model.Id);
            if (vid != null)
            {
                vid.UpdateVideo(video, model, _context, _appEnvironment);

            }
            Video vm = _context.Videos.FirstOrDefault(x => x.Id == model.Id);

            return PartialView("_video", vm);
        }


        [HttpPost]
        public async Task SaveVideoDesc(VideoVM model)
        {
            Video vid = _context.Videos.FirstOrDefault(x => x.Id == model.Id);
            if (vid != null)
            {
                vid.Name = model.Name;
                vid.Price = model.Price;
                vid.Description = model.Description;
                _context.Videos.Update(vid);
                await _context.SaveChangesAsync();

            }

        }


        [HttpPost]
        public async Task<ActionResult> ChangeImLoc(IFormFile image, int Id)
        {
            Location loc = _context.Locations.FirstOrDefault(x => x.Id == Id);
            if (loc != null)
            {
                loc.UpdateFile(image);

            }
            return RedirectToAction("Locations");
        }

        [HttpPost]
        public async Task<ActionResult> ChangeImStyle(IFormFile image, int Id)
        {
            ModelStyle st = _context.ModelStyles.FirstOrDefault(x => x.Id == Id);

            if (st != null)
            {
                st.FileName = image.FileName;
                _context.ModelStyles.Update(st);
                await _context.SaveChangesAsync();

                st.UpdateFile(image);

            }
            return RedirectToAction("ModelStyles");
        }


        [HttpPost]
        public async Task<ActionResult> AddLocation(IFormFile image, string? Desc)
        {

            if (Desc != "" || Desc != null)
            {
                Location loc = _context.Locations.FirstOrDefault(x => x.Name.Contains(Desc));
                if (loc != null)
                {
                    loc.UpdateFile(image);
                }
                else
                {
                    Location tmp = new Location(_context, _appEnvironment);
                    tmp.Add(image, Desc);

                }
            }
            else
            {
                Location tmp = new Location(_context, _appEnvironment);
                tmp.Add(image, Desc);

            }

            List<Location> vm = _context.Locations.OrderBy(x => x.Name).ToList();

            return View("Locations", vm);
        }

        [HttpPost]
        public async Task<ActionResult> AddStyle(IFormFile image, string? Desc)
        {

            if (Desc != "" || Desc != null)
            {
                ModelStyle st = _context.ModelStyles.FirstOrDefault(x => x.Name.Contains(Desc));
                if (st != null)
                {
                    st.UpdateFile(image);
                }
                else
                {
                    ModelStyle tmp = new ModelStyle(_context, _appEnvironment);
                    tmp.Add(image, Desc);

                }
            }
            else
            {
                ModelStyle tmp = new ModelStyle(_context, _appEnvironment);
                tmp.Add(image, Desc);

            }

            List<ModelStyle> vm = _context.ModelStyles.OrderBy(x => x.Name).ToList();

            return View("ModelStyles", vm);
        }





        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}