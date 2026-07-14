using System.ComponentModel.DataAnnotations;

namespace article.Dtos
{
    public class ArticleCreateDto
    {
        [Required,MaxLength(200)]
        public string Title { get; set; }
        [Required]

        public string Content { get; set; } = string.Empty;

    }

    public class ArticleUpdateDto {
        [Required,MaxLength (200)]
        public string Title { get; set; }=string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;

    
    }
}
