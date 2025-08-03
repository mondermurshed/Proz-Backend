namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class ReturnLoginHistoryForManagerRequest
    { 
        public Guid ID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool ReturnItAs { get; set; }
    }
}
