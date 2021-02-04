namespace CoreCommon.Data.Domain.Models
{
    public class AuthResponse
    {
        public string ErrorMessage { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
    }   
}
