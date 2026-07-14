using article.Models;

namespace article.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<int> CreateAsync(Article article);
        Task<IEnumerable<Article>> GetAllAsync();
        Task <Article ?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(Article article);

        Task<bool> DeleteAsync(int id, int authorId);
    }
}
