using ContentFactory.Data;
using ContentFactory.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ContentFactory.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Локация")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Цена")]
        public int Price { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Color { get; set; }
        private ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        public Location()
        {
        }
        public Location(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public void UpdateFile(IFormFile file)
        {
            var fullPath = Path.Combine(FilePath, FileName);
            var fileStream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fileStream);
            fileStream.Close();
        }
        public async Task Add(IFormFile file, string? LName)
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
            string path = _appEnvironment.WebRootPath + "\\uploads\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
            var fullPath = Path.Combine(path, renameFile);
            var fileStream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fileStream);
            fileStream.Close();
            Name = LName == null ? "Новая локация" : LName;
            Price = 0;
            Description = "";

            FileName = renameFile;
            FilePath = path;
            _context.Locations.Add(this);
            await _context.SaveChangesAsync();


        }

    }
    public class Color
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }


}
