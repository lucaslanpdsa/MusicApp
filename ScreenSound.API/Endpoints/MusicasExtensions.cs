﻿using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints;

public static class MusicasExtensions
{ 
    public static void AddEndPointsMusicas(this WebApplication app)
    {
        #region Endpoint Músicas
        app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
        {
            var musicaList = dal.Listar();
            if (musicaList is null)
            {
                return Results.NotFound();
            }
            var musicaListResponse = EntityListToResponseList(musicaList);
            return Results.Ok(musicaListResponse);
        });

        app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero,[FromBody] MusicaRequest musicaRequest) =>
        {
            var musica = new Musica(musicaRequest.nome) 
            {                 
                ArtistaId = musicaRequest.ArtistaId,
                AnoLancamento = musicaRequest.anoLancamento,
                Generos = musicaRequest.Generos is not null?GeneroRequestConverter(musicaRequest.Generos, dalGenero) :
                new List<Genero>()

            };
            dal.Adicionar(musica);
            return Results.Ok();
        });

        app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) => {
            var musica = dal.RecuperarPor(a => a.Id == id);
            if (musica is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(musica);
            return Results.NoContent();

        });

        app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequest musicaRequest) => {
            var musicaAAtualizar = dal.RecuperarPor(a => a.Id == musicaRequest.Id);
            if (musicaAAtualizar is null)
            {
                return Results.NotFound();
            }
            musicaAAtualizar.Nome = musicaRequest.nome;
            musicaAAtualizar.AnoLancamento = musicaRequest.anoLancamento;

            dal.Atualizar(musicaAAtualizar);
            return Results.Ok();
        });
        #endregion

    }

    private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
    {
       var listaDeGeneros = new List<Genero>();
        foreach (var item in generos)
        {
            var entity = RequestToEntity(item);
            var genero = dalGenero.RecuperarPor(g=>g.Nome.ToUpper().Equals(item.Nome.ToUpper()));
            if (genero is not null)
            {
                listaDeGeneros.Add(genero);
            }
            else
            {
                listaDeGeneros.Add(entity);
            }
        }

        return listaDeGeneros;
    }

    private static Genero RequestToEntity(GeneroRequest genero)
    {
        return new Genero() { Nome = genero.Nome };
    }

    private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
    {
        return musicaList.Select(a => EntityToResponse(a)).ToList();
    }

    private static MusicaResponse EntityToResponse(Musica musica)
    {
        return new MusicaResponse(musica.Id, musica.Nome!, musica.Generos, musica.AnoLancamento, musica.Artista );
    }
}
