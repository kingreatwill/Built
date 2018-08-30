using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Mongo
{
    public interface IPagedResult<T>
    {
        /// <summary>
        /// List of items.
        /// </summary>
        IReadOnlyList<T> Items { get; set; }

        /// <summary>
        /// Total count of Items.
        /// </summary>
        long TotalCount { get; set; }
    }

    public class PagedResult<T> : IPagedResult<T>
    {
        /// <summary>
        /// Total count of Items.
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// List of items.
        /// </summary>
        public IReadOnlyList<T> Items
        {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }

        private IReadOnlyList<T> _items;

        /// <summary>
        /// Creates a new <see cref="PagedResult{T}"/> object.
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// Creates a new <see cref="PagedResult{T}"/> object.
        /// </summary>
        /// <param name="totalCount">Total count of Items</param>
        /// <param name="items">List of items in current page</param>
        public PagedResult(long totalCount, IReadOnlyList<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }

    public class PagedResultDto<T> : PagedResult<T>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling(TotalCount * 1.0 / PageSize);
            }
        }

        public bool FirstPage
        {
            get
            {
                return PageIndex == 0;
            }
        }

        public bool LastPage
        {
            get
            {
                return PageCount == (PageIndex + 1);
            }
        }

        public PagedResultDto(int index, int size, long totalCount, IReadOnlyList<T> items)
        {
            PageIndex = index;
            PageSize = size;
            TotalCount = totalCount;
            Items = items;
        }
    }
}