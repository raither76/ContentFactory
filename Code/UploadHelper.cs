using ContentFactory.Data;
using ContentFactory.Models;
using ContentFactory.ViewModels;
using LazZiya.ImageResize;
using System.Drawing;

namespace ContentFactory.Code
{
    public static class UploadHelper
    {
        public static async Task<string> SaveUserPhoto(this IFormFile file, int itemId, int photoNum, IWebHostEnvironment _appEnvironment, ApplicationDbContext _context)
        {

            int __maxSize = 5 * 1024 * 1024;
            List<string> mimes = new List<string>
            {
            "image/jpeg", "image/jpg", "image/png"
            };
            var result = new Result
            {
                Fls = new List<string>()
            };
            string fileName = file.FileName;
            if (file.Length > __maxSize)
            {
                result.Error = "Размер файла не должен превышать 5 Мб";
            }
            else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
            {
                result.Error = "Недопустимый формат файла";
            }
            string path = _appEnvironment.WebRootPath + "\\temp\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
            var fullPath = Path.Combine(path, renameFile);
            var fileStream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fileStream);
            Image img = Image.FromStream(fileStream);
            var scaleImage = ImageResize.Scale(img, 200, 200);
            fileStream.Close();
            scaleImage.SaveAs(fullPath);
            //ImageBuilder.Current.Build(
            //new ImageJob(
            //    file.OpenReadStream(),
            //    fullPath,
            //    new Instructions("maxwidth=480&maxheight=480"),
            //    false,
            //    false));



            List<OrderFile> orderFiles = _context.OrderFiles.Where(x => x.OrderItemId == itemId && x.imPart == photoNum).ToList();

            if (orderFiles.Any())
            {
                string delpath = Path.Combine(orderFiles.First().FilePath, orderFiles.First().FileName);
                if (File.Exists(delpath))
                {
                    File.Delete(delpath);
                }

                orderFiles.First().FileName = renameFile;
                orderFiles.First().FilePath = path;
                orderFiles.First().imPart = photoNum;
                _context.OrderFiles.Update(orderFiles.First());
                await _context.SaveChangesAsync();
            }
            else
            {
                OrderFile of = new OrderFile() { FileName = renameFile, FilePath = path, OrderItemId = itemId, imPart = photoNum };
                _context.OrderFiles.Add(of);
                await _context.SaveChangesAsync();
            }

            return renameFile;

        }
        public static async Task<int> SaveOrderRow(this OrderViewModel row, User user, ApplicationDbContext _context)
        {
            OrderItem item = _context.OrderItems.FirstOrDefault(x => x.Id == row.CurrentOrderItem);
            if (item != null)
            {
                item.Quontity = row.Quontity;

                item.photoNumber = PhotoCount(row.NumberPhotos);
                item.OrderId = row.OrderId;
                item.VideoId = row.VideoId ?? 9999;

                double videoPrice = 0;
                if (item.VideoId != 9999)
                {
                    videoPrice = _context.Videos.FirstOrDefault(x => x.Id == item.VideoId).Price;
                    item.VideoType = _context.Videos.FirstOrDefault(x => x.Id == item.VideoId).Name;
                }
                else { item.VideoType = "Без видео"; }
                item.LocId = row.LocId ?? _context.Locations.OrderBy(x => x.Price).First().Id;
                item.LocationType = _context.Locations.FirstOrDefault(x => x.Id == item.LocId).Name;

                double locPrice = _context.Locations.FirstOrDefault(x => x.Id == item.LocId).Price;
                item.StyleId = row.StyleId ?? _context.ModelStyles.OrderBy(x => x.Price).First().Id;
                double stylePrice = _context.ModelStyles.FirstOrDefault(x => x.Id == item.StyleId).Price;

                item.StyleType = _context.ModelStyles.FirstOrDefault(x => x.Id == item.StyleId).Name;
                item.Color = _context.Locations.FirstOrDefault(x => x.Id == item.LocId).Color ?? "#000000";
                item.FooterDesription = row.FooterDescription;
                item.StyleDescription = row.StyleDescription;
                item.Price = (_context.Catalogs.FirstOrDefault(x => x.Id == row.CurrentChildCatalog).GetPrice(item.photoNumber)
                    + videoPrice + locPrice + stylePrice) * item.Quontity;

                item.CatalogId = row.CurrentChildCatalog;
                item.Name = _context.Catalogs.FirstOrDefault(x => x.Id == row.CurrentChildCatalog).Name;
                item.IsSave = true;
                _context.OrderItems.Update(item);
                await _context.SaveChangesAsync();

            }
            return 1;
        }
        private static int PhotoCount(int x)
        {
            return (x + 1) * 3;
        }


    }

}
