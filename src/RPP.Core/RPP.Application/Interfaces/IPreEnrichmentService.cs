using RPP.Application.DTOs;
using System.Collections.Generic;

namespace RPP.Application.Interfaces
{
    public interface IPreEnrichmentService
    {
        List<ModulePayload> PreEnrich(List<ModulePayload> payloads);
    }
}
