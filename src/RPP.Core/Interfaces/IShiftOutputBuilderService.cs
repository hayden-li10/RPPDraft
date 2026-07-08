using RPP.Core.DTOs;
using System.Collections.Generic;

namespace RPP.Core.Interfaces
{
    public interface IShiftOutputBuilderService
    {
        List<ModulePayload> BuildOutputs(List<ModulePayload> payloads);
    }
}
