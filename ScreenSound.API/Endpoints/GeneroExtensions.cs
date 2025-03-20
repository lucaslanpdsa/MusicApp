using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints;

public static class GeneroExtensions
{

    public static void AddEndPointGeneros(this WebApplication app)
    {
        app.MapPost("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest) =>
        {
            dal.Adicionar(new Genero() { Nome = generoRequest.Nome });
        });

        app.MapGet("/Generos", ([FromServices] DAL<Genero> dal) =>
        {
            return EntityListToResponseList(dal.Listar());
        });

        app.MapGet("/Generos/{nomeOuId}", ([FromServices] DAL<Genero> dal, string nomeOuId) =>
        {
            // Verifica se o parâmetro é um número (ID) ou uma string (nome)
            if (int.TryParse(nomeOuId, out int id))
            {
                // Busca o gênero por ID
                var genero = dal.RecuperarPor(a => a.Id == id);
                if (genero is not null)
                {
                    var response = EntityToResponse(genero);
                    return Results.Ok(response);
                }
            }
            else
            {
                // Busca o gênero por nome (case-insensitive)
                var genero = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nomeOuId.ToUpper()));
                if (genero is not null)
                {
                    var response = EntityToResponse(genero);
                    return Results.Ok(response);
                }
            }

            // Se não encontrar, retorna NotFound
            return Results.NotFound("Gênero não encontrado.");
        });

        app.MapDelete("/Generos/{id}", ([FromServices] DAL<Genero> dal, int id) =>
        {
            var genero = dal.RecuperarPor(a => a.Id == id);
            if (genero is null)
            {
                return Results.NotFound("Gênero para exclusão não encontrado.");
            }
            dal.Deletar(genero);
            return Results.NoContent();
        });
    }

    private static ICollection<GeneroResponse> EntityListToResponseList(IEnumerable<Genero> generos)
    {
        return generos.Select(a => EntityToResponse(a)).ToList();
    }

    private static GeneroResponse EntityToResponse(Genero genero)
    {
        return new GeneroResponse(genero.Id,genero.Nome!);
    }
}
