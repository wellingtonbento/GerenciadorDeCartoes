namespace CardManager.Communication.Responses
{
    public class ResponseErrorJson
    {
        public IList<string> Errors { get; set; }
        public bool AccessTokenExpired { get; set; }

        public ResponseErrorJson(IList<string> errors) => Errors = errors;

        public ResponseErrorJson(string error) => Errors = [error];

        public ResponseErrorJson(string error, bool accessTokenExpired)
        {
            Errors = [error];
            AccessTokenExpired = accessTokenExpired;
        }
    }
}
