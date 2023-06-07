using System.ComponentModel.DataAnnotations;

public class AuthorInfo
{
    [Required(ErrorMessage = "Id是必填的")]
    [Range(1, int.MaxValue, ErrorMessage = "請輸入大於或等於1的數字")]
    public int Id { get; set; }
    [Required]
    [MaxLength(20, ErrorMessage = "名稱不得高於20個字元")]
    public string Name { get; set; } = "";

}