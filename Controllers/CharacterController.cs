using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnet_rpg.Controllers
{
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
        public async Task<ActionResult<ServiceResponse<List<Character>>>> Get()
        {
            return Ok( await _characterService.GetAllCharacters() );
        }
        
        [HttpGet("{Id}")]
        public async Task<ActionResult<ServiceResponse<Character>>> GetSingle(int Id)
        {
            return Ok( await _characterService.GetCharacterById(Id) );
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<Character>>>> AddCharacter( Character newCharacter )
        {
            return Ok( await _characterService.AddCharacter(newCharacter) );
        }
    }
}