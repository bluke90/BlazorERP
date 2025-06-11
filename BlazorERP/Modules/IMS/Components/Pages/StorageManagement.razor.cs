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
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
        
        // Load storage locations
        _stores = await Ims.GetAllStorageLocations(includeStocks: true);
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
}