using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;
using Reference;

namespace articleAdoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration { get; }
        private JwtHelpers jwt { get; }
        private ResReference resReference { get; }
        private SqlFunction sqlFunction { get; }
        private IHttpContextAccessor contextAccessor { get; }
        private int authorId { get; }
        private bool authorRole { get; }
        public AuthController(IConfiguration configuration, JwtHelpers jwt, IHttpContextAccessor iHttpContextAccessor)
        {
            this.contextAccessor = iHttpContextAccessor;
            if (this.contextAccessor.HttpContext != null)
            {
                if (this.contextAccessor.HttpContext.Items["AuthorId"] != null)
                {
                    this.authorId = Convert.ToInt32(contextAccessor.HttpContext.Items["AuthorId"]);
                }
                if (this.contextAccessor.HttpContext.Items["AuthorRole"] != null)
                {
                    this.authorRole = Convert.ToBoolean(contextAccessor.HttpContext.Items["AuthorRole"]);
                }
            }
            this.configuration = configuration;
            this.jwt = jwt;
            this.sqlFunction = new SqlFunction(configuration);
            this.resReference = new ResReference();
        }
        /// <summary>登入
        /// </summary>
        /// <param name="loginInfo">帳號密碼</param>
        /// <returns>使用者資訊</returns>
        [HttpPost("login")]
        public ResponseBox<AuthorInfo> Login([FromBody] LoginInfo loginInfo)
        {
            ResponseBox<AuthorInfo> res;
            try
            {
                res = new ResponseBox<AuthorInfo>();
                if (!ModelState.IsValid)
                {
                    res.Status = StateReference.DataError;
                }
                else
                {
                    string query = @"
                    SELECT `Author`.`Id`
                        , `Author`.`Name`
                        , `Author`.`Role`
                    FROM `ArticleWeb`.`Author` 
                    WHERE (`Account` = @Account 
                        AND `Password` = @Password);";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter parameter;
                    parameter = new MySqlParameter("@Account", MySqlDbType.String);
                    parameter.Value = loginInfo.Account;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@Password", MySqlDbType.String);
                    parameter.Value = loginInfo.Password;
                    parameters.Add(parameter);
                    DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                    if (result.Rows.Count > 0)
                    {
                        AuthorInfo authorInfo = new AuthorInfo();
                        foreach (DataRow row in result.Rows)
                        {
                            authorInfo.Id = Convert.ToInt32(row["Id"]);
                            authorInfo.Name = row["Name"].ToString() ?? "";
                        }
                        string token = jwt.GenerateToken(result, configuration.GetValue<int>("JwtSettings:ExpMinutes", 30));
                        query = @"
                        UPDATE `ArticleWeb`.`LoginInfo` 
                        SET `Token` = @Token
                            , `UpdateTime` = @UpdateTime
                        WHERE (`Author_Id` = @Author_Id);";
                        List<MySqlParameter> loginParameters = new List<MySqlParameter>();
                        parameter = new MySqlParameter("@Token", MySqlDbType.String);
                        parameter.Value = token;
                        loginParameters.Add(parameter);
                        parameter = new MySqlParameter("@Author_Id", MySqlDbType.Int32);
                        parameter.Value = result.Rows[0]["Id"];
                        loginParameters.Add(parameter);
                        parameter = new MySqlParameter("@UpdateTime", MySqlDbType.DateTime);
                        parameter.Value = DateTime.UtcNow;
                        loginParameters.Add(parameter);
                        int updateResult = sqlFunction.StartNonQuery(query, loginParameters.ToArray());
                        if (updateResult > 0)
                        {
                            CookieOptions cookieOptions = new CookieOptions();
                            cookieOptions.Expires = DateTime.Now.AddHours(configuration.GetValue<int>("Expiration:CookieEXp", 1));
                            cookieOptions.HttpOnly = true;
                            Response.Cookies.Append("token", token, cookieOptions);
                            res.Status = StateReference.Success;
                            res.Result = authorInfo;
                        }
                        else
                        {
                            res.Status = StateReference.LoginFail;
                        }
                    }
                    else
                    {
                        res.Status = StateReference.LoginFail;
                    }
                }
            }
            catch (System.Exception)
            {
                res = new ResponseBox<AuthorInfo>();
                res.Status = StateReference.ServerError;
            }
            finally
            {
                res = new ResponseBox<AuthorInfo>();
                res.Text = resReference.GetTextReference(res.Status);
            }
            return res;
        }
        /// <summary>登出
        /// </summary>
        /// <returns>回應訊息</returns>
        [HttpGet("logout")]
        [Authorize]
        public ResponseBox<string> Logout()
        {
            ResponseBox<string> res = new ResponseBox<string>();
            try
            {
                string query = @"
                    UPDATE `ArticleWeb`.`LoginInfo` 
                    SET `Token` = @Token
                        , `UpdateTime` = @UpdateTime
                    WHERE (`Author_Id` = @Author_Id);";
                List<MySqlParameter> logoutParameter = new List<MySqlParameter>();
                MySqlParameter parameter;
                parameter = new MySqlParameter("@Token", MySqlDbType.Text);
                parameter.Value = string.Empty;
                logoutParameter.Add(parameter);
                parameter = new MySqlParameter("@Author_Id", MySqlDbType.Int32);
                parameter.Value = authorId;
                logoutParameter.Add(parameter);
                parameter = new MySqlParameter("@UpdateTime", MySqlDbType.DateTime);
                parameter.Value = DateTime.UtcNow;
                logoutParameter.Add(parameter);
                int result = sqlFunction.StartNonQuery(query, logoutParameter.ToArray());
                if (result > 0)
                {
                    Response.Cookies.Delete("token");
                    res.Status = StateReference.Success;
                }
                else
                {
                    res.Status = StateReference.AuthenticationFailed;
                }
            }
            catch (System.Exception)
            {
                res.Status = StateReference.ServerError;
            }
            finally
            {
                res.Text = resReference.GetTextReference(res.Status);
            }
            return res;
        }
        public ResponseBox<AuthorInfo> getAuthorInfo()
        {
            ResponseBox<AuthorInfo> res = new ResponseBox<AuthorInfo>();
            try
            {
                string query = @"
                    SELECT `Author`.`Id`
                        ,`Author`.`Name`
                    FROM `ArticleWeb`.`Author`
                        JOIN `ArticleWeb`.`LoginInfo`
                            ON `Author`.`Id` = `LoginInfo`.`Author_Id`
                    WHERE (`LoginInfo`.`Token` = @Token);";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter;
                parameter = new MySqlParameter("@Token", MySqlDbType.Text);
                parameter.Value = Request.Cookies["token"];
                DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                if (result.Rows.Count > 0)
                {
                    AuthorInfo authorInfo = new AuthorInfo();
                    foreach (DataRow row in result.Rows)
                    {
                        authorInfo.Id = Convert.ToInt32(row["Id"]);
                        authorInfo.Name = row["Content"].ToString() ?? "";
                    }
                    res.Status = StateReference.Success;
                    res.Result = authorInfo;
                }
                else
                {
                    res.Status = StateReference.UserNotFound;
                }
            }
            catch (System.Exception)
            {
                res.Status = StateReference.ServerError;
            }
            finally
            {
                res.Text = resReference.GetTextReference(res.Status);
            }
            return res;
        }

    }
}