using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Elsa.Workflows.Management.Activities.SetOutput;
using Elsa.Workflows.Models;
using Elsa.Workflows.Runtime.Activities;
using Polly;
using SchedulingEngine.BusinessEngine.Models;
using SchedulingEngine.BusinessEngine.Modules;
using System.Text.Json;

namespace ElsaServer.SchedulingEngine.Workflows
{
    public class ChildWorkflow : WorkflowBase
    {
        protected override void Build(IWorkflowBuilder builder)
        {
            builder.Name = "Child Workflow";
            builder.Id = "Child Workflow";

            builder.WithInput<List<ModulePayload>>("ParentMessage");
            var payloadList = builder.WithVariable<List<ModulePayload>>("payloadList", new List<ModulePayload>());

            var writeline = new WriteLine(context =>
            {
                var incomingList = context.GetInput<List<ModulePayload>>("ParentMessage") ?? new List<ModulePayload>();
                var jsonString = JsonSerializer.Serialize(incomingList);
                return $"Child received message: {jsonString}";
            });

            var logNodeInfo = new WriteLine(context =>
               $"[CHILD] Executing on Machine (Pod): {Environment.MachineName} | Background Thread: {Thread.CurrentThread.ManagedThreadId}");

            var enrichment1 = new Enrichment()
            {
                Inputs = new Input<List<ModulePayload>>(context => context.GetInput<List<ModulePayload>>("ParentMessage") ?? new List<ModulePayload>()),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            var enrichment2 = new Enrichment()
            {
                Inputs = new Input<List<ModulePayload>>(payloadList),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            var assetAllocation = new AssetAllocation()
            {
                Inputs = new Input<List<ModulePayload>>(payloadList),
                Outputs = new Output<List<ModulePayload>>(payloadList)
            };

            
            builder.Root = new Flowchart
            {
                Activities =
                {
                    writeline,
                    logNodeInfo,
                    enrichment1,
                    enrichment2,
                    assetAllocation
                },
                Connections = 
                {
                    new (logNodeInfo, writeline),
                    new (writeline, enrichment1),
                    new (enrichment1, enrichment2),
                    new (enrichment2, assetAllocation),
                }
            };
        }
    }
}
