using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Cinema.Models
{
    public enum Clasificacion
    {
        G,          // General Audience  
        PG,         // Parental Guidance  
        PG13,       // Parents Strongly Cautioned 
        R,          // Restricted  
        NC17        // No One 17 and Under Admitted  
    }

    // Enumeración para los géneros con los valores proporcionados
    public enum Genero
    {
        Acción,
        Aventura,
        Comedia,
        Drama,
        [Display(Name = "Ciencia Ficción")]
        CienciaFiccion,
        Fantasía,
        Terror,
        [Display(Name = "Suspenso (Thriller)")]
        Suspenso,
        Misterio,
        Romance,
        Animación,
        Musical,
        Documental,
        Biográfico,
        Histórico,
        Bélico,
        Western,
        Policíaco,
        Deportes,
        [Display(Name = "Superhéroes")]
        Superheroes
    }

    public class Pelicula
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Duración (minutos)")]
        public int Duracion { get; set; }

        [Required]
        [Display(Name = "Clasificación")]
        public Clasificacion Clasificacion { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Póster")]
        public byte[] Poster { get; set; }

        // Nueva propiedad para Géneros
        [Display(Name = "Géneros")]
        public List<Genero> Generos { get; set; } = new List<Genero>();

        // Propiedad calculada para almacenar los géneros en la base de datos como una cadena separada por comas
        [NotMapped]
        public string GenerosString
        {
            get { return string.Join(",", Generos); }
            set { Generos = value.Split(',').Select(g => Enum.Parse<Genero>(g)).ToList(); }
        }
    }
}
