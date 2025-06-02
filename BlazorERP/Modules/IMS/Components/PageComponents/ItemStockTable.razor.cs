using BlazorERP.Data.Entities;
using BlazorERP.Modules.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.PageComponents;

public class ItemStockTableBase : ComponentBase
{
    [Parameter]
    public required Item Item { get; set; }
    [Parameter]
    public Stock? SelectedStock { get; set; }
    
    [Inject]
    protected ImsService Ims { get; private set; }
    
    [Inject]
    protected ISnackbar Snackbar { get; private set; }
    
    public async Task SaveStock()
    {
        // Save the stock for the selected item
        await Ims.SaveStock(SelectedStock); ;

        // Set SelectedStock to null after saving
        SelectedStock = null;
        
        Snackbar.Add("Stock saved", Severity.Success);
    }
}

