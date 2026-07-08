using Elsa.Scheduling.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using System;

namespace RPP.Orchestration.Workflows;

public class TimerTestWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Name = "Test: 30-Second Timer";
        builder.Description = "Tests if elsa correctly triggers recurrent jobs every 30 seconds.";

        //builder.Root = new Sequence
        //{
        //    Activities =
        //    {
        //        new Elsa.Scheduling.Activities.Timer
        //        {
        //            Interval = new(TimeSpan.FromSeconds(30)),
        //            CanStartWorkflow = true
        //        },
        //        new WriteLine("Recurrent workflow triggered by timer every 30 seconds!")
        //    }
        //};
    }
}
