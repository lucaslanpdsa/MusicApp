using System.ComponentModel.DataAnnotations;

namespace ScreenSound.API.Requests;
public record ArtistaRequest(int Id, [Required] string Nome, [Required] string Bio, [Required] string FotoPerfil);

