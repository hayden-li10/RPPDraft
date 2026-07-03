using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using RPP.Application.DTOs;
using RPP.Application.Interfaces;

namespace RPP.Orchestration.Activities;

[Activity(Type = "ShiftBaselineGeneration", Category = "SchedulingEngine", Description = "Generate shift baseline.")]
public class ShiftBaselineGeneration : CodeActivity
{
    [Output]
    public Output<List<ModulePayload>> Outputs { get; set; } = new();

    protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var result = context.GetRequiredService<IShiftBaselineGenerationService>().GenerateBaseline();
        context.Set(Outputs, result);
        return ValueTask.CompletedTask;
    }
}



