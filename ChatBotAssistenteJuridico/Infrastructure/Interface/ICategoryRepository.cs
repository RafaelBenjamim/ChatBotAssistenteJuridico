using ChatBotAssistenteJuridico.Infrastructure.Entitys;

namespace ChatBotAssistenteJuridico.Infrastructure.Interface
{
    public interface ICategoryRepository
    {
        Task<CategoryEntity> GetCategoryId(string category);
        Task<List<string>> GetAllCategoryNames();
    }
}
