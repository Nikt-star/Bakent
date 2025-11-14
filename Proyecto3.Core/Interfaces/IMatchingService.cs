using Proyecto3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto3.Core.Interfaces
{
    public interface IMatchingService
    {
        // Método principal para el caso de uso: Búsqueda de candidatos (UC-B1)
        Task<IEnumerable<MatchResult>> FindCandidatesForVacancyAsync(int vacanteId);
    }
}
