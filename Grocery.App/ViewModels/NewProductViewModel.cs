using Grocery.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grocery.App.ViewModels
{
    public abstract class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
        }
    }
}
