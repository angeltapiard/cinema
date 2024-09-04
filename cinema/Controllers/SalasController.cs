using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using Cinema.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Cinema.Controllers
{
    public class SalasController : Controller
    {
        private readonly string _connectionString;

        public SalasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Acción para mostrar la lista de salas
        public IActionResult Index()
        {
            List<Sala> salas = new List<Sala>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombre, TipoSala, Precio FROM Salas";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Sala sala = new Sala
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        TipoSala = reader.GetString(2),
                        Precio = reader.GetDecimal(3)
                    };

                    salas.Add(sala);
                }
            }

            return View(salas);
        }

        // Acción para mostrar el formulario de agregar sala
        public IActionResult Crear()
        {
            return View();
        }

        // Acción para procesar el formulario de agregar sala
        [HttpPost]
        public IActionResult Crear(Sala sala)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO Salas (Nombre, TipoSala, Precio) VALUES (@Nombre, @TipoSala, @Precio)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nombre", sala.Nombre);
                    cmd.Parameters.AddWithValue("@TipoSala", sala.TipoSala);
                    cmd.Parameters.AddWithValue("@Precio", sala.Precio);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(sala);
        }

        // Acción para mostrar detalles de una sala
        public IActionResult Detalles(int id)
        {
            Sala sala = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombre, TipoSala, Precio FROM Salas WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    sala = new Sala
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        TipoSala = reader.GetString(2),
                        Precio = reader.GetDecimal(3)
                    };
                }
            }

            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // Acción para mostrar el formulario de edición de sala
        public IActionResult Editar(int id)
        {
            Sala sala = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombre, TipoSala, Precio FROM Salas WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    sala = new Sala
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        TipoSala = reader.GetString(2),
                        Precio = reader.GetDecimal(3)
                    };
                }
            }

            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // Acción para procesar la edición de la sala
        [HttpPost]
        public IActionResult Editar(Sala sala)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Salas SET Nombre = @Nombre, TipoSala = @TipoSala, Precio = @Precio WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", sala.Id);
                    cmd.Parameters.AddWithValue("@Nombre", sala.Nombre);
                    cmd.Parameters.AddWithValue("@TipoSala", sala.TipoSala);
                    cmd.Parameters.AddWithValue("@Precio", sala.Precio);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(sala);
        }

        // Acción para eliminar una sala
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Salas WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
