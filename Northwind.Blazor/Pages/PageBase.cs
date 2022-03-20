using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Northwind.Blazor.Pages
{
    public class PageBase : ComponentBase
    {
        [Inject]
        public ISnackbar Snackbar { get; set; }

        public bool IsLoading { get; set; }
        public bool HasError { get; set; }
        public void Notify(string message, Severity severity)
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopEnd;
            Snackbar.Add(message, severity);
        }
    }
}
