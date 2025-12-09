using ChatBotAssistenteJuridico.Infrastructure.Entitys;
using Microsoft.EntityFrameworkCore;

namespace ChatBotAssistenteJuridico.Infrastructure.Data
{
    public class RequestContext : DbContext
    {

        public RequestContext( DbContextOptions<RequestContext> options) : base(options) 
        {

        }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ResponseEntity> Responses { get; set; }
        public DbSet<HistoryEntity> History { get; set; }
    }
}
