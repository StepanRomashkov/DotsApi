namespace DotsApi.Models
{
    public class DotsDatabaseSettings: IDotsDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }

    public interface IDotsDatabaseSettings
    {
        string ConnectionString { get; set; }
        string Database { get; set; }
    }
}
