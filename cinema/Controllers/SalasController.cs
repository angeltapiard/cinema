using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Cinema.Models;
using Microsoft.Extensions.Configuration;

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
                    string query = "INSERT INTO Salas (Nombre, TipoSala, Precio) OUTPUT INSERTED.ID VALUES (@Nombre, @TipoSala, @Precio)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nombre", sala.Nombre);
                    cmd.Parameters.AddWithValue("@TipoSala", sala.TipoSala);
                    cmd.Parameters.AddWithValue("@Precio", sala.Precio);

                    conn.Open();
                    int salaId = (int)cmd.ExecuteScalar(); // Obtener el ID de la sala recién creada

                    // Crear 150 asientos para la sala
                    CrearAsientosParaSala(salaId);
                }

                return RedirectToAction("Index");
            }

            return View(sala);
        }

        // Método para crear 150 asientos para una sala (10 filas x 15 columnas)
        private void CrearAsientosParaSala(int salaId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Asientos (Fila, Columna, Ocupado, SalaId) VALUES (@Fila, @Columna, 0, @SalaId)";

                for (char fila = 'A'; fila <= 'J'; fila++) // 10 filas (A-J)
                {
                    for (int columna = 1; columna <= 15; columna++) // 15 columnas por fila
                    {
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Fila", fila);
                        cmd.Parameters.AddWithValue("@Columna", columna);
                        cmd.Parameters.AddWithValue("@SalaId", salaId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // Acción para mostrar el formulario de edición de sala (GET)
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

        // Acción para procesar el formulario de edición de sala (POST)
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

        // Acción para mostrar detalles de una sala
        public IActionResult Detalles(int id, int? asientoId)
        {
            Sala sala = null;
            List<Asiento> asientos = new List<Asiento>();
            Asiento asientoSeleccionado = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Obtener detalles de la sala
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

                reader.Close(); // Cerrar el reader para reutilizar la conexión

                // Obtener los asientos de la sala
                string asientosQuery = "SELECT Id, Fila, Columna, Ocupado FROM Asientos WHERE SalaId = @SalaId";
                SqlCommand asientosCmd = new SqlCommand(asientosQuery, conn);
                asientosCmd.Parameters.AddWithValue("@SalaId", id);
                SqlDataReader asientosReader = asientosCmd.ExecuteReader();

                while (asientosReader.Read())
                {
                    Asiento asiento = new Asiento
                    {
                        Id = asientosReader.GetInt32(0),
                        Fila = asientosReader.GetString(1)[0], // Obtener el primer carácter de la fila
                        Columna = asientosReader.GetInt32(2),
                        Ocupado = asientosReader.GetBoolean(3)
                    };
                    asientos.Add(asiento);
                }
            }

            if (sala == null)
            {
                return NotFound();
            }

            // Obtener el asiento seleccionado si existe
            if (asientoId.HasValue)
            {
                asientoSeleccionado = asientos.FirstOrDefault(a => a.Id == asientoId.Value);
            }

            var model = new DetallesViewModel
            {
                Sala = sala,
                Asientos = asientos,
                AsientoSeleccionado = asientoSeleccionado
            };

            return View(model);
        }

        // Acción para seleccionar o cambiar el estado de un asiento
        [HttpPost]
        public IActionResult SeleccionarAsiento(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Primero obtenemos el estado actual del asiento
                string checkQuery = "SELECT Ocupado FROM Asientos WHERE Id = @Id";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
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
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Id", id);
                updateCmd.Parameters.AddWithValue("@Ocupado", !ocupado); // Cambiar el estado
                updateCmd.ExecuteNonQuery();
            }

            // Retornar una respuesta JSON para notificar el cambio
            return Json(new { success = true });
        }

        // Acción para eliminar una sala
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Eliminar asientos asociados
                string deleteAsientosQuery = "DELETE FROM Asientos WHERE SalaId = @SalaId";
                SqlCommand deleteAsientosCmd = new SqlCommand(deleteAsientosQuery, conn);
                deleteAsientosCmd.Parameters.AddWithValue("@SalaId", id);

                // Eliminar la sala
                string deleteSalaQuery = "DELETE FROM Salas WHERE Id = @Id";
                SqlCommand deleteSalaCmd = new SqlCommand(deleteSalaQuery, conn);
                deleteSalaCmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                deleteAsientosCmd.ExecuteNonQuery();
                deleteSalaCmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
