using BlazorERP.Data.Context;
using BlazorERP.Data.Entities;
using BlazorERP.Modules.IMS.Components.Dialogs;
using BlazorERP.Modules.Services;
using BlazorERP.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.Pages;

public class StorageManagementBase : ComponentBase
{
    
    
    protected List<StorageLocation> _stores { get; set; } = new List<StorageLocation>();
    protected StorageLocation? _selectedStore { get; set; }
    protected List<Stock>? _selectedStoreStocks { get; set; }
    
    [Inject]
    protected ImsService Ims { get; private set; }
    [Inject]
    protected ISnackbar Snackbar { get; private set; }
    [Inject]
    protected IDialogService Dialog { get; private set; }
    [Inject]
    private proContext _context { get; set; } // Injected for loading references
    
    // Chart Values
    public string[] ChartLabels { get; set; } = Array.Empty<string>();
    public double[] ChartValues { get; set; } = Array.Empty<double>();
    
    // Secondary Button Params
    protected string SecondaryButtonText = "Deactivate";
    protected string SecondaryButtonIcon = Icons.Material.Filled.Error;
    protected Color SecondaryButtonColor { get; set; } = Color.Error;
    protected EventCallback SecondaryButtonClick { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
        
        // Load storage locations
        _stores = await Ims.GetAllStorageLocations(includeStocks: true);
    }
    
    private async Task SetSecondaryButtonClick()
    {
        if (_selectedStore.IsActive)
        {
            SecondaryButtonText = "Deactivate";
            SecondaryButtonIcon = Icons.Material.Filled.Error;
            SecondaryButtonColor = Color.Error;
            SecondaryButtonClick = EventCallback.Factory.Create(this, DeactivateStorageLocation);   
        } else if (!_selectedStore.IsActive)
        {
            SecondaryButtonText = "Reactivate";
            SecondaryButtonIcon = Icons.Material.Filled.Check;
            SecondaryButtonColor = Color.Info;
            SecondaryButtonClick = EventCallback.Factory.Create(this, ReactivateStorageLocation);
        }
    }
    
    public async Task LocationSelected(TableRowClickEventArgs<StorageLocation> args)
    {
        StorageLocation clicked_item = args.Item;
        if (_selectedStore != clicked_item) 
        {
            // Set Item
            _selectedStore = clicked_item;
            
            // Get Stock Chart Data
            await _context.LoadCollectionsAsync<StorageLocation, Stock>(
                _selectedStore, 
                x => x.Stocks,
                f => f.StorageLocationId == _selectedStore.StorageLocationId);
            
            _selectedStoreStocks = _selectedStore.Stocks.ToList();
            
            // Get Chart Values
            ChartLabels = _selectedStore.Stocks
                .Select(stock => stock.Item?.Name ?? "Unknown Item")
                .ToArray();
            
            ChartValues = _selectedStore.Stocks
                .Select(stock => (double)stock.OnHand)
                .ToArray();
        }
    }
    
    public async Task SaveStorageLocation()
    {
        if (_selectedStore is null)
            throw new InvalidOperationException("_selectedStore was null when trying to update storage location.");
        
        // Save the storage location
        await Ims.SaveStorageLocation(_selectedStore);
        
        // Refresh the list of storage locations
        _stores = await Ims.GetAllStorageLocations(includeStocks: true);
    }
    public async Task OpenNewStorageLocationDialog()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            BackdropClick = true,
            Position = DialogPosition.Center
        };

        var dialog = await Dialog.ShowAsync<_AddStorageLocation>("New Storage Location", options: options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            _stores = await Ims.GetAllStorageLocations(includeStocks: true);
            Snackbar.Add("Storage location added", Severity.Success);
        }
    }

    private async Task ReactivateStorageLocation()
    {
        if (_selectedStore is null)
            throw new ArgumentNullException(nameof(_selectedStore), "Storage location cannot be null.");
        
        bool? confirm = await Dialog.ShowMessageBox("Warning", "Are you sure you want to reactivate this storage location?",
            yesText: "Reactivate", noText: "Cancel", options: new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = true, Position = DialogPosition.Center });
        
        if (confirm != true)
        {
            Snackbar.Add("Reactivation cancelled", Severity.Warning);
            return;
        }
        
        // Reactivate the storage location
        _selectedStore.IsActive = true;
        await Ims.SaveStorageLocation(_selectedStore);
        
        // Refresh the list of storage locations
        _stores = await Ims.GetAllStorageLocations(includeStocks: true);
        
        Snackbar.Add("Storage location reactivated", Severity.Success);
    }
    
    public async Task DeactivateStorageLocation()
    {
        if (_selectedStore is null)
            throw new ArgumentNullException(nameof(_selectedStore), "Storage location cannot be null.");
        
        bool? confirm = await Dialog.ShowMessageBox("Warning", "Once deactivating a storage location, it will remain inactive until all stock has been removed. At this point the storage location will be deleted automatically. Are you sure you want to deactivate this storage location?",
            yesText: "Deactivate", noText: "Cancel", options: new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = true, Position = DialogPosition.Center });
        
        if (confirm != true)
        {
            Snackbar.Add("Deactivation cancelled", Severity.Warning);
            return;
        }
        
        // Deactivate the storage location
        _selectedStore.IsActive = false;
        await Ims.SaveStorageLocation(_selectedStore);
        
        // Refresh the list of storage locations
        _stores = await Ims.GetAllStorageLocations(includeStocks: true);
        
        Snackbar.Add("Storage location deactivated", Severity.Success);
    }
}