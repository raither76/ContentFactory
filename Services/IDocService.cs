using ClosedXML.Excel;
using ContentFactory.Models;

namespace ContentFactory.Services
{
    public interface IDocService
    {
        Task<Order> CreateDocs(int OrderId);
        Task<IXLWorksheet> FillOrder(IXLWorksheet sheet, Order _order);
        Task<IXLWorksheet> FillOrder2(IXLWorksheet sheet, Order _order);
        Task<IXLWorksheet> FillPay(IXLWorksheet sheet, Order _order);
        Task<IXLWorksheet> FillReference(IXLWorksheet sheet, OrderItem _item);
        Task<List<Order>> GetSendingDocs();
        Task<User> GetUser(int orderId);
        Task<string> GetVcard(User user);
        Task UpdateDoc();
        Task UpdateOrderAsync(Order order);
        Task UpdateUser(User user);
    }
}