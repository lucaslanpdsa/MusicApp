﻿using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints;

public static class ArtistasExtensions
{
    public static void AddEndPointsArtistas(this WebApplication app)
    {

        #region Endpoint Artistas
        app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
        {
            var listaDeArtistas = dal.Listar();
            if (listaDeArtistas is null)
            {
                return Results.NotFound();
            }
            var listaDeArtistaResponse = EntityListToResponseList(listaDeArtistas);
            return Results.Ok(listaDeArtistaResponse);
        });

        app.MapGet("/Artistas/{idOrNome}", ([FromServices] DAL<Artista> dal, string idOrNome) =>
        {
            // Verifica se o parâmetro é um número (ID)
            if (int.TryParse(idOrNome, out int id))
            {
                // Busca por ID
                var artista = dal.RecuperarPor(a => a.Id.Equals(id));
                if (artista is null)
                {
                    return Results.NotFound("Música não encontrada.");
                }
                return Results.Ok(EntityToResponse(artista));
            }
            else
            {
                // Busca por nome
                var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(idOrNome.ToUpper()));
                if (artista is null)
                {
                    return Results.NotFound("Música não encontrada.");
                }
                return Results.Ok(EntityToResponse(artista));
            }
        });

        app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        {
            var artista = new Artista(artistaRequest.Nome, artistaRequest.Bio, artistaRequest.FotoPerfil);

            dal.Adicionar(artista);
            return Results.Ok();
        });

        app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) => {
            var artista = dal.RecuperarPor(a => a.Id == id);
            if (artista is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(artista);
            return Results.NoContent();

        });

        app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) => {
            var artistaAAtualizar = dal.RecuperarPor(a => a.Id == artistaRequest.Id);
            if (artistaAAtualizar is null)
            {
                return Results.NotFound();
            }
            artistaAAtualizar.Nome = artistaRequest.Nome;
            artistaAAtualizar.Bio = artistaRequest.Bio;
            artistaAAtualizar.FotoPerfil = artistaRequest.FotoPerfil;
            dal.Atualizar(artistaAAtualizar);
            return Results.Ok();
        });
        #endregion
    }

    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }

    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
    }

  
}
