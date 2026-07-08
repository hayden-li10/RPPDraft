using RPP.Core.DTOs;
using System.Collections.Generic;

namespace RPP.Core.Interfaces
{
    public interface IEnrichmentService
    {
        List<ModulePayload> Enrich(List<ModulePayload> payloads);
    }
}
