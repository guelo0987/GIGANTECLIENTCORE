using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.Models;
using GIGANTECLIENTCORE.DTO;
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
        
        // POST: api/vacantes
        [HttpPost]
        public async Task<IActionResult> CreateVacante([FromBody] VacanteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var vacante = new Vacantes()
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
                // Si no se envía CurriculumUrl, se mantiene en null
                CurriculumUrl = dto.CurriculumUrl,
                FechaAplicacion = DateTime.Now,
                // Valor por defecto "Pendiente" según la definición de la tabla
                estado = "Pendiente"
            };
            
            _db.Vacantes.Add(vacante);
            await _db.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetVacanteById), new { id = vacante.id }, MapToDto(vacante));
        }
        
        // GET: api/vacantes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVacanteById(int id)
        {
            var vacante = await _db.Vacantes.FirstOrDefaultAsync(v => v.id == id);
            if (vacante == null)
            {
                _logger.LogError("Vacante con Id {Id} no encontrada", id);
                return NotFound($"Vacante con Id {id} no encontrada.");
            }
            
            return Ok(MapToDto(vacante));
        }
        
        // Método privado para mapear de Vacante a VacanteDto
        private VacanteDTO MapToDto(Vacantes vacante)
        {
            return new VacanteDTO()
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
