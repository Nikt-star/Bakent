// Proyecto3.Core/Interfaces/IVacanteRepository.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto3.Core.Entities; // Asegúrate de que este using esté correcto

public interface IVacanteRepository
{
    // UC-V1: Crear una nueva vacante
    Task<PerfilesVacante> AddAsync(PerfilesVacante vacante);

    // Método especializado para el Matching Service (UC-B1)
    // Carga la vacante con sus requisitos y los nombres de los skills.
    Task<PerfilesVacante> GetByIdWithRequisitosAsync(int id);

    // UC-V2: Obtener todas las vacantes
    Task<IEnumerable<PerfilesVacante>> GetAllAsync();

    // CRUD: Métodos restantes
    Task UpdateAsync(PerfilesVacante vacante);
    Task DeleteAsync(int id);
}