using RPP.Core.DTOs;
using RPP.Core.Interfaces;
using System.Collections.Generic;

namespace RPP.Core.BusinessEngine
{
    public class ShiftBaselineGenerationService : IShiftBaselineGenerationService
    {
        public List<ModulePayload> GenerateBaseline()
        {
            var list = new List<ModulePayload>
            {
                new ModulePayload { TestNumber = 1},
                new ModulePayload { TestNumber = 2},
                new ModulePayload { TestNumber = 3},
            };

            return list;
        }
    }
}
