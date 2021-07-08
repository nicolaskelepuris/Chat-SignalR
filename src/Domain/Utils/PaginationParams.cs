using System;

namespace Domain.Utils
{
    public class PaginationParams
    {
        private const int MaxPageSize = 20;
        private const int MinPageSize = 2;
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = Math.Min(MaxPageSize, Math.Max(MinPageSize, value));
            }
        }
    }
}