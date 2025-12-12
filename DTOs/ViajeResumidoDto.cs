namespace TravelFriend;


    public class ViajeResumenDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public string Estado { get; set; }
        
        public UsuarioMinimoDto Creador { get; set; }
        public List<UsuarioMinimoDto> Participantes { get; set; } = new();
    }

