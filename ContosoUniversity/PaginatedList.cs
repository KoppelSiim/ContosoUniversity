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

        /*
         The CreateAsync method in this code takes page size and page number and applies the appropriate Skip and Take statements to the IQueryable.
         When ToListAsync is called on the IQueryable, it will return a List containing only the requested page.
         The properties HasPreviousPage and HasNextPage can be used to enable or disable Previous and Next paging buttons.
         A CreateAsync method is used instead of a constructor to create the PaginatedList<T> object because constructors can't run asynchronous code.
        */
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // Calculate the total count of items in the source asynchronously using CountAsync()
            var count = await source.CountAsync();

            // Skip() - skip a specified number of elements in a collection and return the remaining elements.
            // Take() - take a specified number of elements from a collection and return them as a new collection.
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
