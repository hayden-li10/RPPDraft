using RPP.Core.DTOs;
using System.Collections.Generic;

namespace RPP.Core.Interfaces
{
    public interface IAssetAllocationService
    {
        List<ModulePayload> AllocateAssets(List<ModulePayload> payloads);
    }
}
