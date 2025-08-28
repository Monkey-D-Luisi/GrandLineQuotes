namespace GrandLineQuotes.Client.Abstractions.RequestModels.Integrity
{
    public class PlayIntegrityRequestModel
    {


        public string PackageName { get; set; } = default!;

        public string Token { get; set; } = default!;
        public string Nonce { get; set; } = default!;
    }
}
