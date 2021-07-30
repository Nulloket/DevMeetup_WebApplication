namespace DevMeeting.Models.SettingModels
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string MeetupsCollection { get; set; }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string MeetupsCollection { get; set; }
    }
}