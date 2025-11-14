// Proyecto3.Infrastructure/Repositories/ColaboradorRepository.cs

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proyecto3.Core.Entities;
using Proyecto3.Core.Interfaces;
using Proyecto3.Infrastructure.Data; // Necesita esta carpeta/clase

namespace Proyecto3.Infrastructure.Repositories
{
    // Cambiamos 'internal class' por 'public class' e implementamos la interfaz
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly TalentoInternoDbContext _context;

        // Inyección de Dependencias: Recibe el contexto de la base de datos
        public ColaboradorRepository(TalentoInternoDbContext context)
        {
            _context = context;
        }

        public async Task<Colaborador> AddAsync(Colaborador colaborador)
        {
            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }

        // Método especializado para obtener detalles (incluye habilidades y certificaciones)
        public async Task<Colaborador> GetByIdAsync(int id)
        {
            // Usamos .Include() y .ThenInclude() para cargar las relaciones. 
            // Esto es crucial para el algoritmo de matching.
            return await _context.Colaboradores
                                 .Include(c => c.Habilidades)
                                     .ThenInclude(h => h.Skill)
                                 .Include(c => c.Certificaciones)
                                 .FirstOrDefaultAsync(c => c.ColaboradorID == id);
        }

        // Método especializado para el Matching Service (carga todas las habilidades de todos)
        public async Task<IEnumerable<Colaborador>> GetAllWithHabilidadesAsync()
        {
            return await _context.Colaboradores
                                 .Include(c => c.Habilidades)
                                     .ThenInclude(h => h.Skill)
                                 .ToListAsync();
        }

        // Obtener todos (CRUD básico)
        public async Task<IEnumerable<Colaborador>> GetAllAsync()
        {
            return await _context.Colaboradores.ToListAsync();
        }

        public async Task UpdateAsync(Colaborador colaborador)
        {
            _context.Entry(colaborador).State = EntityState.Modified;
            // Asegúrate de que las entidades relacionadas se manejen correctamente en el servicio
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador != null)
            {
                _context.Colaboradores.Remove(colaborador);
                await _context.SaveChangesAsync();
            }
        }
    }
}