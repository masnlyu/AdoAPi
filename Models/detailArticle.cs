using System.ComponentModel.DataAnnotations;

public class DetailArticle
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
    [Required(ErrorMessage = "Name是必填的")]
    [MaxLength(20, ErrorMessage = "Name不得高於20個字元")]
    public string Name { get; set; } = "";
    [Required(ErrorMessage = "CreateTime是必填的")]
    public DateTime CreateTime { get; set; }
}