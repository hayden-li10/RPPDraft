using Elsa.Scheduling.Activities;
using Elsa.Workflows;
using Elsa.Workflows.Activities;
using IronCompress;

namespace RPP.Orchestration.Workflows;

public class CronTestWorkflow : WorkflowBase
{
    protected override void Build(IWorkflowBuilder builder)
    {
        builder.Name = "Test: Cron";
        builder.Description = "Tests if croncorrectly triggers recurrent jobs without double-firing.";

        builder.Root = new Sequence
        {
            Activities =
            {
                new Cron
                {
                    CronExpression = new("*/1 * * * *"),
                    CanStartWorkflow = true
                },
                new WriteLine("Recurrent cron workflow is triggered")
            }
        };
    }
}
