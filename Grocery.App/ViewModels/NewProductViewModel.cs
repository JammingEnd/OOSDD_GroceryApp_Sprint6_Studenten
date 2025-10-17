
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int amount;

        [ObservableProperty]
        private DateTime shelfLife = DateTime.Today;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private bool isMessageVisible;

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
            Amount = 0;
            ShelfLife = DateTime.Today;
        }

        [RelayCommand]
        private async Task AddProduct()
        {
            IsMessageVisible = false;
            if (string.IsNullOrWhiteSpace(Name) || Amount <= 0 || ShelfLife == DateTime.Today)
            {
                Message = "Vul alle velden in om een product toe te voegen.";
                IsMessageVisible = true;
                return;
            }

            var S_Life = DateOnly.FromDateTime(ShelfLife);

            var product = new Product(0, Name, Amount, S_Life);

            _productService.Add(product);
            Message = "Product succesvol toegevoegd!";
            IsMessageVisible = true;

            await Shell.Current.GoToAsync(nameof(ProductView));
        }
    }

}
