namespace ContentFactory.Code
{
    public class Paginator
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int Count { get; set; }



        public Paginator(int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            RowCount = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;

            Count = count;

        }
        public int FirstRowOnPage
        {
            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

}
