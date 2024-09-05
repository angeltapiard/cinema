using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Asiento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(1)]
        public char Fila { get; set; }

        [Required]
        public int Columna { get; set; }

        [Required]
        public bool Ocupado { get; set; }

    }
}
