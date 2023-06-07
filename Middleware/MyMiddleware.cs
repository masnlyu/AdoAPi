using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using MySqlConnector;
using Reference;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class AuthorizationMiddlewareResultHandler : ControllerBase, IAuthorizationMiddlewareResultHandler
{
    private IConfiguration configuration { get; }
    private ResReference resReference { get; }
    private SqlFunction sqlFunction { get; }
    private JwtSecurityTokenHandler tokenHandler { get; }
    public AuthorizationMiddlewareResultHandler(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.sqlFunction = new SqlFunction(configuration);
        this.resReference = new ResReference();
        this.tokenHandler = new JwtSecurityTokenHandler();
    }
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        ResponseBox<string> res = new ResponseBox<string>();
        try
        {

            if (context.Request.Cookies.TryGetValue("token", out string? TokenValue))
            {
                if (TokenValue == null)
                {
                    res.Status = StateReference.AuthenticationFailed;
                }   
                else
                {
                    var myIssuer = configuration.GetValue<string>(("JwtSettings:Issuer"));
                    var mySignKey = configuration.GetValue<string>(("JwtSettings:SignKey"));
                    TokenValidationParameters validationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetValue<string>(("JwtSettings:Issuer")),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySignKey))
                    };
                    tokenHandler.ValidateToken(TokenValue, validationParameters, out SecurityToken validatedToken);
                    string query = @"
                            SELECT `Author`.`Id`
                                ,`Author`.`Role`
                            FROM `ArticleWeb`.`LoginInfo` 
                                JOIN `ArticleWeb`.`Author` 
                                    ON `LoginInfo`.`Author_Id` = `Author`.`Id`
                            WHERE (`LoginInfo`.`Token` = @Token);";
                    List<MySqlParameter> parameters = new List<MySqlParameter>();
                    MySqlParameter parameter = new MySqlParameter("@Token", MySqlDbType.Text);
                    parameter.Value = TokenValue;
                    parameters.Add(parameter);
                    DataTable result = sqlFunction.StartQuery(query, parameters.ToArray());
                    if (result.Rows.Count > 0)
                    {
                        context.Items.Add("AuthorId", result.Rows[0]["Id"]);
                        context.Items.Add("AuthorRole", result.Rows[0]["Role"]);
                        await next(context);
                    }
                    else
                    {
                        // context.Response.Cookies.Delete("token");
                        res.Status = StateReference.AuthenticationFailed;
                    }
                }
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
            context.Response.Cookies.Delete("token");
            res.Text = resReference.GetTextReference(res.Status);
        }
        await context.Response.WriteAsJsonAsync(res);
    }
}