using BlazorERP.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorERP.Components.Global.Components;

public class ImageDragDropBase : ComponentBase
{
    // Parameters for the component
    [Parameter] public string Height { get; set; } = "32vh";
    [Parameter] public string MaxHeight { get; set; } = "100%";
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public string MaxWidth { get; set; } = "50vh";
    
    // 2 way params
    [Parameter] public ErpFile? File { get; set; }
    [Parameter] public string? FileDataUrl { get; set; }
    
    // File upload related variables and methods
    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mud-width-full mud-height-full";
    protected string _dragClass = DefaultDragClass;
    protected MudFileUpload<IBrowserFile>? _fileUpload;

    public async Task ClearAsync()
    {
        await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);
        FileDataUrl = null;
        ClearDragClass();
    }

    protected Task OpenFilePickerAsync()
        => _fileUpload?.OpenFilePickerAsync() ?? Task.CompletedTask;

    public async Task OnFileInputChanged(InputFileChangeEventArgs e)
    {
        ClearDragClass();
        
        var file = e.File;

        // Get the file data URL
        await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10 MB limit
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        
        var bytes = ms.ToArray();
        
        // Set the file data URL for preview
        FileDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes, 0, bytes.Length)}";
        
        StateHasChanged();
        
        // Prepare the ErpFile object with the file details
        File = new ErpFile()
        {
            FileName = file.Name,
            FileExtension = Path.GetExtension(file.Name),
            MimeType = file.ContentType,
            Size = bytes.Length,
            CreatedAt = DateTime.UtcNow,
            Bytes = bytes
        };
    }
    
    // Method to set the drag class for the drop area
    public void SetDragClass()
        => _dragClass = $"{DefaultDragClass} mud-border-primary";

    public void ClearDragClass()
        => _dragClass = DefaultDragClass;

}