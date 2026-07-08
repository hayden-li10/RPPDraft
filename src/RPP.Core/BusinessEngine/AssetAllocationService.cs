using RPP.Core.DTOs;
using RPP.Core.Interfaces;
using System.Collections.Generic;

namespace RPP.Core.BusinessEngine
{
    public class AssetAllocationService : IAssetAllocationService
    {
        public List<ModulePayload> AllocateAssets(List<ModulePayload> payloads)
        {
            var list = payloads ?? new List<ModulePayload>();

            foreach (var item in list)
            {
                item.TestNumber += 3;
            }

            return list;
        }
    }
}
