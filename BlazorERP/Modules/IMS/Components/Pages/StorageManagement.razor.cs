using BlazorERP.Data.Entities;
using BlazorERP.Modules.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.Pages;

public class StorageManagementBase : ComponentBase
{
    
    
    protected List<StorageLocation> _stores { get; set; } = new List<StorageLocation>();
    protected StorageLocation? _selectedStore { get; set; }
    
    [Inject]
    protected ImsService Ims { get; private set; }
    [Inject]
    protected ISnackbar Snackbar { get; private set; }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
        
        // Load storage locations
        _stores = await Ims.GetStorageLocations(includeStocks: true);
    }
    
    public async Task SaveStorageLocation()
    {
        if (_selectedStore is null)
            throw new InvalidOperationException("_selectedStore was null when trying to update storage location.");
        
        // Save the storage location
        await Ims.SaveStorageLocation(_selectedStore);
        
        // Refresh the list of storage locations
        _stores = await Ims.GetStorageLocations(includeStocks: true);
    }
}