using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cinema.Models;
using Microsoft.Extensions.Configuration;

namespace Cinema.Controllers
{
    public class MenuController : Controller
    {
        private readonly string _connectionString;

        public MenuController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Acción para mostrar la lista de artículos del menú
        public IActionResult Index()
        {
            List<Menu> menus = new List<Menu>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Articulo, Descripcion, Precio, Foto FROM Menu";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Menu menu = new Menu
                    {
                        Id = reader.GetInt32(0),
                        Articulo = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Foto = reader.IsDBNull(4) ? null : (byte[])reader["Foto"]
                    };

                    menus.Add(menu);
                }
            }

            return View(menus);
        }

        // Acción para mostrar el formulario de agregar artículo al menú
        public IActionResult Crear()
        {
            return View();
        }

        // Acción para procesar el formulario de agregar artículo al menú
        [HttpPost]
        public IActionResult Crear(Menu menu, IFormFile fotoFile)
        {
            if (fotoFile != null && fotoFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    fotoFile.CopyTo(ms);
                    menu.Foto = ms.ToArray();
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Menu (Articulo, Descripcion, Precio, Foto) VALUES (@Articulo, @Descripcion, @Precio, @Foto)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Articulo", menu.Articulo);
                cmd.Parameters.AddWithValue("@Descripcion", menu.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", menu.Precio);
                cmd.Parameters.AddWithValue("@Foto", menu.Foto ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Acción para mostrar detalles de un artículo del menú
        public IActionResult Detalles(int id)
        {
            Menu menu = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Articulo, Descripcion, Precio, Foto FROM Menu WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    menu = new Menu
                    {
                        Id = reader.GetInt32(0),
                        Articulo = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Foto = reader.IsDBNull(4) ? null : (byte[])reader["Foto"]
                    };
                }
            }

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // Acción para mostrar el formulario de edición de un artículo del menú
        public IActionResult Editar(int id)
        {
            Menu menu = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Articulo, Descripcion, Precio, Foto FROM Menu WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    menu = new Menu
                    {
                        Id = reader.GetInt32(0),
                        Articulo = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Foto = reader.IsDBNull(4) ? null : (byte[])reader["Foto"]
                    };
                }
            }

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // Acción para procesar la edición del artículo del menú
        [HttpPost]
        public IActionResult Editar(Menu menu, IFormFile fotoFile)
        {
            if (fotoFile != null && fotoFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    fotoFile.CopyTo(ms);
                    menu.Foto = ms.ToArray();
                }
            }
            else
            {
                // Si no se subió una nueva foto, mantener la foto actual
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Foto FROM Menu WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", menu.Id);

                    conn.Open();
                    var currentFoto = cmd.ExecuteScalar();
                    menu.Foto = currentFoto as byte[];
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Menu SET Articulo = @Articulo, Descripcion = @Descripcion, Precio = @Precio, Foto = @Foto WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", menu.Id);
                cmd.Parameters.AddWithValue("@Articulo", menu.Articulo);
                cmd.Parameters.AddWithValue("@Descripcion", menu.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", menu.Precio);
                cmd.Parameters.AddWithValue("@Foto", menu.Foto ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Acción para eliminar un artículo del menú
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Menu WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
