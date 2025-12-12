using Microsoft.AspNetCore.Mvc;
using TravelFriend;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IJwtService _jwtService;

        public UsuarioController(IUsuarioService usuarioService, IJwtService jwtService)
        {
            _usuarioService = usuarioService;
            _jwtService = jwtService;
        }


        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            if (await _usuarioService.ExisteUsuarioAsync(usuario.UsuarioNombre, usuario.Email))
                return BadRequest(new { mensaje = "El nombre de usuario o email ya están registrados." });

            usuario.FechaCreacion = DateTime.Now;

            // El servicio se encarga de hashear la contraseña con BCrypt
            var creado = await _usuarioService.CrearUsuarioAsync(usuario);

            return Ok(new
            {
                mensaje = "Usuario creado correctamente",
                usuario = new UsuarioDto(creado)
            });
        }

        // prueba para token

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _usuarioService.ValidarLoginAsync(dto.Usuario, dto.Password);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            var token = _jwtService.GenerarToken(usuario);

            return Ok(new
            {
                mensaje = "Login exitoso",
                token
            });
        }

        [HttpGet("perfil")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);

            if (idClaim == null) return Unauthorized(new { mensaje = "Token inválido" });

            var usuario = await _usuarioService.ObtenerPorIdAsync(int.Parse(idClaim.Value));

            return usuario == null ? NotFound() : Ok(usuario);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Ok(await _usuarioService.ObtenerTodosAsync());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            return usuario == null ? NotFound(new { mensaje = "Usuario no encontrado" }) : Ok(usuario);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminarUsuario(int id)
        {

            string resultado = await _usuarioService.EliminarUsuarioAsync(id);

            if (resultado == "OK")
            {
                return Ok(new { mensaje = "Cuenta eliminada correctamente. ¡Te extrañaremos!" });
            }
            else
            {

                return BadRequest(new { mensaje = resultado });
            }
        }

    }
}
