using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using Cinema.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Cinema.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly string _connectionString;

        public PeliculasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Acción para mostrar la lista de películas
        public IActionResult Index()
        {
            List<Pelicula> peliculas = new List<Pelicula>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombre, Duracion, Clasificacion, Poster FROM Peliculas";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Pelicula pelicula = new Pelicula
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Duracion = reader.GetInt32(2),
                        Clasificacion = reader.IsDBNull(3) ? Clasificacion.G : ParseClasificacion(reader.GetString(3)),
                        Poster = reader.IsDBNull(4) ? null : (byte[])reader["Poster"]
                    };

                    peliculas.Add(pelicula);
                }
            }

            return View(peliculas);
        }

        // Método auxiliar para convertir el string a enum Clasificacion de manera segura
        private Clasificacion ParseClasificacion(string clasificacionStr)
        {
            if (Enum.TryParse(clasificacionStr, out Clasificacion clasificacion))
            {
                return clasificacion;
            }
            return Clasificacion.G; // Valor por defecto si falla la conversión
        }

        // Acción para mostrar el formulario de agregar película
        public IActionResult Crear()
        {
            return View();
        }

        // Acción para procesar el formulario de agregar película
        [HttpPost]
        public IActionResult Crear(Pelicula pelicula, IFormFile posterFile)
        {
            if (posterFile != null && posterFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    posterFile.CopyTo(ms);
                    pelicula.Poster = ms.ToArray();
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Peliculas (Nombre, Duracion, Clasificacion, Descripcion, Poster) VALUES (@Nombre, @Duracion, @Clasificacion, @Descripcion, @Poster)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", pelicula.Nombre);
                cmd.Parameters.AddWithValue("@Duracion", pelicula.Duracion);
                cmd.Parameters.AddWithValue("@Clasificacion", pelicula.Clasificacion.ToString());
                cmd.Parameters.AddWithValue("@Descripcion", pelicula.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Poster", pelicula.Poster ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Acción para mostrar detalles de una película
        public IActionResult Detalles(int id)
        {
            Pelicula pelicula = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombre, Duracion, Clasificacion, Descripcion, Poster FROM Peliculas WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    pelicula = new Pelicula
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Duracion = reader.GetInt32(2),
                        Clasificacion = reader.IsDBNull(3) ? Clasificacion.G : ParseClasificacion(reader.GetString(3)),
                        Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Poster = reader.IsDBNull(5) ? null : (byte[])reader["Poster"]
                    };
                }
            }

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // Acción para mostrar el formulario de edición de película
        public IActionResult Editar(int id)
        {
            Pelicula pelicula = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Nombre, Duracion, Clasificacion, Descripcion, Poster FROM Peliculas WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    pelicula = new Pelicula
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Duracion = reader.GetInt32(2),
                        Clasificacion = reader.IsDBNull(3) ? Clasificacion.G : ParseClasificacion(reader.GetString(3)),
                        Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Poster = reader.IsDBNull(5) ? null : (byte[])reader["Poster"]
                    };
                }
            }

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // Acción para procesar la edición de la película
        [HttpPost]
        public IActionResult Editar(Pelicula pelicula, IFormFile posterFile)
        {
            if (posterFile != null && posterFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    posterFile.CopyTo(ms);
                    pelicula.Poster = ms.ToArray();
                }
            }
            else
            {
                // Si no se subió un nuevo póster, mantener el póster actual
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Poster FROM Peliculas WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", pelicula.Id);

                    conn.Open();
                    var currentPoster = cmd.ExecuteScalar();
                    pelicula.Poster = currentPoster as byte[];
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Peliculas SET Nombre = @Nombre, Duracion = @Duracion, Clasificacion = @Clasificacion, Descripcion = @Descripcion, Poster = @Poster WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", pelicula.Id);
                cmd.Parameters.AddWithValue("@Nombre", pelicula.Nombre);
                cmd.Parameters.AddWithValue("@Duracion", pelicula.Duracion);
                cmd.Parameters.AddWithValue("@Clasificacion", pelicula.Clasificacion.ToString());
                cmd.Parameters.AddWithValue("@Descripcion", pelicula.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Poster", pelicula.Poster ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Acción para eliminar una película
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Peliculas WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}