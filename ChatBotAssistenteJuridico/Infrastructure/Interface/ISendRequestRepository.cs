using ChatBotAssistenteJuridico.Infrastructure.Entitys;
using System.Runtime.CompilerServices;

namespace ChatBotAssistenteJuridico.Infrastructure.Interface
{
    public interface ISendRequestRepository
    {
       Task<ResponseEntity> GetResponseHash(string askHash);

       Task saveAsk(string ask, string question, string asnwer, int? categoryId);
    }
}
