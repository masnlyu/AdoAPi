using System.ComponentModel.DataAnnotations;

public class NewsArticle
{
    [Required(ErrorMessage = "ID是必填的")]
    [Range(1, int.MaxValue, ErrorMessage = "請輸入大於或等於1的數字")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Title是必填的")]
    [MaxLength(50, ErrorMessage = "Title不得高於50個字元")]
    public string Title { get; set; } = "";
    [Required(ErrorMessage = "CreateTime是必填的")]
    public DateTime CreateTime { get; set; }
}