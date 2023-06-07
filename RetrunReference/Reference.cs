namespace Reference
{
    public enum StateReference
    {
        Success = 0,
        LoginFail = -1,
        GetAllArticlesFail = -2,
        GetArticlesCountFail = -3,
        GetNewsArticlesFail = -4,
        GetUserAllArticlesFail = -5,
        GetArticleDetailFail = -7,
        PostArticleFail = -8,
        UpdateArticleFail = -9,
        DeleteArticleFail = -10,
        DataError = -11,
        UserNotFound = -12,
        AuthenticationFailed = -13,
        ServerError = -99
    }
    public class ResReference
    {
        /// <summary>取得狀態碼參照表對應中文
        /// </summary>
        /// <param name="stateEnum">狀態碼</param>
        /// <returns>參照表對應中文</returns>
        public string GetTextReference(StateReference stateEnum)
        {
            string textResult = "";
            switch (stateEnum)
            {
                default:
                    textResult = "無匹配的參照";
                    break;
                case StateReference.Success:
                    textResult = "成功";
                    break;
                case StateReference.LoginFail:
                    textResult = "登入失敗，帳號密碼錯誤";
                    break;
                case StateReference.GetAllArticlesFail:
                    textResult = "取得所有文章失敗";
                    break;
                case StateReference.GetNewsArticlesFail:
                    textResult = "取得最新文章列表失敗";
                    break;
                case StateReference.GetUserAllArticlesFail:
                    textResult = "取得使用者所屬文章失敗";
                    break;
                case StateReference.GetArticlesCountFail:
                    textResult = "取得所有文章筆數失敗";
                    break;
                case StateReference.GetArticleDetailFail:
                    textResult = "取得文章詳細內容失敗";
                    break;
                case StateReference.PostArticleFail:
                    textResult = "新增文章失敗";
                    break;
                case StateReference.UpdateArticleFail:
                    textResult = "修改文章失敗";
                    break;
                case StateReference.DeleteArticleFail:
                    textResult = "刪除文章失敗";
                    break;
                case StateReference.AuthenticationFailed:
                    textResult = "身分驗證失敗";
                    break;
                case StateReference.DataError:
                    textResult = "資料驗證錯誤";
                    break;
                case StateReference.UserNotFound:
                    textResult = "查無此使用者";
                    break;
                case StateReference.ServerError:
                    textResult = "伺服器執行錯誤";
                    break;
            }
            return textResult;
        }

    }

}