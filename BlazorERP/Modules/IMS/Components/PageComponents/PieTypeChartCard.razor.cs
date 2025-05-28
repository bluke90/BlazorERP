using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.PageComponents;

public class PieTypeChartCardBase : ComponentBase
{
    // Card Options
    [Parameter] public string Title { get; set; } = "Pie Chart";
    
    // Type 1 Chart Data
    [Parameter] public required string[] Labels { get; set; }
    [Parameter] public required double[] Values { get; set; }
    // [Parameter] public required string[] XAxisLabels { get; set; }
    // [Parameter] public required List<ChartSeries> ChartSeries { get; set; }
    
    // Type 1 Chart Options
    public Type1ChartOption ChartTypeOption { get; set; } = Type1ChartOption.Pie;
    public ChartType ChartType { get; set; } = ChartType.Pie;

    public void SetChartType(Type1ChartOption option)
    {
        ChartType = option switch
        {
            Type1ChartOption.Pie => ChartType.Pie,
            Type1ChartOption.Doughnut => ChartType.Donut,
            _ => ChartType.Pie
        };
    }
}




public enum Type1ChartOption
{
    Pie,
    Doughnut
}