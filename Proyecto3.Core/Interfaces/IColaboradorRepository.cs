// Proyecto3.Core/Interfaces/IColaboradorRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto3.Core.Entities;

public interface IColaboradorRepository
{
    // CRUD básico
    Task<Colaborador> AddAsync(Colaborador colaborador);
    Task<IEnumerable<Colaborador>> GetAllAsync();
    Task UpdateAsync(Colaborador colaborador);
    Task DeleteAsync(int id);

    // Método especializado: Obtener con relaciones (Skills/Certificaciones)
    Task<Colaborador> GetByIdAsync(int id);

    // Método especializado para el Matching Service
    Task<IEnumerable<Colaborador>> GetAllWithHabilidadesAsync();
}