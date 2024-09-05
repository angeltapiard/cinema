using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Cinema.Models;
using Microsoft.Extensions.Configuration;

namespace Cinema.Controllers
{
    public class AsientoController : Controller
    {
        private readonly string _connectionString;

        public AsientoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Método para obtener todos los asientos
        public IActionResult Index()
        {
            List<Asiento> asientos = new List<Asiento>();

            // Conexión a la base de datos y obtención de los asientos
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Asientos"; // Asegúrate de que la tabla se llame 'Asientos'
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Leer los datos y llenar la lista de asientos
                while (reader.Read())
                {
                    asientos.Add(new Asiento
                    {
                        Id = (int)reader["Id"],
                        Fila = reader["Fila"].ToString()[0], // Cambiado para obtener el primer carácter
                        Columna = (int)reader["Columna"],
                        Ocupado = (bool)reader["Ocupado"]
                    });
                }
            }

            // Pasar la lista de asientos a la vista
            return View(asientos);
        }

        // Método para seleccionar o cambiar el estado de un asiento
        [HttpPost]
        public IActionResult Seleccionar(int id)
        {
            // Conexión a la base de datos y actualización del asiento
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Asientos SET Ocupado = ~Ocupado WHERE Id = @Id"; // Cambia el estado del asiento
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            // Redirigir de nuevo a la vista Index
            return RedirectToAction("Index");
        }
    }
}