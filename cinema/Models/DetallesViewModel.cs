namespace Cinema.Models
{
    public class DetallesViewModel
    {
        public Sala Sala { get; set; }
        public List<Asiento> Asientos { get; set; }
        public Asiento AsientoSeleccionado { get; set; }
    }
}
