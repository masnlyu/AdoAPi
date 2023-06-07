using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;
using Reference;
using System.Text;
namespace articleAdoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private IConfiguration configuration { get; }
        private SqlFunction sqlFunction { get; }
        private ResReference resReference { get; }
        private IHttpContextAccessor contextAccessor { get; }
        private int authorId { get; }
        private bool authorRole { get; }
        public ArticlesController(IConfiguration configuration, IHttpContextAccessor iHttpContextAccessor)
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
            this.sqlFunction = new SqlFunction(configuration);
            this.resReference = new ResReference();
            this.configuration = configuration;
        }
        /// <summary>取得分頁的文章
        /// </summary>
        /// <param name="filtter">目前頁面以及顯示的篇數</param>
        /// <returns>文章列表</returns>
        [HttpGet]
        public ResponseBox<List<DetailArticle>> GetArticles([FromQuery] ArticlePageFiltter filtter)
        {
            ResponseBox<List<DetailArticle>> res = new ResponseBox<List<DetailArticle>>();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = StateReference.DataError;
                }
                else
                {

                    string query = @"
                        SELECT `Article`.`Id`
                            , `Article`.`Title`
                            , `Article`.`Content`
                            , `Article`.`CreateTime`
                            , `Article`.`ImgBase64`
                            , `Author`.`Name`
                        FROM `ArticleWeb`.`Article`
                            JOIN `ArticleWeb`.`Author` 
                                ON `Article`.`CreateAuthor_Id` = `Author`.`Id`
                        WHERE (`Article`.`Archive` = @Archive)
                        LIMIT @NowPage, @PageSize;";
                    int fromPage = (filtter.NowPage - 1) * filtter.PageSize;
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter parameter;
                    parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                    parameter.Value = false;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@NowPage", MySqlDbType.Int32);
                    parameter.Value = fromPage;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@PageSize", MySqlDbType.Int32);
                    parameter.Value = filtter.PageSize;
                    parameters.Add(parameter);
                    DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                    if (result.Rows.Count > 0)
                    {
                        List<DetailArticle> articles = new List<DetailArticle>();
                        foreach (DataRow row in result.Rows)
                        {
                            DetailArticle detailArticle = new DetailArticle();
                            detailArticle.Id = Convert.ToInt32(row["Id"]);
                            detailArticle.Title = row["Title"].ToString() ?? "";
                            detailArticle.Content = row["Content"].ToString() ?? "";
                            detailArticle.ImgBase64 = row["ImgBase64"].ToString() ?? "";
                            detailArticle.CreateTime = Convert.ToDateTime(row["CreateTime"]);
                            detailArticle.Name = row["Name"].ToString() ?? "";
                            articles.Add(detailArticle);
                        }
                        res.Status = StateReference.Success;
                        res.Result = articles;
                    }
                    else
                    {
                        res.Status = StateReference.GetAllArticlesFail;
                    }
                }
            }
            catch (System.Exception)
            {
                res.Status = StateReference.GetAllArticlesFail;
            }
            finally
            {
                res.Text = resReference.GetTextReference(res.Status);
            }
            return res;
        }
        /// <summary>取得所有文章的筆數
        /// </summary>
        /// <returns>文章筆數</returns>
        [HttpGet("count")]
        public ResponseBox<int> GetAllArticleCount()
        {
            ResponseBox<int> res = new ResponseBox<int>();
            try
            {
                string query = @"
                    SELECT COUNT(`Article`.`Id`) AS count
                    FROM  `ArticleWeb`.`Article`
                    WHERE (`Article`.`Archive` = @Archive);";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                parameter.Value = false;
                parameters.Add(parameter);
                DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                if (result.Rows.Count > 0)
                {
                    res.Status = StateReference.Success;
                    res.Result = Convert.ToInt32(result.Rows[0]["count"]);
                }
                else
                {
                    res.Status = StateReference.GetArticlesCountFail;
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
        /// <summary>取得某一篇文章
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns>文章內容</returns>
        [HttpGet("{id}")]
        public ResponseBox<DetailArticle> GetArticle([FromRoute] int id)
        {
            ResponseBox<DetailArticle> res = new ResponseBox<DetailArticle>();
            try
            {
                string query = @"
                    SELECT `Article`.`Id`
                        , `Article`.`Title`
                        , `Article`.`Content`
                        , `Article`.`ImgBase64`
                        , `Article`.`CreateTime`
                        , `Author`.`Name`
                    FROM  `ArticleWeb`.`Article`
                        JOIN  `ArticleWeb`.`Author` 
                            ON (`Article`.`CreateAuthor_Id` = `Author`.`Id`)
                    WHERE (`Article`.`Archive` = @Archive
                        AND `Article`.`Id` = @Id);";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter;
                parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                parameter.Value = false;
                parameters.Add(parameter);
                parameter = new MySqlParameter("@id", MySqlDbType.Int32);
                parameter.Value = id;
                parameters.Add(parameter);
                DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                if (result.Rows.Count > 0)
                {
                    DetailArticle detailArticle = new DetailArticle();
                    foreach (DataRow row in result.Rows)
                    {
                        detailArticle.Id = Convert.ToInt32(row["Id"]);
                        detailArticle.Title = row["Title"].ToString() ?? "";
                        detailArticle.Content = row["Content"].ToString() ?? "";
                        detailArticle.ImgBase64 = row["ImgBase64"].ToString() ?? "";
                        detailArticle.CreateTime = Convert.ToDateTime(row["CreateTime"]);
                        detailArticle.Name = row["Name"].ToString() ?? "";
                    }
                    res.Status = StateReference.Success;
                    res.Result = detailArticle;
                }
                else
                {
                    res.Status = StateReference.GetArticleDetailFail;
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
        /// <summary>取得最新的文章
        /// </summary>
        /// <returns>最新文章列表</returns>
        [HttpGet("news")]
        public ResponseBox<List<NewsArticle>> GetNewArticles()
        {
            ResponseBox<List<NewsArticle>> res = new ResponseBox<List<NewsArticle>>();
            try
            {
                string query = @"
                    SELECT `Id`
                        , `Title`
                        , `CreateTime`
                    FROM `ArticleWeb`.`Article`
                    Where `Archive` = @Archive
                    ORDER BY `CreateTime` DESC
                    LIMIT @newsArticleCount;";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter;
                parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                parameter.Value = false;
                parameters.Add(parameter);
                parameter = new MySqlParameter("@newsArticleCount", MySqlDbType.Int32);
                parameter.Value = configuration.GetValue<int>("GetNewsArticles:Count", 5);
                parameters.Add(parameter);
                DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                if (result.Rows.Count > 0)
                {
                    List<NewsArticle> newArticles = new List<NewsArticle>();
                    foreach (DataRow row in result.Rows)
                    {
                        NewsArticle newArticle = new NewsArticle();
                        newArticle.Id = Convert.ToInt32(row["Id"]);
                        newArticle.Title = row["Title"].ToString() ?? "";
                        newArticle.CreateTime = Convert.ToDateTime(row["CreateTime"]);
                        newArticles.Add(newArticle);
                    }
                    res.Status = StateReference.Success;
                    res.Result = newArticles;
                }
                else
                {
                    res.Status = StateReference.GetNewsArticlesFail;
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
        /// <summary>新增文章
        /// </summary>
        /// <param name="article">要新增的文章內容</param>
        /// <returns>回應訊息</returns>
        [HttpPost]
        [Authorize]
        public ResponseBox<CreateArticle> PostArticle([FromBody] CreateArticle article)
        {
            ResponseBox<CreateArticle> res = new ResponseBox<CreateArticle>();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = StateReference.DataError;
                }
                else
                {
                    string userQuery = @"
                        INSERT INTO `ArticleWeb`.`Article`
                            (`Title`
                            , `Content`
                            , `ImgBase64`
                            , `CreateTime`
                            , `UpdateTime`
                            , `CreateAuthor_Id`
                            , `UpdateAuthor_Id`
                            , `Archive`)
                        SELECT @Title
                            , @Content
                            , @ImgBase64
                            , @CreateTime
                            , @UpdateTime
                            , `Author`.`Id`
                            , `Author`.`Id`
                            , @Archive
                        FROM `ArticleWeb`.`Author` 
                        WHERE (`Author`.`Id` = @AuthorId);";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter parameter;
                    parameter = new MySqlParameter("@Title", MySqlDbType.VarChar, 50);
                    parameter.Value = article.Title;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@Content", MySqlDbType.VarChar, 1000);
                    parameter.Value = article.Content;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@ImgBase64", MySqlDbType.LongText);
                    parameter.Value = article.ImgBase64;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@CreateTime", MySqlDbType.DateTime);
                    parameter.Value = DateTime.UtcNow;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@UpdateTime", MySqlDbType.DateTime);
                    parameter.Value = DateTime.UtcNow;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                    parameter.Value = article.Archive;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@AuthorId", MySqlDbType.Int32);
                    parameter.Value = authorId;
                    parameters.Add(parameter);
                    int result = sqlFunction.StartNonQuery(userQuery, parameters.ToArray());
                    if (result > 0)
                    {
                        res.Status = StateReference.Success;
                    }
                    else
                    {
                        res.Status = StateReference.PostArticleFail;
                    }
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
        /// <summary>取得要修改的文章內容
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns>文章內容</returns>
        [HttpGet("edit/{id}")]
        [Authorize]
        public ResponseBox<EditArticle> GetEditArticle([FromRoute] int id)
        {
            ResponseBox<EditArticle> res = new ResponseBox<EditArticle>();
            try
            {
                DataTable result = new DataTable();
                StringBuilder query = new StringBuilder(@"
                    SELECT `Article`.`Id`
                        , `Article`.`Title`
                        , `Article`.`Content`
                        , `Article`.`ImgBase64`
                        , `Article`.`UpdateTime`
                        , `Article`.`UpdateAuthor_id`
                    FROM  `ArticleWeb`.`Article`
                        WHERE `Article`.`Archive` = 0 
                            AND `Article`.`Id` = @Id");
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter;
                parameter = new MySqlParameter("@Id", MySqlDbType.Int32);
                parameter.Value = id;
                parameters.Add(parameter);
                if (!authorRole)
                {
                    query.Append(" AND `Article`.`CreateAuthor_Id` = @CreateAuthor_Id;");
                    parameter = new MySqlParameter("@CreateAuthor_Id", MySqlDbType.Int32);
                    parameter.Value = authorId;
                    parameters.Add(parameter);
                }
                result = sqlFunction.StartQuery(query.ToString(), parameters.ToArray());
                if (result.Rows.Count > 0)
                {
                    EditArticle editArticle = new EditArticle();
                    foreach (DataRow row in result.Rows)
                    {
                        editArticle.Id = Convert.ToInt32(row["Id"]);
                        editArticle.Title = row["Title"].ToString() ?? "";
                        editArticle.Content = row["Content"].ToString() ?? "";
                        editArticle.ImgBase64 = row["ImgBase64"].ToString() ?? "";
                        editArticle.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                        editArticle.UpdateAuthor_id = Convert.ToInt32(row["UpdateAuthor_id"]);
                    }
                    res.Status = StateReference.Success;
                    res.Result = editArticle;
                }
                else
                {
                    res.Status = StateReference.GetArticleDetailFail;
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
        /// <summary>修改文章內容
        /// </summary>
        /// <param name="id">文章便號</param>
        /// <param name="article">文章內容</param>
        /// <returns>回應訊息</returns>
        [HttpPut("{id}")]
        [Authorize]
        public ResponseBox<string> PutArticle([FromRoute] int id, [FromBody] EditArticle editArticle)
        {
            ResponseBox<string> res = new ResponseBox<string>();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = StateReference.DataError;
                }
                else
                {
                    int result = 0;
                    StringBuilder query = new StringBuilder(@"
                        UPDATE `ArticleWeb`.`Article`
                        SET `Title` = @Title
                            , `Content` = @Content
                            , `ImgBase64` = @ImgBase64
                            , `UpdateTime` = @UpdateTime
                            , `UpdateAuthor_Id` = @AuthorId
                        WHERE `Id` = @Id");
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter parameter;
                    parameter = new MySqlParameter("@Id", MySqlDbType.Int32);
                    parameter.Value = id;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@Title", MySqlDbType.VarChar, 50);
                    parameter.Value = editArticle.Title;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@Content", MySqlDbType.VarChar, 1000);
                    parameter.Value = editArticle.Content;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@ImgBase64", MySqlDbType.LongText);
                    parameter.Value = editArticle.ImgBase64;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@UpdateTime", MySqlDbType.DateTime);
                    parameter.Value = DateTime.UtcNow;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@AuthorId", MySqlDbType.Int32);
                    parameter.Value = authorId;
                    parameters.Add(parameter);
                    if (!authorRole)
                    {
                        query.Append(" AND `CreateAuthor_Id` = @AuthorId");
                    }
                    result = sqlFunction.StartNonQuery(query.ToString(), parameters.ToArray());
                    if (result > 0)
                    {
                        res.Status = StateReference.Success;
                    }
                    else
                    {
                        res.Status = StateReference.UpdateArticleFail;
                    }
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
        /// <summary>刪除文章
        /// </summary>
        /// <param name="id">文章編號</param>
        /// <returns>回應訊息</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public ResponseBox<string> DeleteArticle([FromRoute] int id)
        {
            ResponseBox<string> res = new ResponseBox<string>();
            try
            {
                int result = 0;
                StringBuilder query = new StringBuilder(@"
                    UPDATE `ArticleWeb`.`Article` 
                    SET `Archive` = @Archive
                        , `UpdateTime` = @UpdateTime
                        , `UpdateAuthor_id` = @AuthorId
                    WHERE `Id` = @Id");
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter;
                parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                parameter.Value = true;
                parameters.Add(parameter);
                parameter = new MySqlParameter("@UpdateTime", MySqlDbType.DateTime);
                parameter.Value = DateTime.UtcNow;
                parameters.Add(parameter);
                parameter = new MySqlParameter("@AuthorId", MySqlDbType.Int32);
                parameter.Value = authorId;
                parameters.Add(parameter);
                parameter = new MySqlParameter("@Id", MySqlDbType.Int32);
                parameter.Value = id;
                parameters.Add(parameter);
                if (!authorRole)
                {
                    query.Append(" AND  `Article`.`CreateAuthor_Id` = @AuthorId");
                }
                result = sqlFunction.StartNonQuery(query.ToString(), parameters.ToArray());
                if (result > 0)
                {
                    res.Status = StateReference.Success;
                }
                else
                {
                    res.Status = StateReference.DeleteArticleFail;
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
        /// <summary>取得管理個人所有文章
        /// </summary>
        /// <param name="filtter">頁碼</param>
        /// <returns>文章列表</returns>
        [HttpGet("management")]
        [Authorize]
        public ResponseBox<List<DetailArticle>> GetManagementArticles([FromQuery] ArticlePageFiltter filtter)
        {
            ResponseBox<List<DetailArticle>> res = new ResponseBox<List<DetailArticle>>();
            try
            {
                if (!ModelState.IsValid)
                {
                    res.Status = StateReference.DataError;
                }
                else
                {
                    DataTable result = new DataTable();
                    StringBuilder query = new StringBuilder(@"
                        SELECT `Article`.`Id`
                            , `Article`.`Title`
                            , `Article`.`Content`
                            , `Article`.`ImgBase64`
                            , `Article`.`CreateTime`
                        FROM `ArticleWeb`.`Article`
                            JOIN `ArticleWeb`.`Author`
                                ON `Article`.`CreateAuthor_Id` = `Author`.`Id`
                        WHERE `Article`.`Archive` = @Archive   
                        LIMIT  @NowPage, @PageSize;");
                    int fromPage = (filtter.NowPage - 1) * filtter.PageSize;
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter parameter;
                    parameter = new MySqlParameter("@Archive", MySqlDbType.Bit);
                    parameter.Value = false;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@NowPage", MySqlDbType.Int32);
                    parameter.Value = fromPage;
                    parameters.Add(parameter);
                    parameter = new MySqlParameter("@PageSize", MySqlDbType.Int32);
                    parameter.Value = filtter.PageSize;
                    parameters.Add(parameter);
                    if (!authorRole)
                    {
                        query.Insert(query.ToString().IndexOf("LIMIT"), "AND `Author`.`Id` = @AuthorId ");
                        parameter = new MySqlParameter("@AuthorId", MySqlDbType.Int32);
                        parameter.Value = authorId;
                        parameters.Add(parameter);
                    }
                    result = sqlFunction.StartQuery(query.ToString(), parameters.ToArray());
                    if (result.Rows.Count > 0)
                    {
                        List<DetailArticle> articles = new List<DetailArticle>();
                        foreach (DataRow row in result.Rows)
                        {
                            DetailArticle detailArticle = new DetailArticle();
                            detailArticle.Id = Convert.ToInt32(row["Id"]);
                            detailArticle.Title = row["Title"].ToString() ?? "";
                            detailArticle.Content = row["Content"].ToString() ?? "";
                            detailArticle.ImgBase64 = row["ImgBase64"].ToString() ?? "";
                            detailArticle.CreateTime = Convert.ToDateTime(row["CreateTime"]);
                            articles.Add(detailArticle);
                        }
                        res.Status = StateReference.Success;
                        res.Result = articles;
                    }
                    else
                    {
                        res.Status = StateReference.GetUserAllArticlesFail;
                    }
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
        /// <summary>取得管理個人所屬文章筆數
        /// </summary>
        /// <returns>文章筆數</returns>
        [HttpGet("management/count")]
        [Authorize]
        public ResponseBox<int> GetManagementArticlesCount()
        {
            ResponseBox<int> res = new ResponseBox<int>();
            try
            {
                DataTable result = new DataTable();
                StringBuilder query = new StringBuilder(@"
                    SELECT COUNT(`Article`.`id`) AS count
                    FROM `ArticleWeb`.`Article` 
                        JOIN `ArticleWeb`.`Author` 
                            ON  `Article`.`CreateAuthor_Id` = `Author`.`Id`
                    WHERE `Article`.`Archive` = 0");
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                MySqlParameter parameter;
                if (!authorRole)
                {
                    query.Append(" AND `Author`.`Id` = @AuthorId");
                    parameter = new MySqlParameter("@AuthorId", MySqlDbType.Int32);
                    parameter.Value = authorId;
                    parameters.Add(parameter);
                }
                result = sqlFunction.StartQuery(query.ToString(), parameters.ToArray());
                if (result.Rows.Count > 0)
                {
                    res.Status = StateReference.Success;
                    res.Result = Convert.ToInt32(result.Rows[0]["count"]);
                }
                else
                {
                    res.Status = StateReference.GetArticlesCountFail;
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