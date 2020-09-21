using Onion.Application.DTOs.Email;
using System.Threading.Tasks;

namespace Onion.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}