using System.ComponentModel.DataAnnotations;

public class CreateArticle
{
    [Required(ErrorMessage = "Title是必填的")]
    [MaxLength(50, ErrorMessage = "Title不可高於50個字元")]
    public string Title { get; set; } = "";
    [Required(ErrorMessage = "Content是必填的")]
    public string Content { get; set; } = "";
    [Required(ErrorMessage = "ImgBase64是必填的")]
    public string ImgBase64 { get; set; } = "";
    [Required(ErrorMessage = "CreateTime是必填的")]
    public DateTime CreateTime { get; set; }
    [Required(ErrorMessage = "UpdateTime是必填的")]
    public DateTime UpdateTime { get; set; }
    [Required(ErrorMessage = "CreateAuthor_Id是必填的")]
    public int CreateAuthor_Id { get; set; }
    [Required(ErrorMessage = "UpdateAuthor_id是必填的")]
    public int UpdateAuthor_id { get; set; }
    [Required(ErrorMessage = "Archive是必填的")]
    public Boolean Archive { get; set; }
};