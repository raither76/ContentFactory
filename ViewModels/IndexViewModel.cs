using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentFactory.ViewModels
{
    public class IndexViewModel
    {
        private readonly ApplicationDbContext _context;

        public IndexViewModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<TitleRow>> GetTitleModels(int size)
        {
            List<Model> mod = await _context.Models.Include(im => im.ModelImages).Where(x => x.IsHide == false && x.WorkDate > DateTime.Now).OrderBy(c => c.WorkDate).AsNoTracking().ToListAsync();

            Rows = await new TitleRow(mod, size).GroupModels();

            return Rows;
        }

        public List<TitleRow> Rows { get; set; } = new List<TitleRow>();


    }
    public class TitleRow
    {
        public TitleRow()
        { }
        private int RowSize { get; set; }
        private int RowCount { get; set; }
        public int CurrentRow { get; set; }
        private int Count { get; set; }
        private List<Model> _allmodels { get; set; } = new List<Model>();
        public List<Model> _groupModels { get; set; } = new List<Model>();
        public TitleRow(List<Model> models, int rowSize)
        {
            Count = models.Count;
            RowCount = (int)Math.Ceiling(Count / (double)rowSize);
            RowSize = rowSize;
            _allmodels = models;

        }
        public async Task<List<TitleRow>> GroupModels()
        {
            List<TitleRow> rows = new List<TitleRow>();
            for (int i = 0; i < RowCount; i++)
            {
                TitleRow row = new TitleRow()
                {
                    CurrentRow = i,
                    _groupModels = _allmodels.Skip(i * RowSize).Take(RowSize).ToList()
                };
                rows.Add(row);

            }
            return rows;
        }

    }
}
