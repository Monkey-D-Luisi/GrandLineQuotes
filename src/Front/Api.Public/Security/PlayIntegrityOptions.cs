namespace Api.Public.Security
{
    public class PlayIntegrityOptions
    {
        public string SessionSecret { get; set; } = "";
        public int SessionMinutes { get; set; } = 60;
        public string ServiceAccountJsonPath { get; set; } = "service-account.json";
    }
}
