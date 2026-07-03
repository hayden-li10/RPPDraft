using RPP.Application.DTOs;
using RPP.Application.Interfaces;
using System.Collections.Generic;

namespace RPP.Application.BusinessEngine
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
