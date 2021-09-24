using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class PageData<T> where T : IRowIndex
    {
        public List<T> DataSource { get; set; }

        public int TotalRows { get; set; } = 0;
    }
}
