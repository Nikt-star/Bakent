using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Models;

namespace Proyecto3.Core.Interfaces
{
    public interface IColaboradorService
    {
        Task<Colaborador> CreateColaboradorAsync(Colaborador colaborador);
        Task<IEnumerable<Colaborador>> GetAllColaboradoresAsync();
        Task<Colaborador> GetColaboradorDetailsAsync(int id);
        Task UpdateColaboradorAsync(Colaborador colaborador);
        Task DeleteColaboradorAsync(int id);

        // New: create from DTO and add skills/certs
        Task<Colaborador> CreateFromDtoAsync(ColaboradorCreateDto dto);
        Task AddHabilidadesAndCertsAsync(int colaboradorId, ColaboradorUpdateDto dto);
    }
}
