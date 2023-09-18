using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<string>> Login( string username, string password );
        Task<ServiceResponse<int>> Register( User user, string password );
        Task<bool> UserExists( string username );
    }
}