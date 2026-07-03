using RPP.Application.DTOs;
using System.Collections.Generic;

namespace RPP.Application.Interfaces
{
    public interface IEnrichmentService
    {
        List<ModulePayload> Enrich(List<ModulePayload> payloads);
    }
}
