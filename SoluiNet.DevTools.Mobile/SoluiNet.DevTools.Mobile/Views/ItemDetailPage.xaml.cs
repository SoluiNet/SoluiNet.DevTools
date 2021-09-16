using SoluiNet.DevTools.Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace SoluiNet.DevTools.Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}