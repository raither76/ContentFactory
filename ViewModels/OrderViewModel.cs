using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.EntityFrameworkCore;


namespace ContentFactory.ViewModels
{
    public class OrderViewModel
    {
        private ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        public OrderViewModel()
        {

        }
        public OrderViewModel(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<OrderViewModel> StartPos(int ModelId, User user)
        {

            Order order = _context.Orders.FirstOrDefault(o => o.ModelId == ModelId && o.UserId == user.Id && o.IsComplete == false);
            LocId = _context.Locations.OrderBy(x => x.Price).First().Id;
            VideoId = 9999;
            StyleId = _context.ModelStyles.OrderBy(x => x.Price).First().Id;
            DateTime _photoDate = _context.Models.FirstOrDefault(x => x.Id == ModelId).WorkDate;
            if (order == null)
            {

                OrderItem item = new OrderItem()
                {
                    Quontity = 3,
                    StyleDescription = "",
                    FooterDesription = "",
                    LocId = (int)LocId,
                    VideoId = 9999,
                    StyleId = (int)StyleId
                };

                order = new Order() { OrderDate = DateTime.Now, PhotoDate = _photoDate, UserId = user.Id, ModelId = ModelId, Quontity = 1, Description = "" };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                item.OrderId = order.Id;
                _context.OrderItems.Add(item);
                await _context.SaveChangesAsync();
                order.OrderItems.Add(item);
                OrderId = order.Id;
                OrderDate = order.OrderDate;
                UserId = order.UserId;
                this.ModelId = order.ModelId;
                Quontity = order.Quontity;
                OrderItems = order.OrderItems;


                CurrentOrderItem = order.OrderItems.OrderBy(x => x.Id).Last().Id;

            }
            else
            {
                OrderId = order.Id;
                OrderDate = order.OrderDate;
                UserId = order.UserId;
                this.ModelId = order.ModelId;
                Quontity = order.Quontity;
                OrderType = order.OrderType;
                Description = order.Description;
                OrderNumber = order.OrderNumber;
                order.OrderItems = _context.OrderItems.Where(x => x.OrderId == OrderId && x.IsSave == false).OrderBy(c => c.Id).ToList();
                if (!order.OrderItems.Any())
                {
                    OrderItem item = new OrderItem() { Quontity = 3, OrderId = OrderId, LocId = (int)LocId, VideoId = 9999, StyleId = (int)StyleId };
                    _context.OrderItems.Add(item);
                    order.OrderItems.Add(item);
                    await _context.SaveChangesAsync();
                    CurrentOrderItem = order.OrderItems.FirstOrDefault(x => x.IsSave == false).Id;
                }
                else
                {
                    CurrentOrderItem = order.OrderItems.FirstOrDefault(x => x.IsSave == false).Id;
                    List<OrderFile> files = _context.OrderFiles.Where(x => x.OrderItemId == CurrentOrderItem && x.imPart == 0).ToList();
                    if (files.Any())
                    {
                        order.OrderItems.OrderBy(x => x.Id).Last().OrderFiles.AddRange(files);

                    }

                }


                OrderItems = order.OrderItems;

            }

            isEdit = false;
            CurrentCategory = _context.Models.FirstOrDefault(x => x.Id == ModelId).CategoryId;
            Catalogs = _context.Catalogs.Include(c => c.Children).OrderBy(x => x.Name).Where(z => z.CategoryId == CurrentCategory && z.Children.Any()).ToList();
            CurrentRootCatalog = Catalogs[0].Id;
            Products = Catalogs[0].Children.OrderBy(x => x.Name).ToList();
            CurrentChildCatalog = Products[0].Id;

            return this;
        }

        public OrderViewModel GetProducts(int Id)
        {
            Products = _context.Catalogs.Where(z => z.ParentId == Id && z.isVisible == true).OrderBy(x => x.Name).ToList();
            return this;
        }
        public async Task<OrderViewModel> FillOrder(int orderId)
        {
            LocId = _context.Locations.OrderBy(x => x.Price).First().Id;
            VideoId = 9999;
            StyleId = _context.ModelStyles.OrderBy(x => x.Price).First().Id;
            Order order = _context.Orders.FirstOrDefault(x => x.Id == orderId);

            CurrentCategory = _context.Models.FirstOrDefault(x => x.Id == order.ModelId).CategoryId;
            Catalogs = _context.Catalogs.Include(c => c.Children).OrderBy(x => x.Name).Where(z => z.CategoryId == CurrentCategory && z.Children.Any()).ToList();
            CurrentRootCatalog = Catalogs[0].Id;
            Products = Catalogs[0].Children.OrderBy(x => x.Name).ToList();
            CurrentChildCatalog = Products[0].Id;
            OrderItem item = _context.OrderItems.FirstOrDefault(x => x.OrderId == orderId && x.IsSave == false);
            if (item == null)
            {
                item = new OrderItem() { Quontity = 3, OrderId = order.Id, LocId = (int)LocId, VideoId = 9999, StyleId = (int)StyleId };
                _context.OrderItems.Add(item);
                await _context.SaveChangesAsync();
            }
            OrderItems = _context.OrderItems.Where(o => o.OrderId == order.Id).ToList();
            CurrentOrderItem = OrderItems.FirstOrDefault(x => x.IsSave == false).Id;
            OrderId = orderId;
            Quontity = 1;
            GoodsCount = OrderItems.Where(x => x.IsSave == true).Sum(z => z.Quontity);
            PhotosCount = OrderItems.Where(x => x.IsSave == true).Sum(x => x.photoNumber * x.Quontity);
            VideosCount = OrderItems.Where(x => x.VideoId != 9999 && x.IsSave == true).Sum(z => z.Quontity);
            ITOG = OrderItems.Where(x => x.IsSave == true).Sum(x => x.Price);
            isEdit = false;
            ModelId = order.ModelId;

            return this;
        }

        public async Task<OrderViewModel> GetPos(int Id)
        {
            CurrentOrderItem = Id;
            OrderItem item = _context.OrderItems.FirstOrDefault(x => x.Id == Id);
            Order order = _context.Orders.FirstOrDefault(x => x.Id == item.OrderId);
            OrderItems = _context.OrderItems.Where(o => o.OrderId == order.Id && o.IsSave == true).ToList();

            CurrentCategory = _context.Models.FirstOrDefault(x => x.Id == order.ModelId).CategoryId;
            Catalogs = _context.Catalogs.Include(c => c.Children).OrderBy(x => x.Name).Where(z => z.CategoryId == CurrentCategory && z.Children.Any()).ToList();
            CurrentChildCatalog = item.CatalogId;
            CurrentRootCatalog = (int)_context.Catalogs.FirstOrDefault(x => x.Id == CurrentChildCatalog).ParentId;
            Products = Catalogs.FirstOrDefault(x => x.Id == CurrentRootCatalog).Children.OrderBy(x => x.Name).ToList();
            List<OrderFile> files = _context.OrderFiles.Where(x => x.OrderItemId == CurrentOrderItem && x.imPart == 0).ToList();
            if (files.Any())
            {
                OrderItems.FirstOrDefault(x => x.Id == CurrentOrderItem).OrderFiles.AddRange(files);

            }

            OrderId = order.Id;
            Quontity = item.Quontity;
            NumPhoto = PhotoCount(item.photoNumber);
            NumberPhotos = NumPhoto;
            LocId = item.LocId;
            VideoId = item.VideoId;
            StyleId = item.StyleId;
            ModelId = order.ModelId;
            isEdit = true;
            GoodsCount = OrderItems.Where(x => x.IsSave == true).Sum(z => z.Quontity);
            PhotosCount = OrderItems.Where(x => x.IsSave == true).Sum(x => x.photoNumber * x.Quontity);
            VideosCount = OrderItems.Where(x => x.VideoId != 9999 && x.IsSave == true).Sum(z => z.Quontity);
            ITOG = OrderItems.Where(x => x.IsSave == true).Sum(x => x.Price);
            return this;
        }


        public bool isEdit { get; set; }
        public int CurrentCategory { get; set; }
        public int NumberPhotos { get; set; }

        public int GoodsCount { get; set; }
        public int PhotosCount { get; set; }
        public int VideosCount { get; set; }
        public double ITOG { get; set; }
        public int CurrentOrderItem { get; set; }
        public int CurrentRootCatalog { get; set; }
        public int CurrentChildCatalog { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserId { get; set; }
        public int ModelId { get; set; }
        public int OrderType { get; set; }
        public int NumPhoto { get; set; }
        public int? StyleId { get; set; }
        public int? LocId { get; set; }
        public int? VideoId { get; set; }



        public List<OrderItem> OrderItems = new List<OrderItem>();
        public string? StyleDescription { get; set; }
        public string? FooterDescription { get; set; }

        public string? Description { get; set; }
        public int OrderNumber { get; set; }
        public int Quontity { get; set; }
        public List<Catalog> Catalogs { get; set; } = new List<Catalog>();
        public List<Catalog> Products { get; set; } = new List<Catalog>();
        private static int PhotoCount(int x)
        {
            return (x / 3) - 1;
        }

    }
}
