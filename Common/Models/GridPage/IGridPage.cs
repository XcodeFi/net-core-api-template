using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public interface IGridOrderBy
    {
        string fieldName { get; set; }
        
        bool isDesc { get; set; }

    }
}
