using Microsoft.AspNetCore.Components;
using BlazorERP.Data.Entities;
using BlazorERP.Modules.Services;

public class ProductsBase : ComponentBase
{
    
    public List<Item> Items { get; set; }
    
    public Item SelectedItem { get; set; }

    public ImsService Ims { get; set; }

    public ProductsBase(ImsService ims)
    {
        Ims = ims;
    }

    public ProductsBase()
    {
        
    }
    
    protected override async Task OnInitializedAsync()
    {
        Items = await Ims.GetItems();
        
        await base.OnInitializedAsync();
    }
    

}
