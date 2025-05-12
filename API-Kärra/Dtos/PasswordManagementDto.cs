namespace APIKarra.Dtos;

public class ForgotPasswordDto
{
    public string Email { get; set; }
}

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}

public class PasswordChangeDto
{
    public string UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
