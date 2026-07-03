namespace RPP.Application.DTOs
{
    public class ModulePayload
    {
        public int? TestNumber { get; set; }
        public ShiftDraft? Draft { get; set; }
        public ShiftPreEnriched? PreEnriched { get; set; }
        public ShiftAllocated? Allocated { get; set; }
        public ShiftEnriched? Enriched { get; set; }
        public ShiftOutputPayload? Output { get; set; }
    }
}
