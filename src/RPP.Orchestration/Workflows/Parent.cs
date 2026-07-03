using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Runtime.Activities;
using RPP.Application.DTOs;
using RPP.Orchestration.Activities;
namespace RPP.Orchestration.Workflows
{
    public class Parent : WorkflowBase
    {
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
                        Input = new(new Dictionary<string, object> { ["ParentMessage"] = allPayloads.Take(3).ToList() }),
                        WaitForCompletion = new(true),
                        Result = new(childOutput1)
                    },
                    new DispatchWorkflow
                    {
                        WorkflowDefinitionId = new(nameof(Child)),
                        Input = new(new Dictionary<string, object> { ["ParentMessage"] = allPayloads.Skip(3).Take(3).ToList() }),
                        WaitForCompletion = new(true),
                        Result = new(childOutput2)
                    }
                }
            };
            var fanIn = new FanInActivity
            {
                ChildOutput1 = new(childOutput1),
                ChildOutput2 = new(childOutput2)
            };

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