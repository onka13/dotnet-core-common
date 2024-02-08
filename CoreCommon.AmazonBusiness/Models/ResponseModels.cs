namespace CoreCommon.AmazonBusiness.Models;

public class SignInResponse
{
    public string ErrorMessage { get; set; }
    public string UserId { get; set; }
}

public class PasswordResponse
{
    public string ErrorMessage { get; set; }
    public bool IsSuccess { get; set; }
}
