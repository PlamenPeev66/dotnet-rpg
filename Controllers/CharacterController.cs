using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace dotnet_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController( ICharacterService characterService )
        {
            this._characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
        {
            int userId = int.Parse( User.Claims.FirstOrDefault( c => c.Type == ClaimTypes.NameIdentifier )!.Value );
            return Ok( await _characterService.GetAllCharacters(userId) );
        }
        
        [HttpGet("{Id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int Id)
        {
            return Ok( await _characterService.GetCharacterById(Id) );
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<Character>>>> AddCharacter( AddCharacterDto newCharacter )
        {
            return Ok( await _characterService.AddCharacter(newCharacter) );
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<Character>>>> UpdateCharacter( UpdateCharacterDto updatedCharacter )
        {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if( response.Data is null )
                return NotFound(response);
            else            
                return Ok(response);
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteCharacter(int Id)
        {
            var response = await _characterService.DeleteCharacter(Id);
            if( response.Data is null )
                return NotFound(response);
            else            
                return Ok(response);
        }

    }
}