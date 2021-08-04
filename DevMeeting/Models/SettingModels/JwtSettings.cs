namespace DevMeeting.Models.SettingModels
{
    public interface IJwtBearerTokenSettings
    {
        string Issuer { get; set; }
        string Type { get; set; }
        string Key { get; set; }
        string Audience { get; set; }
    }

    public class JwtBearerTokenSettings : IJwtBearerTokenSettings
    {
        public string Issuer { get; set; }
        
        public string Type { get; set; }
        public string Key { get; set; }
        
        public string Audience { get; set; }
    }
}