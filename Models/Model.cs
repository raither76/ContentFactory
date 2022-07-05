using System.ComponentModel.DataAnnotations;

namespace ContentFactory.Models
{
    public class Model
    {
        public void SetFacePhoto(int id)
        {

            foreach (var img in ModelImages)
            {
                if (img.Id != id) img.FaceImg = false; else img.FaceImg = true;
            }


        }
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Имя модели")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Размер ноги")]
        public int FootSize { get; set; }
        [Required]
        [Display(Name = "Размер")]
        public string Size { get; set; }

        [Display(Name = "Бюст")]
        public string? Bust { get; set; }
        [Required]
        [Display(Name = "Рост")]
        public int Growth { get; set; }
        [Required]
        [Display(Name = "Объем груди")]
        public int BreastVolume { get; set; }
        [Required]
        [Display(Name = "Обьем талии")]
        public int Waist { get; set; }
        [Required]
        [Display(Name = "Объем бедер")]
        public int Hips { get; set; }
        [Required]
        [Display(Name = "Дата")]
        public DateTime WorkDate { get; set; }
        [Required]
        [Display(Name = "Занят")]
        public bool IsHide { get; set; }
        [Required]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        public virtual List<ModelImage> ModelImages { get; set; } = new List<ModelImage>();

    }
    public class ModelImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool FaceImg { get; set; }
        public int ModelId { get; set; }
        public virtual Model Model { get; set; }

    }

}
