using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_rpg.Data;

namespace dotnet_rpg.Controllers.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {

        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService( IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor )
        {            
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse( _httpContextAccessor.HttpContext!.User.FindFirstValue( ClaimTypes.NameIdentifier )! );

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter( AddCharacterDto newCharacter )
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = _mapper.Map<Character>( newCharacter );
            character.User =  await _context.Users.FirstOrDefaultAsync( u => u.id == GetUserId() );
            _context.Characters.Add( character );
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Characters
                .Where( c => c.User!.id == GetUserId() )
                .Select( c => _mapper.Map<GetCharacterDto>(c) )
                .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.Where( c => c.User!.id == GetUserId() ).ToListAsync();
            serviceResponse.Data = dbCharacters.Select( c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById( int Id )
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();            
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync( c => c.Id == Id && c.User!.id == GetUserId() );
            serviceResponse.Data = _mapper.Map<GetCharacterDto>( dbCharacter );
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new  ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _context.Characters
                    .Include( c => c.User )
                    .FirstOrDefaultAsync( c => c.Id == updatedCharacter.Id );
                if( character is null || character.User!.id != GetUserId() )
                    throw new Exception( $"Character with id: {updatedCharacter.Id} not found" );

                //character = _mapper.Map<Character>(updatedCharacter);
                character.Name = updatedCharacter.Name;
                character.Defence = updatedCharacter.Defence;
                character.Strength = updatedCharacter.Strength;                
                character.HitPoints = updatedCharacter.HitPoints;
                character.Intelligence = updatedCharacter.Intelligence;

                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch( Exception ex )
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new  ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync( c => c.Id == id && c.User!.id == GetUserId() );
                if( character is null )
                    throw new Exception( $"Character with id: {id} not found" );

                _context.Characters.Remove( character );
                await _context.SaveChangesAsync();
                serviceResponse.Data = await _context.Characters
                    .Where( c => c.User!.id == GetUserId() )
                    .Select( c => _mapper.Map<GetCharacterDto>(c) )
                    .ToListAsync();
            }
            catch( Exception ex )
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}