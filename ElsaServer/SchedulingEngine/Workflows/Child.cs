using Elsa.Extensions;
using Elsa.Scheduling.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Elsa.Workflows.Management.Activities.SetOutput;
using Elsa.Workflows.Memory;
using Elsa.Workflows.Models;
using SchedulingEngine.BusinessEngine.Models;
using SchedulingEngine.BusinessEngine.Modules;
using System.Text.Json;
namespace ElsaServer.SchedulingEngine.Workflows
{

    public class Child : WorkflowBase
    {
        protected override void Build(IWorkflowBuilder builder)
        {
            builder.Name = "Child";
            builder.Id = "Child";
            var payloadList = builder.WithVariable<List<ModulePayload>>("payloadList", new List<ModulePayload>());
            var output = builder.WithOutput<List<ModulePayload>>("ChildOutput");

            var logNodeInfo = new WriteLine(context =>
                $"[CHILD] Executing on Machine (Pod): {Environment.MachineName} | Background Thread: {Thread.CurrentThread.ManagedThreadId}");
            logNodeInfo.SetDisplayText("log node info");

            var randomDelay = new Inline(async context =>
            {
                var random = new Random();
                int delaySeconds = random.Next(0, 11);

                System.Console.WriteLine($"[CHILD] Starting delay: Waiting for {delaySeconds} seconds...");
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), context.CancellationToken);
                System.Console.WriteLine($"[CHILD] Delay finished! Continuing execution...");
            });
            randomDelay.SetDisplayText("Delay random seconds");

            var printMessage = new WriteLine(context =>
            {
                var incomingList = context.GetInput<List<ModulePayload>>("ParentMessage") ?? new List<ModulePayload>();
                var testNumbers = incomingList
                    .Select(p => p.TestNumber?.ToString() ?? "null");

                return $"[CHILD] Received Test Numbers: {string.Join(", ", testNumbers)}";
            });
            printMessage.SetDisplayText("log input");

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

            var setOutput = new SetOutput
            {
                OutputName = new(output.Name),
                OutputValue = new Input<object?>((Variable)payloadList),
            };

            builder.Root = new Flowchart
            {
                Activities =
                {
                    logNodeInfo,
                    randomDelay,
                    printMessage,
                    enrichment1,
                    enrichment2,
                    assetAllocation,
                    setOutput
                },
                Connections =
                {
                    new(logNodeInfo, randomDelay),
                    new(randomDelay, printMessage),
                    new(printMessage, enrichment1),
                    new(enrichment1, enrichment2),
                    new(enrichment2, assetAllocation),
                    new(assetAllocation, setOutput)
                }
            };
        }
    }
}