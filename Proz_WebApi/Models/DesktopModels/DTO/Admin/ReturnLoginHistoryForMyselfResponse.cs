namespace Proz_WebApi.Models.DesktopModels.DTO.Admin
{
    public class ReturnLoginHistoryForMyselfResponse
    {
    public DateTime LoggedOn { get; set; }
    public string DeviceTokenHashed { get; set; }
    public string DeviceName { get; set; }
    }
}
