using Onion.Application.DTOs;
using Onion.Application.DTOs.Account;
using System.Threading.Tasks;

namespace Onion.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);

        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);

        Task<Response<string>> ConfirmEmailAsync(string userId, string code);
    }
}