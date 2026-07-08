using RPP.Core.DTOs;
using System.Collections.Generic;

namespace RPP.Core.Interfaces
{
    public interface IShiftBaselineGenerationService
    {
        List<ModulePayload> GenerateBaseline();
    }
}
