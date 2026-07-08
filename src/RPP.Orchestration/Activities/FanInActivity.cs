using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using RPP.Core.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RPP.Orchestration.Activities
{
    [Activity(Type = "FanInActivity", Category = "SchedulingEngine", Description = "Merges child outputs and prints the result.")]
    public class FanInActivity : CodeActivity
    {
        [Input]
        public Input<IDictionary<string, object>> ChildOutput1 { get; set; } = default!;

        [Input]
        public Input<IDictionary<string, object>> ChildOutput2 { get; set; } = default!;

        // The helper method is now safely encapsulated inside the compiled activity
        private List<ModulePayload> ExtractPayloads(IDictionary<string, object> dict)
        {
            if (dict != null && dict.TryGetValue("ChildOutput", out var rawValue) && rawValue != null)
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

        protected override void Execute(ActivityExecutionContext context)
        {
            // Extract the inputs from the workflow variables
            var dict1 = context.Get(ChildOutput1) ?? new Dictionary<string, object>();
            var dict2 = context.Get(ChildOutput2) ?? new Dictionary<string, object>();

            // Process the payloads
            var listA = ExtractPayloads(dict1);
            var listB = ExtractPayloads(dict2);
            var mergedList = listA.Concat(listB).ToList();

            var testNumbersA = listA.Select(p => p.TestNumber?.ToString() ?? "null");
            var testNumbersB = listB.Select(p => p.TestNumber?.ToString() ?? "null");
            var combinedNumbers = mergedList.Select(p => p.TestNumber?.ToString() ?? "null");

            // Print to console
            System.Console.WriteLine($"[PARENT] Both children finished!");
            System.Console.WriteLine($"- Output A Test Numbers: [{string.Join(", ", testNumbersA)}]");
            System.Console.WriteLine($"- Output B Test Numbers: [{string.Join(", ", testNumbersB)}]");
            System.Console.WriteLine($"- Combined Test Numbers: [{string.Join(", ", combinedNumbers)}]");
        }
    }
}
