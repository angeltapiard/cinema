namespace Cinema.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public byte[] Foto { get; set; }
        public int CategoriaId { get; set; } // Clave foránea para la categoría
        public Categoria Categoria { get; set; } // Relación de navegación
    }
}