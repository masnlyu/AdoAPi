using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Data;

public class JwtHelpers
{
    private IConfiguration configuration { get; }

    public JwtHelpers(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GenerateToken(DataTable result, int expireMinutes)
    {
        string token = "";
        try
        {
            if (result.Rows.Count > 0)
            {
                string issuer = configuration.GetValue<string>("JwtSettings:Issuer");
                string signKey = configuration.GetValue<string>("JwtSettings:SignKey");
                List<Claim> claims = new List<Claim>();
                Claim claimInfo;
                claimInfo = new Claim(JwtHeaderParameterNames.Kid, (result.Rows[0]["Id"].ToString()) ?? "");
                claims.Add(claimInfo);
                claimInfo = new Claim("Name", result.Rows[0]["Name"].ToString() ?? "");
                claims.Add(claimInfo);
                claimInfo = new Claim("roles", (result.Rows[0]["Role"].ToString()) ?? "");
                claims.Add(claimInfo);
                
                ClaimsIdentity userClaimsIdentity = new ClaimsIdentity(claims);
                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
                SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor();
                tokenDescriptor.Issuer = issuer;
                tokenDescriptor.Subject = userClaimsIdentity;
                tokenDescriptor.Expires = DateTime.Now.AddMinutes(expireMinutes);
                tokenDescriptor.SigningCredentials = signingCredentials;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
                token = tokenHandler.WriteToken(securityToken);
            }
        }
        catch (System.Exception)
        {
        }
        return token;
    }
}