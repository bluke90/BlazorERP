using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.PageComponents;

public class ImsMenuBase : ComponentBase
{
    [Parameter] public bool IsOpen { get; set; } = false;
    [CascadingParameter] public Action? CloseMainDrawer { get; set; }
    public void DrawerToggle()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            CloseMainDrawer?.Invoke();
        }
    }
}