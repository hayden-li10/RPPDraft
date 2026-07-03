using RPP.Application.DTOs;
using RPP.Application.Interfaces;
using System.Collections.Generic;

namespace RPP.Application.BusinessEngine
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
