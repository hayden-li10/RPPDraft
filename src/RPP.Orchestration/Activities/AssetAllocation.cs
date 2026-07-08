using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using RPP.Core.DTOs;
using RPP.Core.Interfaces;

namespace RPP.Orchestration.Activities;

[Activity(Type = "AssetAllocation", Category = "SchedulingEngine", Description = "Allocate assets.")]
public class AssetAllocation : CodeActivity
{
    [Input] public Input<List<ModulePayload>> Inputs { get; set; } = default!;
    [Output] public Output<List<ModulePayload>> Outputs { get; set; } = new();

    protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var list = context.Get(Inputs) ?? new List<ModulePayload>();

        var service = context.GetRequiredService<IAssetAllocationService>();

        var payloads = service.AllocateAssets(list);

        context.Set(Outputs, payloads);
        return ValueTask.CompletedTask;
    }
}