namespace DevMeeting.Models.SettingModels
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string MeetupsCollection { get; set; }
        string UsersCollection { get; set; }
        string RoleCollection { get; set; }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string MeetupsCollection { get; set; }
        public string UsersCollection { get; set; }
        
        public string RoleCollection { get; set; }
    }
}