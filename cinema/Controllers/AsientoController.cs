using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
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

        // Método para mostrar los detalles de un asiento en una sala específica
        public IActionResult Detalles(int id)
        {
            Sala sala = null;
            List<Asiento> asientos = new List<Asiento>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Obtener detalles de la sala
                string salaQuery = "SELECT Id, Nombre, TipoSala, Precio FROM Salas WHERE Id = @Id";
                SqlCommand salaCmd = new SqlCommand(salaQuery, con);
                salaCmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader salaReader = salaCmd.ExecuteReader();

                if (salaReader.Read())
                {
                    sala = new Sala
                    {
                        Id = salaReader.GetInt32(0),
                        Nombre = salaReader.GetString(1),
                        TipoSala = salaReader.GetString(2),
                        Precio = salaReader.GetDecimal(3)
                    };
                }

                salaReader.Close(); // Cerrar el reader para reutilizar la conexión

                // Obtener los asientos de la sala
                string asientosQuery = "SELECT Id, Fila, Columna, Ocupado FROM Asientos WHERE SalaId = @SalaId";
                SqlCommand asientosCmd = new SqlCommand(asientosQuery, con);
                asientosCmd.Parameters.AddWithValue("@SalaId", id);
                SqlDataReader asientosReader = asientosCmd.ExecuteReader();

                while (asientosReader.Read())
                {
                    asientos.Add(new Asiento
                    {
                        Id = asientosReader.GetInt32(0),
                        Fila = asientosReader.GetString(1)[0],
                        Columna = asientosReader.GetInt32(2),
                        Ocupado = asientosReader.GetBoolean(3)
                    });
                }
            }

            if (sala == null)
            {
                return NotFound();
            }

            var model = new DetallesViewModel
            {
                Sala = sala,
                Asientos = asientos
            };
            return View(model);
        }

        // Método para seleccionar o cambiar el estado de un asiento
        [HttpPost]
        public IActionResult Seleccionar(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                // Primero obtenemos el estado actual del asiento
                string checkQuery = "SELECT Ocupado FROM Asientos WHERE Id = @Id";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                bool ocupado;

                using (SqlDataReader reader = checkCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ocupado = reader.GetBoolean(0);
                    }
                    else
                    {
                        return NotFound();
                    }
                }

                // Luego actualizamos el estado del asiento
                string updateQuery = "UPDATE Asientos SET Ocupado = @Ocupado WHERE Id = @Id";
                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@Id", id);
                updateCmd.Parameters.AddWithValue("@Ocupado", !ocupado); // Cambiar el estado
                updateCmd.ExecuteNonQuery();
            }

            // Redirigir a la vista de detalles de la sala correspondiente (si es necesario)
            // En este caso, redirigimos a la vista de asientos generales
            return RedirectToAction("Index");
        }
    }
}
