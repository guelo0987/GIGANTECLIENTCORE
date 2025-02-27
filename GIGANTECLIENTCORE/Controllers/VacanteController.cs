using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;
using GIGANTECLIENTCORE.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace GIGANTECLIENTCORE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacantesController : ControllerBase
    {
        private readonly ILogger<VacantesController> _logger;
        private readonly MyDbContext _db;
        private const string ExternalFilesPath = "/Users/miguelcruz/ImageGigante/Curriculums";

        public VacantesController(MyDbContext db, ILogger<VacantesController> logger)
        {
            _db = db;
            _logger = logger;
            
            // Asegurar que el directorio externo existe
            Directory.CreateDirectory(ExternalFilesPath);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVacante([FromForm] CreateVacanteRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validación del archivo
            if (dto.Curriculum == null || dto.Curriculum.Length == 0)
            {
                return BadRequest("Debe subir un currículum");
            }

            var fileExtension = Path.GetExtension(dto.Curriculum.FileName).ToLower();
            if (fileExtension != ".pdf")
            {
                return BadRequest("Solo se permiten archivos PDF");
            }

            // Generar nombre único para el archivo
            var fileName = $"{Guid.NewGuid()}_{SanitizeFileName(dto.cedula)}.pdf";
            var filePath = Path.Combine(ExternalFilesPath, fileName);

            // Guardar el archivo en la ruta externa
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Curriculum.CopyToAsync(stream);
            }

            // Mapeo a entidad
            var vacante = new Vacantes
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
                CurriculumUrl = $"/{fileName}",
                FechaAplicacion = DateTime.Now,
                estado = "Pendiente"
            };

            _db.Vacantes.Add(vacante);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVacanteById), 
                new { id = vacante.id }, 
                MapToResponseDto(vacante)
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVacanteById(int id)
        {
            var vacante = await _db.Vacantes.FindAsync(id);
            if (vacante == null)
            {
                _logger.LogWarning("Vacante con ID {Id} no encontrada", id);
                return NotFound();
            }
            return Ok(MapToResponseDto(vacante));
        }
        

        private VacanteResponseDTO MapToResponseDto(Vacantes vacante)
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

        private string SanitizeFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                .Aggregate(fileName, (current, c) => current.Replace(c, '_'));
        }
    }
}