using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        // Return true if the PageIndex is greater than 1, meaning there are previous pages.
        public bool HasPreviousPage => PageIndex > 1;

        // Return true if the PageIndex is less than TotalPages, meaning there are more pages ahead.
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
