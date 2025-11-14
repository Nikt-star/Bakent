// Proyecto3/Controllers/MatchingController.cs

using Microsoft.AspNetCore.Mvc;
using Proyecto3.Core.Interfaces;
using Proyecto3.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;

    // Inyección del Servicio de Matching
    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    // [GET] api/matching/candidates/{vacanteId} (UC-B1: Búsqueda Automática)
    [HttpGet("candidates/{vacanteId}")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MatchResult>))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCandidates(int vacanteId)
    {
        // El servicio ejecuta toda la lógica de obtención de datos y cálculo del porcentaje.
        var resultados = await _matchingService.FindCandidatesForVacancyAsync(vacanteId);

        if (resultados == null || !resultados.Any())
        {
            return NotFound($"No se encontraron candidatos internos con un 50% o más de match para la vacante ID {vacanteId}.");
        }

        return Ok(resultados);
    }
}