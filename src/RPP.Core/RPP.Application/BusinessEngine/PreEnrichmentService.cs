using RPP.Application.DTOs;
using RPP.Application.Interfaces;
using System.Collections.Generic;

namespace RPP.Application.BusinessEngine
{
    public class PreEnrichmentService : IPreEnrichmentService
    {
        public List<ModulePayload> PreEnrich(List<ModulePayload> payloads)
        {
            var list = payloads ?? new List<ModulePayload>();

            foreach (var item in list)
            {
                item.TestNumber += 2;
            }

            return list;
        }
    }
}
