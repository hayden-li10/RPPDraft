using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using RPP.Application.DTOs;
using RPP.Application.Interfaces;

namespace RPP.Orchestration.Activities;

[Activity(Type = "PreEnrichment", Category = "SchedulingEngine", Description = "Add 2 to each number.")]
public class PreEnrichment : CodeActivity
{
    [Input] public Input<List<ModulePayload>> Inputs { get; set; } = default!;
    [Output] public Output<List<ModulePayload>> Outputs { get; set; } = new();

    protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var list = context.Get(Inputs) ?? new List<ModulePayload>();

        var service = context.GetRequiredService<IPreEnrichmentService>();

        var payloads = service.PreEnrich(list);

        context.Set(Outputs, payloads);
        return ValueTask.CompletedTask;
    }
}
