using System.Collections.Generic;
using Application.Responses.Base;

namespace Application.Responses.Pagination
{
    public class PaginationResponse<T> : BaseResponse
    {
        public IReadOnlyList<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int ItemsCount { get; set; }
    }
}