using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public interface IGridPage
    {
        int pageIndex { get; set; }
        
        int pageSize { get; set; }

    }
}
