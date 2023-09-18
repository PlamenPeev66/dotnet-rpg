using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Controllers.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter( AddCharacterDto newCharacter );
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter( UpdateCharacterDto updatedCharacter );
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters( int userId );
        Task<ServiceResponse<GetCharacterDto>> GetCharacterById( int id );        
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter( int id );  
    }
}