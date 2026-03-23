using ChattingApp.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace ChattingApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> GenerateTokenAsync(LoginDto loginDto);
    }
}
