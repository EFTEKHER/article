using article.Models;
using article.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace article.Repositories
{
    public class ArticleRepository:IArticleRepository
    {

        private readonly string _cs;

        public ArticleRepository(IConfiguration configuration)
        {
            _cs = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
        }
        public async Task <int> CreateAsync(Article article)
        {
           using var con= new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_CreateArticle", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Title", article.Title);
            cmd.Parameters.AddWithValue("@Content", article.Content);
            cmd.Parameters.AddWithValue("@AuthorId", article.AuthorId);
            await con.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());


        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            var list = new List<Article>();
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_GetAllArticles", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) { list.Add(Map(reader)); }

            return list;
        }

        public async Task<Article?> GetByIdAsync(int id)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_GetArticleById", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }
        public async Task<bool> UpdateAsync(Article article)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_UpdateArticle", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", article.Id);
            cmd.Parameters.AddWithValue("@Title", article.Title);
            cmd.Parameters.AddWithValue("@Content", article.Content);
            cmd.Parameters.AddWithValue("@AuthorId", article.AuthorId);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int id, int authorId)
        {
            using var con = new SqlConnection(_cs);
            using var cmd = new SqlCommand("Sp_DeleteArticle", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@AuthorId", authorId);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        private static Article Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            Title = r.GetString(r.GetOrdinal("Title")),
            Content = r.GetString(r.GetOrdinal("Content")),
            AuthorId = r.GetInt32(r.GetOrdinal("AuthorId")),
            AuthorName = r.GetString(r.GetOrdinal("AuthorName")),
            CreatedAt = r.GetDateTime(r.GetOrdinal("CreatedAt")),
            UpdatedAt = r.IsDBNull(r.GetOrdinal("UpdatedAt")) ? null : r.GetDateTime(r.GetOrdinal("UpdatedAt"))

        };
    }
}
