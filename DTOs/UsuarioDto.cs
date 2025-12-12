namespace TravelFriend;

 public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UsuarioC { get; set; }
        public string Email { get; set; }
            public string AvatarColor{ get; set; }

        

        public UsuarioDto(Usuario usuario)
        {
            Id = usuario.Id;
            Nombre = usuario.Nombre;
            UsuarioC = usuario.UsuarioNombre; 
            Email = usuario.Email;
            AvatarColor = usuario.AvatarColor;
        }
    }