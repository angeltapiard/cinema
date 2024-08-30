using System.Collections.Generic;

namespace Cinema.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public ICollection<Menu> Menus { get; set; } // Asegúrate de que la clase Menu esté definida
    }
}