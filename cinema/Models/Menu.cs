using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    // Enumeración para las categorías del menú
    public enum Categoria
    {
        Combos,
        Bebidas,
        Candies,
        Popcorn,
        [Display(Name = "HotDog/Hamburguesa")]
        HotDogHamburguesa,
        Otros
    }

    public class Menu
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Artículo")]
        public string Articulo { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Display(Name = "Foto")]
        public byte[] Foto { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public Categoria Categoria { get; set; }

        // Propiedad calculada para almacenar la categoría en la base de datos como una cadena
        [NotMapped]
        public string CategoriaString
        {
            get { return Categoria.ToString(); }
            set { Categoria = Enum.Parse<Categoria>(value); }
        }
    }
}
