using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Elsa.Workflows.Models;
using RPP.Orchestration.Activities;
//using Elsa.Scheduling.Activities.Timer;
using RPP.Application.DTOs;
namespace RPP.Orchestration.Workflows
{
    public class PipelineWorkflow : WorkflowBase
    {
        protected override void Build(IWorkflowBuilder builder)
        {
            builder.Name = "Scheduling RPP Pipeline";

            var payloadList = builder.WithVariable<List<ModulePayload>>("payloadList", new List<ModulePayload>());

            var shiftBaselineGeneration = new ShiftBaselineGeneration()
            {
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            var assetAllocation = new AssetAllocation()
            {
                Inputs = new Input<List<ModulePayload>>(payloadList),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            var shiftOutputBuilder = new ShiftOutputBuilder()
            {
                Inputs = new Input<List<ModulePayload>>(payloadList),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            var enrichmentA = new Enrichment()
            {
                Inputs = new Input<List<ModulePayload>>(payloadList),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            var enrichmentB = new Enrichment()
            {
                Inputs = new Input<List<ModulePayload>>(payloadList),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            builder.Root = new Flowchart
            {
                Activities =
            {
                shiftBaselineGeneration,
                assetAllocation,
                enrichmentA,
                enrichmentB,
                shiftOutputBuilder
            },
                Connections =
            {
                new(shiftBaselineGeneration, enrichmentA),
                new(enrichmentA, enrichmentB),
                new(enrichmentB, assetAllocation),
                new(assetAllocation, shiftOutputBuilder)
            }
            };
        }
    }
}
