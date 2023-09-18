using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public AuthRepository( DataContext context, IConfiguration configuration )
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login( string username, string password )
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync( u => u.UserName.ToLower() == username.ToLower() );
            if( user is null )
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if( !VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt ) )
            {
                response.Success = false;
                response.Message = "Wong password.";
            }
            else
            {
                response.Success = true;
                response.Data = CreateToken( user );
            }

            return response;
        }

        public async Task<ServiceResponse<int>> Register( User user, string password )
        {            
            var response = new ServiceResponse<int>();

            if( await UserExists(user.UserName) )
            {
                response.Success = false;
                response.Message = "User already exists.";                
            }
            else
            {
                CreatePasswordHash( password, out byte[] passwordHash, out byte[] passwordSalt );
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _context.Users.Add( user );
                await _context.SaveChangesAsync();            
                response.Data = user.id;
            }
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if( await _context.Users.AnyAsync( x => x.UserName.ToLower() == username.ToLower() ) )
                return true;
            else
                return false;        
        }

        private void CreatePasswordHash( string password, out byte[] passwordHash, out byte[] passwordSalt )
        {
            using( var hmac = new HMACSHA512() )
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash( Encoding.UTF8.GetBytes( password ) );
            }
        }

        private bool VerifyPasswordHash( string password, byte[] passwordHash, byte[] passwordSalt )
        {
            using( var hmac = new HMACSHA512(passwordSalt) )
            {
                var computedHash = hmac.ComputeHash( Encoding.UTF8.GetBytes( password ) );
                return computedHash.SequenceEqual( passwordHash );
            }
        }

        private string CreateToken( User user )
        {
            var claim = new List<Claim>
            {
                new Claim( ClaimTypes.NameIdentifier, user.id.ToString() ),
                new Claim( ClaimTypes.Name, user.UserName )
            };
            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if( appSettingsToken is null )
                throw new Exception( "AppSettings Token is null" );

            SymmetricSecurityKey key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(appSettingsToken) );
            SigningCredentials creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha512Signature );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddDays( 1 ),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken( tokenDescriptor );

            return tokenHandler.WriteToken(token);;
        }
    }
}