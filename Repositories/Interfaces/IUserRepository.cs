using article.Models;

namespace article.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<int> RegisteredAsync(User user);

        Task<User?> GetByUsernameAsync(string username);


        Task<User?> GetByIdAsync(int id);

    }
}
