using ContentFactory.Data;
using ContentFactory.Models;

namespace ContentFactory.ViewModels
{
    public static class OrderHelper
    {
        public static async Task<List<OrderVM>> GetOrdersVM(this ApplicationDbContext _context)
        {
            //List<OrderVM> _vm = _context.Orders.Where(x => x.IsComplete == true)
            //    .OrderBy(z => z.OrderNumber).ToList().Select(p => new OrderVM()
            //    {
            //        _user = _context.Users.FirstOrDefault(c => c.Id == p.UserId),
            //        OrderNumber = p.OrderNumber,
            //        OrderId = p.Id,
            //        Date = p.OrderDate,
            //        ModelName = _context.Models.FirstOrDefault(c => c.Id == p.ModelId).Name,
            //        Price = _context.OrderItems.Where(z => z.OrderId == p.Id && z.IsSave == true).Sum(x => x.Price)
            //    }).ToList();

            var ovm = from order in _context.Orders
                      where order.IsComplete == true
                      orderby order.OrderNumber
                      join us in _context.Users on order.UserId equals us.Id
                      join model in _context.Models on order.ModelId equals model.Id
                      select new OrderVM()
                      {
                          _user = us,
                          OrderNumber = order.OrderNumber,
                          OrderId = order.Id,
                          Date = order.OrderDate,
                          ModelName = model.Name,
                          Price = _context.OrderItems.Where(z => z.OrderId == order.Id && z.IsSave == true).Sum(x => x.Price)
                      };

            List<OrderVM> df = ovm.ToList();

            return df;
        }
    }

    public class OrderVM
    {


        public User _user { get; set; }
        public int OrderNumber { get; set; }
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string ModelName { get; set; }
        public double Price { get; set; }



    }
}
