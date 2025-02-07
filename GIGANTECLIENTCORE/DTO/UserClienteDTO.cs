namespace GIGANTECLIENTCORE.DTO;

public class UserClienteDTO
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string? Password { get; set; }

    public string? Direccion { get; set; }

    public string? Ciudad { get; set; }

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Rnc { get; set; }

    public DateOnly? Dob { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int RolId { get; set; }
}