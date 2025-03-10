namespace GIGANTECLIENTCORE.DTO
{
    public class UpdateProfileDTO
    {
        public string UserName { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string? Rnc { get; set; }
        public DateOnly Dob { get; set; }
    }
} 