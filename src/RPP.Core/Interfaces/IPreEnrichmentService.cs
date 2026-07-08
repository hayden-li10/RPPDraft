using RPP.Core.DTOs;
using System.Collections.Generic;

namespace RPP.Core.Interfaces
{
    public interface IPreEnrichmentService
    {
        List<ModulePayload> PreEnrich(List<ModulePayload> payloads);
    }
}
