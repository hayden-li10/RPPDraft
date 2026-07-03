using RPP.Application.DTOs;
using System.Collections.Generic;

namespace RPP.Application.Interfaces
{
    public interface IAssetAllocationService
    {
        List<ModulePayload> AllocateAssets(List<ModulePayload> payloads);
    }
}
