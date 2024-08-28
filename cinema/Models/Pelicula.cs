using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}