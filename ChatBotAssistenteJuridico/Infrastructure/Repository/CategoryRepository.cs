using ChatBotAssistenteJuridico.Infrastructure.Data;
using ChatBotAssistenteJuridico.Infrastructure.Entitys;
using ChatBotAssistenteJuridico.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace ChatBotAssistenteJuridico.Infrastructure.Repository
{
    public class CategoryRepository(RequestContext _context) : ICategoryRepository
    {
        public async Task<CategoryEntity> GetCategoryId(string category)
        {
            var categoryFind = category.Trim().ToLower();

            return await _context.Categories.FirstOrDefaultAsync(x => x.Name.ToLower().Contains(categoryFind) || categoryFind.Contains(x.Name.ToLower()));
        }

        public async Task<List<string>> GetAllCategoryNames()
        {
            return await _context.Categories.Select(c => c.Name).ToListAsync();
        }
    }
}
