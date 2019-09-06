using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFBase
{
    public class PagedList<T> : IPagedList where T: class
    {
        public PagedList()
        {
            Data = new List<T>();
        }

        public PagedList(IList<T> items, int pageIndex, int pageSize)
            : this()
        {
            PageSize = pageSize;
            TotalItemCount = items.Count;
            CurrentPageIndex = pageIndex;
            for (int i = StartRecordIndex - 1; i < EndRecordIndex; i++)
            {
                this.Data.Add(items[i]);
            }
        }

        public PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalItemCount)
            : this()
        {
            this.Data.AddRange(items);
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            CurrentPageIndex = pageIndex;
        }
        public int CurrentPageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public int TotalItemCount
        {
            get;
            set;
        }

        public int TotalPageCount
        {
            get
            {
                return (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            }
        }

        public int StartRecordIndex { get { return (CurrentPageIndex - 1) * PageSize + 1; } }

        public int EndRecordIndex { get { return TotalItemCount > CurrentPageIndex * PageSize ? CurrentPageIndex * PageSize : TotalItemCount; } }

        public List<T> Data { get; set; }
    }

    public static class PageLinqExtensions
    {
        public static PagedList<T> ToPagedList<T, S>(this IQueryable<T> items, int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, S>> orderBy = null, bool bUp = false) where T : class
        {
            if (pageIndex < 1)
                pageIndex = 1;

            var itemIndex = (pageIndex - 1) * pageSize;
            IQueryable<T> pageOfItems = null;
            if(orderBy == null)
            {
                pageOfItems = items.OrderBy(a => a).Skip(itemIndex).Take(pageSize);
            }
            else
            {
                pageOfItems = bUp ? items.OrderBy(orderBy).Skip(itemIndex).Take(pageSize) :
                                    items.OrderByDescending(orderBy).Skip(itemIndex).Take(pageSize);
            }
         
            var totalItemCount = items.Count();
            return new PagedList<T>(pageOfItems, pageIndex, pageSize, totalItemCount);
        }

        //public static PagedList<T> ToPagedList<T, S1, S2>(this IQueryable<T> items, int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, S1>> orderBy, bool bUp, System.Linq.Expressions.Expression<Func<T, S2>> thenBy, bool bUp2)
        //{
        //    if (pageIndex < 1)
        //        pageIndex = 1;

        //    var itemIndex = (pageIndex - 1) * pageSize;
        //    var tmpPageOfItems = bUp ? items.OrderBy(orderBy) : items.OrderByDescending(orderBy);
        //    var thenPageOfItems = bUp2 ? tmpPageOfItems.ThenBy(thenBy) : tmpPageOfItems.ThenByDescending(thenBy);
        //    var pageOfItems = thenPageOfItems.Skip(itemIndex).Take(pageSize);
        //    var totalItemCount = items.Count();
        //    return new PagedList<T>(pageOfItems, pageIndex, pageSize, totalItemCount);
        //}
    }
}
