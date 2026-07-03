using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using RPP.Application.DTOs;
using RPP.Application.Interfaces;

namespace RPP.Orchestration.Activities;

[Activity(Type = "ShiftOutputBuilder", Category = "SchedulingEngine", Description = "Build final output.")]
public class ShiftOutputBuilder : CodeActivity
{
    [Input] public Input<List<ModulePayload>> Inputs { get; set; } = default!;
    [Output] public Output<List<ModulePayload>> Outputs { get; set; } = new();

    protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var list = context.Get(Inputs) ?? new List<ModulePayload>();

        var service = context.GetRequiredService<IShiftOutputBuilderService>();

        var payloads = service.BuildOutputs(list);

        context.Set(Outputs, payloads);
        return ValueTask.CompletedTask;
    }
}



