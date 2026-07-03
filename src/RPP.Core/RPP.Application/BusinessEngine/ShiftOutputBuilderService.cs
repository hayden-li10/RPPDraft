using RPP.Application.DTOs;
using RPP.Application.Interfaces;
using System.Collections.Generic;

namespace RPP.Application.BusinessEngine
{
    public class ShiftOutputBuilderService : IShiftOutputBuilderService
    {
        public List<ModulePayload> BuildOutputs(List<ModulePayload> payloads)
        {
            var list = payloads ?? new List<ModulePayload>();

            foreach (var item in list)
            {
                item.TestNumber += 5;
            }

            return list;
        }
    }
}
