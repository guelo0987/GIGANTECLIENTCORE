using System.Text.Json.Serialization;
using Octetus.ConsultasDgii.Services;
using System.Text.Json.Serialization;
namespace GIGANTECLIENTCORE.Utils;

public class DgiiService
{
    public string ConsultarContribuyente(string rnc)
    {
        string nombre = string.Empty;
        
        var dgii = new ServicioConsultasWebDgii();
        var response = dgii.ConsultarRncRegistrados(rnc);
        Console.WriteLine(response.Nombre);
        Console.WriteLine(response.RncOCedula);
        
        
        if (response.Success)
        {
            nombre = response.Nombre;
        }
        Console.WriteLine(nombre);
        return nombre;
    }
}
