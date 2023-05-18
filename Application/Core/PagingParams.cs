using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{
    public class PagingParams
    {
        private const int MaxPageSize = 50;
        public int pageNumber { get; set; } = 1;

        private int _pagesize = 10;

        private bool _sort = true;

        private String _sort_on_field = null;

        public int PageSize
        {
            get => _pagesize;
            set => _pagesize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public bool Sort { 
            get => _sort; 
            set => _sort = value; 
        }

        public String sortOnField { 
            get => _sort_on_field; 
            set => _sort_on_field = value; 
        }
        
    }
}