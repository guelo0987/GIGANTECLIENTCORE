using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GIGANTECLIENTCORE.DTO;

// DTO para la creación (REQUEST)
public class CreateVacanteRequestDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string nombre { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria")]
    public string cedula { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Correo { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    public string telefono { get; set; }

    [Required(ErrorMessage = "El sexo es obligatorio")]
    public char sexo { get; set; }

    [Required(ErrorMessage = "El nivel académico es obligatorio")]
    public string NivelAcademico { get; set; }

    public int? AnosExperiencia { get; set; }

    [Required(ErrorMessage = "La función laboral es obligatoria")]
    public string FuncionLaboral { get; set; }
    
    public string? OtraFuncionLaboral { get; set; }
    
    [Range(0, 999999.99, ErrorMessage = "Salario inválido")]
    public decimal? UltimoSalario { get; set; }

    [Required(ErrorMessage = "El nivel laboral es obligatorio")]
    public string NivelLaboral { get; set; }
    
    public string? OtroNivelLaboral { get; set; }

    [Required(ErrorMessage = "Debe adjuntar un currículum")]
    [DataType(DataType.Upload)]
    public IFormFile Curriculum { get; set; }
}

// DTO para la respuesta (RESPONSE)
public class VacanteResponseDTO
{
    public int id { get; set; }
    public string nombre { get; set; }
    public string cedula { get; set; }
    public string Correo { get; set; }
    public string telefono { get; set; }
    public char sexo { get; set; }
    public string NivelAcademico { get; set; }
    public int? AnosExperiencia { get; set; }
    public string FuncionLaboral { get; set; }
    public string? OtraFuncionLaboral { get; set; }
    public decimal? UltimoSalario { get; set; }
    public string NivelLaboral { get; set; }
    public string? OtroNivelLaboral { get; set; }
    public string CurriculumUrl { get; set; }
    public DateTime FechaAplicacion { get; set; }
    public string estado { get; set; }
}