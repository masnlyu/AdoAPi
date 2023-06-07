using System.ComponentModel.DataAnnotations;
public class Article
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = "";
    [Required]
    public string Content { get; set; } = "";
    [Required]
    public string ImgBase64 { get; set; } = "";
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    public DateTime UpdateTime { get; set; }
    [Required]
    public int CreateAuthor_Id { get; set; }
    [Required]
    public int UpdateAuthor_id { get; set; }
    [Required]
    public Boolean Archive { get; set; }
}