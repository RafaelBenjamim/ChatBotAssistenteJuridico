using ChatBotAssistenteJuridico.Infrastructure.Data;
using ChatBotAssistenteJuridico.Infrastructure.Entitys;
using ChatBotAssistenteJuridico.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace ChatBotAssistenteJuridico.Infrastructure.Repository
{
    public class SendRequestRepository(RequestContext _context) : ISendRequestRepository
    {
        public async Task<ResponseEntity> GetResponseHash(string askHash)
        {
            return await _context.Responses.FirstOrDefaultAsync(x => x.PerguntaHash == askHash);
        }

        public async Task saveAsk(string ask, string question, string asnwer, int? categoryId)
        {
            var newEntry = new ResponseEntity
            {
                PerguntaHash = ask,
                Pergunta = question,
                Reposta = asnwer,
                DataCriacao = DateTime.Now,
                CategoryId = categoryId
            };

            await _context.Responses.AddAsync(newEntry);
            await _context.SaveChangesAsync();
        }
    }
}
