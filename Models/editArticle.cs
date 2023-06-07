using System.ComponentModel.DataAnnotations;

public class EditArticle
{
    [Required(ErrorMessage = "Id是必填的")]
    [Range(1, int.MaxValue, ErrorMessage = "請輸入大於或等於1的數字")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Title是必填的")]
    [MaxLength(50, ErrorMessage = "Title不得高於50個字元")]
    public string Title { get; set; } = "";
    [Required(ErrorMessage = "Content是必填的")]
    public string Content { get; set; } = "";
    [Required(ErrorMessage = "ImgBase64是必填的")]
    public string ImgBase64 { get; set; } = "";
    [Required(ErrorMessage = "UpdateTime是必填的")]
    public DateTime UpdateTime { get; set; }
    [Required(ErrorMessage = "ID是必填的")]
    public int UpdateAuthor_id { get; set; }
};