namespace Proz_WebApi.Models.DesktopModels.DTO.DepartmentManager
{
    public class ReturnLoginHistory
    {
        public DateTime LoggedOn { get; set; }
        public string DeviceTokenHashed { get; set; }
        public string DeviceName { get; set; }
    }
}
