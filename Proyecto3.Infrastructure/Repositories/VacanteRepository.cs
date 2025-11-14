// Proyecto3.Infrastructure/Repositories/VacanteRepository.cs

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces;
using Proyecto3.Infrastructure.Data;

namespace Proyecto3.Infrastructure.Repositories
{
    // Asegúrate de que IColaboradorRepository esté definido en Core/Interfaces
    public class VacanteRepository : IVacanteRepository
    {
        private readonly TalentoInternoDbContext _context;

        public VacanteRepository(TalentoInternoDbContext context)
        {
            _context = context;
        }

        // Método crucial para el algoritmo de Matching (UC-B1)
        public async Task<PerfilesVacante> GetByIdWithRequisitosAsync(int id)
        {
            // Usamos .Include() y .ThenInclude() para cargar los requisitos de la vacante
            // y los nombres de los skills asociados a esos requisitos.
            return await _context.PerfilesVacante
                                 .Include(v => v.RequisitosVacante)
                                 .ThenInclude(r => r.Skill) // Carga el objeto Skill asociado
                                 .FirstOrDefaultAsync(v => v.VacanteID == id);
        }

        // UC-V1: Crear una nueva vacante
        public async Task<PerfilesVacante> AddAsync(PerfilesVacante vacante)
        {
            _context.PerfilesVacante.Add(vacante);
            await _context.SaveChangesAsync();
            return vacante;
        }

        // UC-V2: Obtener todas las vacantes
        public async Task<IEnumerable<PerfilesVacante>> GetAllAsync()
        {
            // Podrías filtrar por Activa == true aquí si quisieras
            return await _context.PerfilesVacante.ToListAsync();
        }

        // UC-V1: Actualizar vacante
        public async Task UpdateAsync(PerfilesVacante vacante)
        {
            _context.Entry(vacante).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // UC-V1: Eliminar vacante
        public async Task DeleteAsync(int id)
        {
            var vacante = await _context.PerfilesVacante.FindAsync(id);
            if (vacante != null)
            {
                _context.PerfilesVacante.Remove(vacante);
                await _context.SaveChangesAsync();
            }
        }
    }
}