using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models.ViewModels
{
    public class ProductVM //this is viewmodel for access multiple models data in upsert as there are 3 models, but 1view=1model so here everymodel
                          ///is added in view model and it is accesed in upsert with models data included//
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> categoryList { get; set; }
        public IEnumerable<SelectListItem> coverTypeList { get; set; }
    }
}
