using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;
using GIGANTECLIENTCORE.DTO;
using GIGANTECLIENTCORE.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacantesController : ControllerBase
    {
        private readonly ILogger<VacantesController> _logger;
        private readonly MyDbContext _db;

        public VacantesController(MyDbContext db, ILogger<VacantesController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVacante([FromForm] CreateVacanteRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Curriculum == null || dto.Curriculum.Length == 0)
            {
                return BadRequest("Debe subir un currículum");
            }

            var adminMedia = new AdminCurriculumMedia(_db);
            var uploadResult = await adminMedia.Upload(dto.Curriculum, dto.cedula);

            if (uploadResult is { } result && result.GetType().GetProperty("success")?.GetValue(result) is bool success)
            {
                if (!success)
                {
                    return BadRequest(new { Message = result.GetType().GetProperty("message")?.GetValue(result)?.ToString() });
                }

                var fileName = result.GetType().GetProperty("fileName")?.GetValue(result)?.ToString();

                var vacante = new vacantes
                {
                    nombre = dto.nombre,
                    cedula = dto.cedula,
                    Correo = dto.Correo,
                    telefono = dto.telefono,
                    sexo = dto.sexo,
                    NivelAcademico = dto.NivelAcademico,
                    AnosExperiencia = dto.AnosExperiencia,
                    FuncionLaboral = dto.FuncionLaboral,
                    OtraFuncionLaboral = dto.OtraFuncionLaboral,
                    UltimoSalario = dto.UltimoSalario,
                    NivelLaboral = dto.NivelLaboral,
                    OtroNivelLaboral = dto.OtroNivelLaboral,
                    CurriculumUrl = fileName,
                    FechaAplicacion = DateTime.UtcNow,
                    estado = "Pendiente"
                };

                _db.vacantes.Add(vacante);
                await _db.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetVacanteById),
                    new { id = vacante.id },
                    MapToResponseDto(vacante)
                );
            }

            return BadRequest("Error al procesar el archivo");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVacanteById(int id)
        {
            var vacante = await _db.vacantes.FindAsync(id);
            if (vacante == null)
            {
                _logger.LogWarning("Vacante con ID {Id} no encontrada", id);
                return NotFound();
            }
            return Ok(MapToResponseDto(vacante));
        }
        

        private VacanteResponseDTO MapToResponseDto(vacantes vacante)
        {
            return new VacanteResponseDTO
            {
                id = vacante.id,
                nombre = vacante.nombre,
                cedula = vacante.cedula,
                Correo = vacante.Correo,
                telefono = vacante.telefono,
                sexo = vacante.sexo,
                NivelAcademico = vacante.NivelAcademico,
                AnosExperiencia = vacante.AnosExperiencia,
                FuncionLaboral = vacante.FuncionLaboral,
                OtraFuncionLaboral = vacante.OtraFuncionLaboral,
                UltimoSalario = vacante.UltimoSalario,
                NivelLaboral = vacante.NivelLaboral,
                OtroNivelLaboral = vacante.OtroNivelLaboral,
                CurriculumUrl = vacante.CurriculumUrl,
                FechaAplicacion = vacante.FechaAplicacion,
                estado = vacante.estado
            };
        }
    }
}