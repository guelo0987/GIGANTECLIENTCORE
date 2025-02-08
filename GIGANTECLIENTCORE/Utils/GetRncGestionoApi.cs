using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GIGANTECLIENTCORE.Utils
{
    public class DgiiService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<string> ConsultarContribuyenteAsync(string rnc)
        {
            string nombre = string.Empty;
            string url = $"https://api.gestiono.app/v1/beneficiary/taxId/{rnc}?deepCheck=false";

            try
            {
                var response = await httpClient.GetStringAsync(url);
                var contribuyente = JsonSerializer.Deserialize<ContribuyenteResponse>(response);
                
                if (contribuyente != null)
                {
                    nombre = !string.IsNullOrEmpty(contribuyente.Name) ? contribuyente.Name : "Nombre no disponible";
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error en la consulta: {ex.Message}");
            }

            return nombre;
        }
    }

    public class ContribuyenteResponse
    {
        [JsonPropertyName("taxId")]
        public string TaxId { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("comercialName")]
        public string ComercialName { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("activity")]
        public string Activity { get; set; }
        
        [JsonPropertyName("taxAdministration")]
        public string TaxAdministration { get; set; }
        
        [JsonPropertyName("registryDate")]
        public string RegistryDate { get; set; }
        
        [JsonPropertyName("paymentRegime")]
        public string PaymentRegime { get; set; }
    }
}