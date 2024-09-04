using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Sala
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoSala { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }
    }
}
