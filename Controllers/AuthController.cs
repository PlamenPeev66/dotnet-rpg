using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public IAuthRepository _authRepo;
        public AuthController( IAuthRepository authRepo )
        {
            _authRepo = authRepo;
            
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register( UserRegisterDto request )
        {
            var response = await _authRepo.Register( new User { UserName = request.Username }, request.Password );

            if( response.Success )
                return Ok(response);
            else
                return BadRequest(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login( UserLoginDto request )
        {
            var response = await _authRepo.Login( request.Username, request.Password );

            if( response.Success )
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}