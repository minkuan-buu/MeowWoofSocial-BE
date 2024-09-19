using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using RNGCryptoServiceProvider = System.Security.Cryptography.RNGCryptoServiceProvider;

namespace MeowWoofSocial.Business.ApplicationMiddleware;

public class Authentication
{
    private static string Key = "TestingIssuerSigningKeyPTEducationMS@123";
    private static string Issuser = "TestingJWTIssuerSigningPTEducationMS@123";

    public Authentication()
    {
    }

    static string GenerateSalt()
    {
        int SaltLength = 16;

        byte[] Salt = new byte[SaltLength];

        using (var Rng = new RNGCryptoServiceProvider())
        {
            Rng.GetBytes(Salt);
        }

        return BitConverter.ToString(Salt).Replace("-", "");
    }

    public static CreateHashPasswordModel CreateHashPassword(string Password)
    {
        string SaltString = GenerateSalt();
        byte[] Salt = Encoding.UTF8.GetBytes(SaltString);
        byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
        byte[] CombinedBytes = CombineBytes(PasswordByte, Salt);
        byte[] HashedPassword = HashingPassword(CombinedBytes);
        return new CreateHashPasswordModel()
        {
            Salt = Encoding.UTF8.GetBytes(SaltString),
            HashedPassword = HashedPassword
        };
    }

    public static bool VerifyPasswordHashed(string Password, byte[] Salt, byte[] PasswordStored)
    {
        byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
        byte[] CombinedBytes = CombineBytes(PasswordByte, Salt);
        byte[] NewHash = HashingPassword(CombinedBytes);
        return PasswordStored.SequenceEqual(NewHash);
    }

    static byte[] HashingPassword(byte[] PasswordCombined)
    {
        using (SHA256 SHA256 = SHA256.Create())
        {
            byte[] HashBytes = SHA256.ComputeHash(PasswordCombined);
            return HashBytes;
        }
    }

    static byte[] CombineBytes(byte[] First, byte[] Second)
    {
        byte[] Combined = new byte[First.Length + Second.Length];
        Buffer.BlockCopy(First, 0, Combined, 0, First.Length);
        Buffer.BlockCopy(Second, 0, Combined, First.Length, Second.Length);
        return Combined;
    }

    public static string GenerateJWT(User User)
    {
        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        var Credential = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> Claims = new()
        {
            new Claim(ClaimsIdentity.DefaultRoleClaimType, User.Role),
            new Claim("userid", User.Id.ToString()),
            new Claim("email", User.Email),
        };

        var Token = new JwtSecurityToken(
            issuer: Issuser,
            audience: Issuser,
            claims: Claims,
            expires: DateTime.Now.AddHours(5),
            signingCredentials: Credential
            );
        var Encodetoken = new JwtSecurityTokenHandler().WriteToken(Token);
        return Encodetoken;
    }

    public static string GenerateRandomPassword()
    {
        int length = 12;
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        byte[] data = new byte[length];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            byte[] buffer = new byte[sizeof(int)];
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                crypto.GetBytes(buffer);
                int randomNumber = BitConverter.ToInt32(buffer, 0);
                randomNumber = Math.Abs(randomNumber);
                int index = randomNumber % chars.Length;
                result.Append(chars[index]);
            }
            return result.ToString();
        }
    }

    public static string DecodeToken(string jwtToken, string nameClaim)
    {
        var _tokenHandler = new JwtSecurityTokenHandler();
        Claim? claim = _tokenHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
        return claim != null ? claim.Value : "Error!!!";
    }
}