using RPP.Application.DTOs;
using System.Collections.Generic;

namespace RPP.Application.Interfaces
{
    public interface IShiftBaselineGenerationService
    {
        List<ModulePayload> GenerateBaseline();
    }
}
