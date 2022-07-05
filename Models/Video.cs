using ContentFactory.Data;
using System.ComponentModel.DataAnnotations;

namespace ContentFactory.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        private ApplicationDbContext _context;
        private IWebHostEnvironment _appEnvironment;
        public Video()
        {
        }
        public Video(ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public async Task UpdateVideo(IFormFile file, VideoVM model, ApplicationDbContext context, IWebHostEnvironment appEnvironment)
        {

            string fileName = file.FileName;

            string path = appEnvironment.WebRootPath + "\\videos\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            //string renameFile = Convert.ToString(Guid.NewGuid()) + "." + fileName.Split('.').Last();
            var fullPath = Path.Combine(path, fileName);
            var fileStream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fileStream);
            fileStream.Close();
            Name = model.Name == null ? "Новое видео" : model.Name;
            Price = model.Price;
            Description = model.Description;
            FileName = fileName;
            FilePath = path;
            context.Videos.Update(this);
            await context.SaveChangesAsync();


        }


    }
    public class VideoVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
    }

}
