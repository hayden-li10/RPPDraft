using RPP.Application.DTOs;
using System.Collections.Generic;

namespace RPP.Application.Interfaces
{
    public interface IShiftOutputBuilderService
    {
        List<ModulePayload> BuildOutputs(List<ModulePayload> payloads);
    }
}
