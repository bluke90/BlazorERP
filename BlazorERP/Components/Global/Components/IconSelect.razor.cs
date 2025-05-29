using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorERP.Components.Global.Components;

public class IconSelectBase : ComponentBase
{
    
    // Parameters
    [Parameter]
    public List<IconSelectItem> SelectItems { get; set; }
    [Parameter]
    public EventCallback<IconSelectItem> SelectItemChanged { get; set; }
    
    // Injects
    [Inject]
    public IJSRuntime JS { get; set; }
    
    // Properties
    public IconSelectItem SelectedItem { get; set; }
    public ElementReference InputSelect { get; set; }
    
    
    public async Task OnInputSelectChanged()
    {
        var selectedIndex = await JS.InvokeAsync<int>("getSelectIndex", InputSelect);
        
        if (selectedIndex < 0 || selectedIndex >= SelectItems.Count)
        {
            Console.WriteLine("Invalid select index: " + selectedIndex);
            return;
        }
        
        SelectedItem = SelectItems[selectedIndex];
        
        await SelectItemChanged.InvokeAsync(SelectedItem);
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("initSelect2WithIcons", "#icon-select");
        }
    }
    
}

// public class IconSelectItem

public class IconSelectItem
{
    public bool Default { get; set; } = false;
    public required MudIcon Icon { get; set; }
    public required object Value { get; set; }
    public required Type T { get; set; }
}