using System.ComponentModel.DataAnnotations;

public class ArticlePageFiltter
{
    [Required(ErrorMessage = "NowPage是必填的")]
    [Range(1, int.MaxValue, ErrorMessage = "請輸入大於或等於1的數字")]
    public int NowPage { get; set; }
    [Required(ErrorMessage = "PageSize是必填的")]
    [Range(1, int.MaxValue, ErrorMessage = "請輸入大於或等於1的數字")]
    public int PageSize { get; set; }
};