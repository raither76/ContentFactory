using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.EntityFrameworkCore;


namespace ContentFactory.ViewModels
{
    public class FillOrderViewModel
    {
        private ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        public FillOrderViewModel()
        {

        }
        public FillOrderViewModel(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public async Task<FillOrderViewModel> GetRow(int CurrentCatalogId, int CurrentItem)
        {

            if (_context.CatalogImages.Where(x => x.CatalogId == CurrentCatalogId).Sum(x => x.Level) > 1)
            {
                CatalogImages = _context.CatalogImages.Where(x => x.CatalogId == CurrentCatalogId).OrderBy(c => c.Level).ToList();
            }
            else
            {
                CatalogImages = _context.CatalogImages.Where(x => x.CatalogId == CurrentCatalogId).OrderBy(c => c.CatalogId).ToList();

            }
            Catalog tmp = _context.Catalogs.FirstOrDefault(x => x.Id == CurrentCatalogId);
            Catalog.AddRange(new List<double>() { tmp.Price3, tmp.Price6, tmp.Price9, tmp.Price12, tmp.Price15 });
            List<Txt> lst = _context.Txts.Take(5).OrderBy(x => x.Id).ToList();
            Txt.AddRange(lst.Select(x => x.Name));

            Locations = await _context.Locations.OrderBy(x => x.Price).ToListAsync();
            Colors = await _context.Colors.ToListAsync();
            CurrentOrderItem = _context.OrderItems.FirstOrDefault(x => x.Id == CurrentItem);





            CurrentColor = CurrentOrderItem.Color ?? "#000000";
            ModelStyles = await _context.ModelStyles.OrderBy(x => x.Price).Take(2).ToListAsync();


            Stylefiles = _context.OrderFiles.Where(x => x.OrderItemId == CurrentItem && (x.imPart > 0 || x.imPart < 3)).OrderBy(c => c.Id).ToList();
            Footerfiles = _context.OrderFiles.Where(x => x.OrderItemId == CurrentItem && (x.imPart > 2)).OrderBy(c => c.Id).ToList();
            NumberPhotos = PhotoCount(CurrentOrderItem.photoNumber);
            StyleDescription = CurrentOrderItem.StyleDescription;
            FooterDescription = CurrentOrderItem.FooterDesription;
            Videos = _context.Videos.ToList();
            LocId = CurrentOrderItem.LocId;
            VideoId = CurrentOrderItem.VideoId;
            StyleId = CurrentOrderItem.StyleId;


            return this;
        }
        private static int PhotoCount(int x)
        {
            return (x / 3) - 1;
        }
        public OrderItem CurrentOrderItem { get; set; }
        public List<string> Txt { get; set; } = new List<string>();
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<ModelStyle> ModelStyles { get; set; } = new List<ModelStyle>();
        public List<OrderFile> Stylefiles { get; set; } = new List<OrderFile>();
        public List<OrderFile> Footerfiles { get; set; } = new List<OrderFile>();
        public List<Video> Videos { get; set; } = new List<Video>();
        public List<Color> Colors { get; set; } = new List<Color>();
        public List<OrderItem> OrderItems = new List<OrderItem>();
        public int LocId { get; set; }
        public int VideoId { get; set; }
        public int StyleId { get; set; }

        public int NumberPhotos { get; set; }
        public string? StyleDescription { get; set; }
        public string? FooterDescription { get; set; }
        public string CurrentColor { get; set; }
        public List<double> Catalog { get; set; } = new List<double>();
        public List<CatalogImage> CatalogImages { get; set; } = new List<CatalogImage>();
    }
    public class imgVM
    {
        public string name { get; set; }
        public int number { get; set; }
    }
}
