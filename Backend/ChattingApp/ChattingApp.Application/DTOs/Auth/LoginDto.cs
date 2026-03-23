namespace ChattingApp.Application.DTOs.Auth
{
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Included for REST structural standards, though not strictly checked for dummy auth
    }
}
