using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using TeknikServis.Entity.Enums;

namespace TeknikServis.BLL.Services.Senders
{
    public interface IMessageService
    {
        MessageStates MessageState { get; }

        Task SendAsync(IdentityMessage message, params string[] contacts);
        void Send(IdentityMessage message, params string[] contacts);
    }
}
