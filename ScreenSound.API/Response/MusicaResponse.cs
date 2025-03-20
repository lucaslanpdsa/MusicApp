using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Response;

public record MusicaResponse(int Id, string Nome, ICollection<Genero> generos, int AnoLancamento, Artista Artista);