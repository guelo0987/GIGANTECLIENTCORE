using GIGANTECLIENTCORE.Context;
using GIGANTECLIENTCORE.DTO;
using GIGANTECLIENTCORE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIGANTECLIENTCORE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ILogger<UserController> _logger;

    public UserController(MyDbContext db, ILogger<UserController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Get user profile
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var user = await _db.UsuarioClientes
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            _logger.LogError("Usuario no encontrado");
            return NotFound("Usuario no encontrado");
        }

        var userDto = new UserClienteDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Direccion = user.Direccion,
            Ciudad = user.Ciudad,
            Apellidos = user.Apellidos,
            Telefono = user.Telefono,
            Rnc = user.Rnc,
            Dob = user.Dob,
            FechaIngreso = user.FechaIngreso,
            RolId = user.RolId
        };

        return Ok(userDto);
    }

    // Update user profile
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDTO updateProfile)
    {
        var user = await _db.UsuarioClientes.FindAsync(id);

        if (user == null)
        {
            _logger.LogError("Usuario no encontrado");
            return NotFound("Usuario no encontrado");
        }

        // Update user properties
        user.UserName = updateProfile.UserName;
        user.Direccion = updateProfile.Direccion;
        user.Ciudad = updateProfile.Ciudad;
        user.Apellidos = updateProfile.Apellidos;
        user.Telefono = updateProfile.Telefono;
        user.Rnc = updateProfile.Rnc;
        user.Dob = updateProfile.Dob;

        try
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("Perfil actualizado exitosamente");
            return Ok("Perfil actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el perfil");
            return StatusCode(500, "Error al actualizar el perfil");
        }
    }

    // Change password
    [HttpPut("change-password/{id}")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDTO changePassword)
    {
        var user = await _db.UsuarioClientes.FindAsync(id);

        if (user == null)
        {
            _logger.LogError("Usuario no encontrado");
            return NotFound("Usuario no encontrado");
        }

        // Verify current password
        if (user.Password != changePassword.CurrentPassword)
        {
            _logger.LogError("Contraseña actual incorrecta");
            return BadRequest("Contraseña actual incorrecta");
        }

        // Update password
        user.Password = changePassword.NewPassword;

        try
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("Contraseña actualizada exitosamente");
            return Ok("Contraseña actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la contraseña");
            return StatusCode(500, "Error al actualizar la contraseña");
        }
    }
}