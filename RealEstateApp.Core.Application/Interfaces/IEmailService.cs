using RealEstateApp.Core.Application.DTOs.Email;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailRequest request);
}


