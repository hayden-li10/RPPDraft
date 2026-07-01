using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Runtime.Activities;
using SchedulingEngine.BusinessEngine.Models;
using System.Text.Json;
namespace ElsaServer.SchedulingEngine.Workflows
{
    public class Parent : WorkflowBase
    {
        // Helper to safely extract and reconstruct our custom List<ModulePayload>
        List<ModulePayload> ExtractPayloads(IDictionary<string, object> dict)
        {
            if (dict.TryGetValue("ChildOutput", out var rawValue) && rawValue != null)
            {
                if (rawValue is List<ModulePayload> typedList)
                {
                    return typedList;
                }

                var json = JsonSerializer.Serialize(rawValue);
                return JsonSerializer.Deserialize<List<ModulePayload>>(json) ?? new List<ModulePayload>();
            }
            return new List<ModulePayload>();
        }
        protected override void Build(IWorkflowBuilder builder)
        {
            builder.Name = "Parent";
            builder.Id = "Parent";

            var childOutput1 = builder.WithVariable<IDictionary<string, object>>();
            var childOutput2 = builder.WithVariable<IDictionary<string, object>>();

            var allPayloads = new List<ModulePayload>
            {
                new() { TestNumber = 1 }, new() { TestNumber = 2 }, new() { TestNumber = 3 },
                new() { TestNumber = 101 }, new() { TestNumber = 102 }, new() { TestNumber = 103 }
            };
            var chunk1 = allPayloads.Take(3).ToList();
            var chunk2 = allPayloads.Skip(3).Take(3).ToList();

            var fanOut = new Elsa.Workflows.Activities.Parallel
            {
                Activities =
                {
                    new DispatchWorkflow
                    {
                        WorkflowDefinitionId = new(nameof(Child)),
                        Input = new(new Dictionary<string, object> { ["ParentMessage"] = chunk1 }),
                        WaitForCompletion = new(true),
                        Result = new(childOutput1)
                    },
                    new DispatchWorkflow
                    {
                        WorkflowDefinitionId = new(nameof(Child)),
                        Input = new(new Dictionary<string, object> { ["ParentMessage"] = chunk2 }),
                        WaitForCompletion = new(true),
                        Result = new(childOutput2)
                    }
                }
            };

            var fanIn = new Inline(context =>
            {
                var dict1 = childOutput1.Get(context) ?? new Dictionary<string, object>();
                var dict2 = childOutput2.Get(context) ?? new Dictionary<string, object>();
                var listA = ExtractPayloads(dict1);
                var listB = ExtractPayloads(dict2);
                var mergedList = listA.Concat(listB).ToList();
                var testNumbersA = listA.Select(p => p.TestNumber?.ToString() ?? "null");
                var testNumbersB = listB.Select(p => p.TestNumber?.ToString() ?? "null");
                var combinedNumbers = mergedList.Select(p => p.TestNumber?.ToString() ?? "null");
                System.Console.WriteLine($"[PARENT] Both children finished!");
                System.Console.WriteLine($"- Output A Test Numbers: [{string.Join(", ", testNumbersA)}]");
                System.Console.WriteLine($"- Output B Test Numbers: [{string.Join(", ", testNumbersB)}]");
                System.Console.WriteLine($"- Combined Test Numbers: [{string.Join(", ", combinedNumbers)}]");
                return ValueTask.CompletedTask;
            });

            builder.Root = new Sequence
            {
                Activities =
            {
                fanOut,
                fanIn
            }
            };

            //!!!!!!IMPORTANT: Flowchart doesn't support the Parallel activity
            //builder.Root = new Flowchart
            //{
            //    Activities =
            //    {
            //        fanOut,
            //        fanIn
            //    },
            //    Connections =
            //    {
            //        new(fanOut, fanIn),
            //    },
            //    Variables =
            //    {
            //        childOutput1,
            //        childOutput2
            //    }
            //};
        }
    }
}