using BlazorERP.Utilities.JsInterop.Models;
using Microsoft.JSInterop;

namespace BlazorERP.Utilities.JsInterop;

public static class LayoutInterop
{
    public static async Task<ScreenSize> GetScreenSize(this IJSRuntime jsRuntime)
    {
        return await jsRuntime.InvokeAsync<ScreenSize>("getScreenSize");
    }
}