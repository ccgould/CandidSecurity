using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CandidQVmMulti.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        public ObservableCollection<FlyoutItemModel> FlyoutItems { get; private set; } = new();

        public partial class FlyoutItemModel : ObservableObject
        {
            [ObservableProperty] private string title;
            [ObservableProperty] private string iconGlyph;
            [ObservableProperty] private string targetRoute;
            [ObservableProperty] private bool isSelected;
        }

        public AppShellViewModel()
        {

            FlyoutItems = new ObservableCollection<FlyoutItemModel>
        {
            new FlyoutItemModel { Title = "Dashboard", IconGlyph = "\uf015;", TargetRoute = "//MainPage", IsSelected=true},
            new FlyoutItemModel { Title = "Vouchers", IconGlyph = "\uf15c;", TargetRoute = "//VouchersPage" },
            new FlyoutItemModel { Title = "Employees", IconGlyph = "\uf007;", TargetRoute = "//EmployeesPage" },
            new FlyoutItemModel { Title = "Airlines", IconGlyph = "\uf072;", TargetRoute = "//AirlinesPage" }
        };
        }

        internal void RefreshMenu(object? sender, ShellNavigatingEventArgs e)
        {
            foreach (var i in FlyoutItems) i.IsSelected = false;
            var item = FlyoutItems.FirstOrDefault(x => x.TargetRoute.Equals(e.Target.Location.OriginalString));

            if(item is not null) item.IsSelected= true;
        }

        public ICommand NavigateCommand => new Command<FlyoutItemModel>(async (item) =>
        {
            if (!string.IsNullOrEmpty(item.TargetRoute))
            {
                await Shell.Current.GoToAsync(item.TargetRoute);
            }
        });
    }
}
