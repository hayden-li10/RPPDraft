using Elsa.Http;
using Elsa.Scheduling.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using System;

namespace RPP.Orchestration.Workflows;

public class DelayedResumeTestWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Name = "Test: Delayed Resume";
        builder.Description = "Tests if elsa correctly pauses and resumes a workflow.";

        builder.Root = new Sequence
        {
            Activities =
            {
                new HttpEndpoint
                {
                    Path = new("/test/delay"),
                    SupportedMethods = new(new[] { "POST" }),
                    CanStartWorkflow = true
                },
                new WriteLine("Workflow started, Pausing for 10 seconds..."),
                
                new Delay(TimeSpan.FromSeconds(10)),

                new WriteLine("Workflow resumed, works perfectly")
            }
        };
    }
}
