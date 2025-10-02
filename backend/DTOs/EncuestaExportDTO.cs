// DTOs/EncuestaExportDTO.cs
using System;
using System.Collections.Generic;

namespace GestorEncuestas_MVC.DTOs
{
    public class EncuestaExportDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime CierraEn { get; set; }
        public DateTime CreadoEn { get; set; }
        public string Autor { get; set; } = string.Empty;
        public int TotalRespuestas { get; set; }
        public List<PreguntaExportDTO> Preguntas { get; set; } = new List<PreguntaExportDTO>();
    }

    public class PreguntaExportDTO
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string TipoPregunta { get; set; } = string.Empty;
        public bool Obligatorio { get; set; }
        public List<OpcionExportDTO> Opciones { get; set; } = new List<OpcionExportDTO>();
        public List<RespuestaExportDTO> Respuestas { get; set; } = new List<RespuestaExportDTO>();
    }

    public class OpcionExportDTO
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int ConteoSelecciones { get; set; }
    }

    public class RespuestaExportDTO
    {
        public DateTime FechaRespuesta { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string ValorRespuesta { get; set; } = string.Empty;
    }
}