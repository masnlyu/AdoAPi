using System.ComponentModel.DataAnnotations;

public partial class LoginInfo
{
    [Required(ErrorMessage = "帳號是必填的")]
    [MinLength(4, ErrorMessage = "帳號不得低於4個字元")]
    [MaxLength(50, ErrorMessage = "帳號不得高於50個字元")]
    public string Account { get; set; } = "";
    [Required(ErrorMessage = "帳號是必填的")]
    [MinLength(4, ErrorMessage = "密碼不得低於4個字元")]
    [MaxLength(50, ErrorMessage = "密碼不得高於50個字元")]
    public string Password { get; set; } = "";
}
