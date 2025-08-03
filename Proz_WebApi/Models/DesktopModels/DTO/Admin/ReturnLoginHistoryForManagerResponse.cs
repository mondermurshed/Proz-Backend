namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class ReturnLoginHistoryForManagerResponse
    {

        public DateTime LoggedOn { get; set; }
        public string DeviceTokenHashed { get; set; }
        public string DeviceName { get; set; }
    }
}
