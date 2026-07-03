using RPP.Application.DTOs;
using RPP.Application.Interfaces;
using System.Collections.Generic;

namespace RPP.Application.BusinessEngine
{
    public class EnrichmentService : IEnrichmentService
    {
        public List<ModulePayload> Enrich(List<ModulePayload> payloads)
        {
            var list = payloads ?? new List<ModulePayload>();

            foreach (var item in list)
            {
                item.TestNumber += 4;
            }

            return list;
        }
    }
}
