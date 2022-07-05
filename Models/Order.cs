using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ContentFactory.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Дата")]
        public DateTime OrderDate { get; set; }
        public DateTime PhotoDate { get; set; }
        public string UserId { get; set; }
        public int ModelId { get; set; }
        public int OrderType { get; set; }
        [InverseProperty(nameof(OrderItem.Order))]

        public List<OrderItem> OrderItems = new List<OrderItem>();

        [Display(Name = "Примечания к заказу")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Номер заказа")]

        public int OrderNumber { get; set; }
        [Required]
        [Display(Name = "Количество")]
        public int Quontity { get; set; }
        public bool IsComplete { get; set; }
        public bool IsSend { get; set; }
        public bool IsDocComplete { get; set; }
        public bool IsMaking { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

    }
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }
        [Required]
        [Display(Name = "Цена")]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Количество")]
        public int Quontity { get; set; }
        public int photoNumber { get; set; }
        public string? VideoType { get; set; }
        public int VideoId { get; set; }
        public string? Name { get; set; }
        public int CatalogId { get; set; }
        public string? LocationType { get; set; }
        public string? Color { get; set; }
        public int LocId { get; set; }
        public string? StyleType { get; set; }
        public int StyleId { get; set; }
        [Display(Name = "Описание")]
        public string? FooterDesription { get; set; }
        [Display(Name = "Примечания к стилю")]
        public string? StyleDescription { get; set; }
        //[InverseProperty(nameof(OrderFile.OrderItem))]
        public List<OrderFile> OrderFiles = new List<OrderFile>();
        public bool IsSave { get; set; }

    }
    public class OrderFile
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int OrderItemId { get; set; }
        public int imPart { get; set; }
        //[ForeignKey(nameof(OrderItemId))]
        //[InverseProperty("OrderFiles")]
        public virtual OrderItem OrderItem { get; set; }

    }
    public class OrderRow
    {

        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int ModelId { get; set; }
        public int CatalogId { get; set; }
        public int Quontity { get; set; }
        public int? NumPhoto { get; set; }
        public int? StyleId { get; set; }
        public int? LocId { get; set; }
        public int? VideoId { get; set; }
        public string? StyleDescription { get; set; }
        public string? FooterDescription { get; set; }
    }


}
