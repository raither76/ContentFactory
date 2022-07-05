using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using ContentFactory.Code;
using ContentFactory.Data;
using ContentFactory.ENUMS;
using ContentFactory.Models;
using GemBox.Spreadsheet;
using GemBox;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime;


namespace ContentFactory.Services;

public class DocService : IDocService
{
    private ILogger<DocService> _logger;
    private IWebHostEnvironment _appEnvironment;
    private ApplicationDbContext _context;

    private IMemoryCache _cache;
    private User us = new User();



    public DocService(ApplicationDbContext context, ILogger<DocService> logger, IWebHostEnvironment appEnvironment, IMemoryCache cache)
    {
        _logger = logger;
        _appEnvironment = appEnvironment;
        _context = context;
        _cache = cache;
    }



    //public async Task<XLWorkbook> GetTemplateXLSX(Order _order)
    //{
    //    XLWorkbook tbook = null;
    //    if (_order != null)
    //    {
    //        if (!_cache.TryGetValue("XLtemplate", out tbook))
    //        {
    //            string path = _appEnvironment.WebRootPath + "\\files\\";
    //            var fileName = "cf.xlsx";
    //            var fullPath = Path.Combine(path, fileName);
    //            if (File.Exists(fullPath))
    //            {
    //                tbook = new XLWorkbook(fullPath);
    //                if (tbook != null)
    //                {
    //                    _cache.Set("XLtemplate", tbook,
    //                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(365)));
    //                }

    //            }
    //        }

    //    }
    //    return tbook;
    //}
    private string PicturePos(int i)
    {
        var cell = typeof(CatalogImagesEnum).GetFields().Where(fi => fi.IsLiteral);
        string[] cellnames = cell.Select(fi => fi.Name).ToArray();
        //  var cellValues = cell.Select(fi => fi.GetRawConstantValue()).Cast<CatalogImagesEnum>().ToArray();
        return cellnames[i];

    }
    public async Task<IXLWorksheet> FillPay(IXLWorksheet sheet, Order _order)
    {


        sheet.Cell(13, 11).Value = $"{String.Format("{0:0000}", _order.OrderNumber)} от {_order.OrderDate.ToString("dd MMMM yyyy")} года";
        double itog = _order.OrderItems.Where(x => x.IsSave == true).Sum(x => x.Price);

        if (us.PaymentType == 2) sheet.Cell(19, 6).Value = $"ИП {us.FullName}, ИНН:{us.INN}";
        if (us.PaymentType == 3) sheet.Cell(19, 6).Value = $"{us.CompanyName}, ИНН:{us.INN1}, КПП:{us.KPP}, тел.{us.Phone}";

        sheet.Cell(22, 26).Value = itog.ToString("# ###.00");
        sheet.Cell(22, 32).Value = itog.ToString("# ###.00");
        sheet.Cell(24, 32).Value = itog.ToString("# ###.00");
        sheet.Cell(26, 32).Value = itog.ToString("# ###.00");
        sheet.Cell(28, 2).Value = $"Всего к оплате: {RuDateAndMoneyConverter.CurrencyToTxtFull(itog, true)}";

        return sheet;
    }
    public async Task<IXLWorksheet> FillOrder2(IXLWorksheet sheet, Order _order)
    {

        sheet.Cell(4, 4).Value = $"{String.Format("{0:0000}", _order.OrderNumber)} от {_order.OrderDate.ToString("dd MMMM yyyy")} года";
        double itog = _order.OrderItems.Where(x => x.IsSave == true).Sum(x => x.Price);

        if (us.PaymentType == 2) sheet.Cell(8, 6).Value = $"ИП {us.FullName}, ИНН:{us.INN}";
        if (us.PaymentType == 3) sheet.Cell(8, 6).Value = $"{us.CompanyName}, ИНН:{us.INN}, КПП:{us.KPP}, тел.{us.Phone}";

        sheet.Cell(11, 16).Value = itog.ToString("# ###.00");
        sheet.Cell(12, 16).Value = itog.ToString("# ###.00");
        sheet.Cell(14, 11).Value = itog.ToString("# ###.00");
        sheet.Cell(16, 2).Value = $"Всего выполнено работ на сумму: {RuDateAndMoneyConverter.CurrencyToTxtFull(itog, true)}";
        if (us.PaymentType == 2) sheet.Cell(26, 2).Value = $"Индивидуальный предприниматель\n {us.FullName}";
        if (us.PaymentType == 3) sheet.Cell(26, 2).Value = $"{us.CompanyName}\n Генеральный директор\n {us.HeadOfCompany}";



        return sheet;

    }
    public async Task<IXLWorksheet> FillReference(IXLWorksheet sheet, OrderItem _item)
    {



        List<OrderFile> files = await _context.OrderFiles.Where(x => x.OrderItemId == _item.Id).ToListAsync();
        ModelStyle style = await _context.ModelStyles.FirstOrDefaultAsync(x => x.Id == _item.StyleId);
        var subcat = _context.Catalogs.FirstOrDefault(x => x.Id == _item.CatalogId);
        var cat = _context.Catalogs.FirstOrDefault(x => x.Id == subcat.ParentId);

        for (int j = 0; j < 5; j++)
        {
            var path = "";
            var filename = "";
            var im = files.FirstOrDefault(x => x.imPart == j);
            if (im != null) { path = im.FilePath; filename = im.FileName; }
            else { path = _appEnvironment.WebRootPath + "\\images\\"; filename = "empty.png"; }

            var fullPath = Path.Combine(path, filename);
            IXLPicture image = null;
            if (File.Exists(fullPath))
            {
                image = sheet.AddPicture(fullPath);
            }
            else
            {
                fullPath = Path.Combine(_appEnvironment.WebRootPath + "\\images\\", "empty.png");
                image = sheet.AddPicture(fullPath);
            }

            switch (j)
            {
                case 0:
                    image.MoveTo(sheet.Cell("D3")).WithSize(89, 123);
                    break;
                case 1:
                    if (style.Price > 0)
                    {
                        image.MoveTo(sheet.Cell("C23")).WithSize(89, 123);
                    }
                    else { image.Delete(); }
                    break;
                case 2:
                    if (style.Price > 0)
                    {
                        image.MoveTo(sheet.Cell("F23")).WithSize(89, 123);
                    }
                    else { image.Delete(); }
                    break;
                case 3:
                    image.MoveTo(sheet.Cell("M3")).WithSize(89, 123);
                    break;
                case 4:
                    image.MoveTo(sheet.Cell("O3")).WithSize(89, 123);
                    break;
                default:
                    break;
            }
        }
        foreach (var item in files)
        {
            var fullPath = Path.Combine(item.FilePath, item.FileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        List<CatalogImage> catim = await _context.CatalogImages.Where(c => c.CatalogId == _item.CatalogId).OrderBy(x => x.Level).Take(_item.photoNumber).ToListAsync();
        int i = 0;

        foreach (var item in catim)
        {
            var fullPath = Path.Combine(item.FilePath, item.FileName);
            if (!File.Exists(fullPath))
            {
                var path = _appEnvironment.WebRootPath + "\\images\\"; var filename = "empty.png";
                fullPath = Path.Combine(path, filename);
            }

            var image = sheet.AddPicture(fullPath);
            image.MoveTo(sheet.Cell(PicturePos(i))).WithSize(89, 123);
            i++;

        }
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
        sheet.Cell(4, 7).Value = cat.Name;
        sheet.Cell(6, 7).Value = subcat.Name;
        sheet.Cell(7, 9).Value = _item.Quontity;
        sheet.Cell(9, 2).Value = _item.FooterDesription;
        sheet.Cell(13, 2).Value = _item.LocationType;
        sheet.Cell(15, 2).Value = _item.StyleType;
        sheet.Cell(16, 2).Value = _item.StyleDescription;
        sheet.Cell(13, 7).Value = _item.VideoType;
        sheet.Cell(13, 13).Value = _item.photoNumber;



        return sheet;
    }
    public async Task<IXLWorksheet> FillOrder(IXLWorksheet sheet, Order _order)
    {


        Model _model = await _context.Models.FirstOrDefaultAsync(x => x.Id == _order.ModelId);
        sheet.Cell(1, 2).Value = $"#{String.Format("{0:0000}", _order.OrderNumber)}";
        sheet.Cell(3, 5).Value = _order.OrderDate.ToString("dd MMMM yyyy");
        sheet.Cell(6, 4).Value = us.Nick == null ? "" : us.Nick;
        sheet.Cell(7, 4).Value = us.Brend;
        sheet.Cell(8, 4).Value = us.Phone;
        sheet.Cell(10, 5).Value = _order.PhotoDate.ToString("dd MMMM yyyy");
        sheet.Cell(11, 4).Value = _model.Name == null ? "" : _model.Name;
        sheet.Cell(6, 15).Value = $"{_model.Growth} см";
        sheet.Cell(7, 15).Value = $"{_model.BreastVolume} см";
        sheet.Cell(8, 15).Value = $"{_model.Waist} см";
        sheet.Cell(9, 15).Value = $"{_model.Hips} см";
        sheet.Cell(10, 15).Value = _model.Size.ToString();
        sheet.Cell(11, 15).Value = _model.FootSize.ToString();
        int startRow = 15;
        int currentPos = 1;

        foreach (var item in _order.OrderItems)
        {
            sheet.Cell(startRow, 2).Value = currentPos;
            sheet.Cell(startRow, 3).Value = item.Name;
            sheet.Cell(startRow, 7).Value = item.Quontity;
            sheet.Cell(startRow, 9).Value = item.photoNumber;
            sheet.Cell(startRow, 11).Value = item.VideoType;
            sheet.Cell(startRow, 13).Value = item.LocationType;
            sheet.Cell(startRow, 15).Value = item.StyleType;
            sheet.Cell(startRow, 17).Value = String.Format("{0:C0}", item.Price);
            sheet.Range(startRow, 2, startRow, 19).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            sheet.Range(startRow, 2, startRow, 19).Style.Font.FontColor = XLColor.Gray;
            startRow++;
            currentPos++;
        }

        sheet.Cell(startRow, 3).Value = "ИТОГО";
        sheet.Cell(startRow, 7).Value = _order.OrderItems.Where(x => x.IsSave == true).Sum(z => z.Quontity);
        sheet.Cell(startRow, 9).Value = _order.OrderItems.Where(x => x.IsSave == true).Sum(x => x.photoNumber * x.Quontity);
        sheet.Cell(startRow, 11).Value = _order.OrderItems.Where(x => x.VideoId != 9999 && x.IsSave == true).Sum(z => z.Quontity);
        sheet.Cell(startRow, 17).Value = String.Format("{0:C0}", _order.OrderItems.Where(x => x.IsSave == true).Sum(x => x.Price));
        sheet.Range(startRow, 2, startRow, 19).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        sheet.Range(startRow, 2, startRow, 19).Style.Font.FontColor = XLColor.Black;
        sheet.Range(startRow, 2, startRow, 19).Style.Font.Bold = true;

  
        sheet.PageSetup.PrintAreas.Add(1,1, startRow++,20);

        return sheet;

    }
    public async Task<List<Order>> GetSendingDocs()
    {
        List<Order> temporders = await _context.Orders.Where(x => x.OrderDate<DateTime.Now.AddDays(-1) && x.OrderNumber == 0).ToListAsync();
       _context.Orders.RemoveRange(temporders);
        await _context.SaveChangesAsync();
        List<Order> orders = await _context.Orders.Where(x => x.IsDocComplete == true && x.IsSend == false).ToListAsync();

        return orders;
    }
    public async Task UpdateDoc()
    {
        Order _order = await _context.Orders.FirstOrDefaultAsync(x => x.IsComplete == true && x.IsSend == false && x.IsDocComplete == false);
        if (_order != null)
        {
            await CreateDocs(_order.Id);
            _order.IsDocComplete = true;
            _context.Orders.Update(_order);
            await _context.SaveChangesAsync();

        }
    }
    public async Task<User> GetUser(int orderId)
    {

        return await _context.Users.FirstOrDefaultAsync(x => x.Id == _context.Orders.FirstOrDefault(c => c.Id == orderId).UserId);

    }
    public async Task<string> GetVcard(User user)
    {
        return await user.GetVcard(_appEnvironment.WebRootPath + "\\orders\\");
    }

    public async Task UpdateUser(User user)
    {

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

    }
    public async Task UpdateOrderAsync(Order order)
    {

        order.IsSend = true;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

    }

    public async Task<Order> CreateDocs(int OrderId)
    {

        _logger.LogInformation("Start One Doc making... ");
        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        List<filePair> filePairs = new List<filePair>();
        Order _order = await _context.Orders.FirstOrDefaultAsync(q => q.Id == OrderId);
        us = await _context.Users.FirstOrDefaultAsync(x => x.Id == _order.UserId);
        PdfSaveOptions po = new PdfSaveOptions() { SelectionType = SelectionType.EntireFile };

        if (_order != null)
        {
            string p = _appEnvironment.WebRootPath + "\\files\\";
            var f = "cf.xlsx";
            var fp = Path.Combine(p, f);
            XLWorkbook TemplateWorkBook = new XLWorkbook(fp);
            var fileName = $"CF-Order-{_order.OrderNumber}";
            var path = _appEnvironment.WebRootPath + "\\orders\\";
            var fullPath = Path.Combine(path, fileName);
            _order.FileName = fileName;
            _order.FilePath = path;
            _context.Orders.Update(_order);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            await _context.SaveChangesAsync();

            using (var wb = new XLWorkbook())
            {
                TemplateWorkBook.Worksheet("Sheet1").CopyTo(wb, "Sheet1");
                List<OrderItem> items = _context.OrderItems.Where(x => x.OrderId == _order.Id && x.IsSave == true).ToList();
                _order.OrderItems.AddRange(items);
                IXLWorksheet sheet = await FillOrder(wb.Worksheet("Sheet1"), _order);
                wb.SaveAs($"{fullPath}_0.xlsx");
                filePairs.Add(new filePair { xlsx = $"{fullPath}_0.xlsx", pdf = $"{fullPath}_0.pdf" });
            }
            using (var wb = new XLWorkbook()) 
            { 
                TemplateWorkBook.Worksheet("Sheet2").CopyTo(wb, "Sheet2");

                IXLWorksheet sheet = wb.Worksheet("Sheet2");
                var cell1 = sheet.Cell(6, 4).SetValue($"Доставить товар в студию можно лично, курьером (Яндекс.доставка, Dostavista и пр.) или транспортной компанией (СДЕК, DPD, MagicTrans и пр.) Обязательно напишите на пакете/корбке (или вложите лист внутрь) Номер Вашего заказа #{String.Format("{0:0000}", _order.OrderNumber)} неподписанный пакет/коробка отправляются на благотворительность");
                int num = cell1.Value.ToString().IndexOf("#");
                cell1.RichText.Substring(num, 6).SetFontColor(XLColor.Red).SetFontSize(11).SetBold(true);
                cell1 = sheet.Cell(28, 4).SetValue($"Забрать товар из студии можно лично, курьером (Яндекс.доставка, Dostavista и пр.) или транспортной компанией (СДЕК, DPD, MagicTrans и пр.) При заборе товара и лично и особенно курьером сообщайте номер вашего заказа  #{String.Format("{0:0000}", _order.OrderNumber)} без озвученного номера заказа, товар курьеру не отдается.");
                num = cell1.Value.ToString().IndexOf("#");
                cell1.RichText.Substring(num, 6).SetFontColor(XLColor.Red).SetFontSize(11).SetBold(true);
                num = cell1.Value.ToString().IndexOf("особенно");
                cell1.RichText.Substring(num, 17).SetFontColor(XLColor.Red).SetFontSize(11).SetBold(true);

                wb.SaveAs($"{fullPath}_00.xlsx");
                filePairs.Add(new filePair { xlsx = $"{fullPath}_00.xlsx", pdf = $"{fullPath}_00.pdf" });
            }
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            foreach (var item in _order.OrderItems)
            {
                using (var wb = new XLWorkbook())
                {
                    TemplateWorkBook.Worksheet("Sheet3").CopyTo(wb, $"Sheet3-{item.Id}");

                    string tmp = $"Sheet3-{item.Id}";
                    IXLWorksheet sheet = await FillReference(wb.Worksheet(tmp), item);

                    wb.SaveAs($"{fullPath}_1_{item.Id}.xlsx");
                }

                filePairs.Add(new filePair { xlsx = $"{fullPath}_1_{item.Id}.xlsx", pdf = $"{fullPath}_1_{item.Id}.pdf" });

            }

            _logger.LogInformation("orderType{0}", _order.OrderType);

            if (_order.OrderType > 1)
            {
                using (var wb = new XLWorkbook())
                {
                    try
                    {
                        var res = TemplateWorkBook.Worksheet("Sheet4").CopyTo(wb, "Sheet4");
                        _logger.LogInformation("loadto{0}", res.FirstCell().ToString());
                        IXLWorksheet sheet = await FillPay(wb.Worksheet("Sheet4"), _order);
                        _logger.LogInformation("loadto{0}", 2);
                        wb.SaveAs($"{fullPath}_2.xlsx");
                        _logger.LogInformation("loadto{0}", 3);
                    }
                    catch (InvalidCastException e)
                    {
                        _logger.LogInformation("Exeption - {0}", e);
                    }

                }
                filePairs.Add(new filePair { xlsx = $"{fullPath}_2.xlsx", pdf = $"{fullPath}__2.pdf" });

                _logger.LogInformation("loadto{0}", 3);
                using (var wb = new XLWorkbook())
                {
                    _logger.LogInformation("loadto{0}", 4);
                    TemplateWorkBook.Worksheet("Sheet5").CopyTo(wb, "Sheet5");
                    _logger.LogInformation("loadto{0}", 5);
                    IXLWorksheet sheet = await FillOrder2(wb.Worksheet("Sheet5"), _order);
                    _logger.LogInformation("loadto{0}", 6);
                    wb.SaveAs($"{fullPath}_22.xlsx");
                    _logger.LogInformation("loadto{0}", 7);
                }
                filePairs.Add(new filePair { xlsx = $"{fullPath}_22.xlsx", pdf = $"{fullPath}__22.pdf" });

            }

            _logger.LogInformation("Starting pdf{0}", 1);
            foreach (var item in filePairs)
            {
                _logger.LogInformation("convert: {0} to {1}", item.xlsx, item.pdf);
                try
                {
                    ExcelFile wb1 = ExcelFile.Load(item.xlsx);
                    wb1.Save(item.pdf, po);
                }
                catch (InvalidCastException e)
                {
                    _logger.LogInformation("Exeption - {0}", e);
                }

            }


            _logger.LogInformation("Starting pdf{0}", 1);
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = path + fileName + "_2.pdf";
            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

            //Open the output file
            sourceDocument.Open();

            foreach (var item in filePairs)
            {
                reader = new PdfReader(item.pdf);
                importedPage = pdfCopyProvider.GetImportedPage(reader, 1);
                pdfCopyProvider.AddPage(importedPage);
                if (item.pdf.Contains("_0.pdf"))
                {

                    for (int i = 2; i <= reader.NumberOfPages; i++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);
                    }


                }
                reader.Close();
            }
            sourceDocument.Close();
            _logger.LogInformation("Starting pdf{0}", 2);
            if (_order.OrderType > 1)
            {

                using (var wb = new XLWorkbook($"{fullPath}_2.xlsx"))
                {
                    _logger.LogInformation("Starting pdf{0}", 3);
                    TemplateWorkBook = new XLWorkbook($"{fullPath}_22.xlsx");
                    _logger.LogInformation("Starting pdf{0}", 4);
                    TemplateWorkBook.Worksheet("Sheet5").CopyTo(wb, "Sheet5");
                    _logger.LogInformation("Starting pdf{0}", 5);

                    _logger.LogInformation("Starting pdf{0}", 8);
                    if (File.Exists($"{fullPath}_2.xlsx")) { File.Delete($"{fullPath}_2.xlsx"); }
                    _logger.LogInformation("Starting pdf{0}", 9);

                    wb.SaveAs($"{fullPath}_2.xlsx");
                }

            }

            foreach (var file in filePairs)
            {
                if (File.Exists(file.pdf) && !file.pdf.Contains("_2.pdf")) { File.Delete(file.pdf); }
                if (File.Exists(file.xlsx) && !file.xlsx.Contains("_2.xlsx")) { File.Delete(file.xlsx); }
            }
            if (File.Exists($"{fullPath}__2.pdf")) { File.Delete($"{fullPath}__2.pdf"); }
        }
        return _order;

    }




}

public class filePair
{
    public string xlsx { get; set; }
    public string pdf { get; set; }
}
