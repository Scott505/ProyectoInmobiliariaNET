using System.Data;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class InmueblesRepository : RepositoryBase
{
    public InmueblesRepository(IConfiguration configuration) : base(configuration) { }

    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? inmueble = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Inmueble.IdInmueble)},
                                   {nameof(Inmueble.Direccion)},
                                   {nameof(Inmueble.Tipo)},
                                   {nameof(Inmueble.Uso)},
                                   {nameof(Inmueble.Ambientes)},
                                   {nameof(Inmueble.Latitud)},
                                   {nameof(Inmueble.Longitud)},
                                   {nameof(Inmueble.IdPropietario)},
                                   {nameof(Inmueble.Disponible)}
                            FROM Inmuebles
                            WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Uso = reader.GetString(nameof(Inmueble.Uso)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible))
                        };
                    }
                }
                connection.Close();
            }
        }
        return inmueble;
    }

    public List<Inmueble> ObtenerTodos()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Inmueble.IdInmueble)},
                                   {nameof(Inmueble.Direccion)},
                                   {nameof(Inmueble.Tipo)},
                                   {nameof(Inmueble.Uso)},
                                   {nameof(Inmueble.Ambientes)},
                                   {nameof(Inmueble.Latitud)},
                                   {nameof(Inmueble.Longitud)},
                                   {nameof(Inmueble.IdPropietario)},
                                   {nameof(Inmueble.Disponible)}
                            FROM Inmuebles;";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Uso = reader.GetString(nameof(Inmueble.Uso)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible))
                        });
                    }
                }
                connection.Close();
            }
        }
        return inmuebles;
    }

    public int Alta(Inmueble inmueble)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"INSERT INTO Inmuebles 
                            ({nameof(Inmueble.Direccion)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Uso)}, 
                             {nameof(Inmueble.Ambientes)}, {nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, 
                             {nameof(Inmueble.IdPropietario)}, {nameof(Inmueble.Disponible)})
                            VALUES (@{nameof(Inmueble.Direccion)}, @{nameof(Inmueble.Tipo)}, @{nameof(Inmueble.Uso)},
                                    @{nameof(Inmueble.Ambientes)}, @{nameof(Inmueble.Latitud)}, @{nameof(Inmueble.Longitud)},
                                    @{nameof(Inmueble.IdPropietario)}, @{nameof(Inmueble.Disponible)});
                            SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Tipo)}", inmueble.Tipo ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Uso)}", inmueble.Uso ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Disponible)}", inmueble.Disponible);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                inmueble.IdInmueble = res;
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Inmueble inmueble)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"UPDATE Inmuebles
                            SET {nameof(Inmueble.Direccion)} = @{nameof(Inmueble.Direccion)},
                                {nameof(Inmueble.Tipo)} = @{nameof(Inmueble.Tipo)},
                                {nameof(Inmueble.Uso)} = @{nameof(Inmueble.Uso)},
                                {nameof(Inmueble.Ambientes)} = @{nameof(Inmueble.Ambientes)},
                                {nameof(Inmueble.Latitud)} = @{nameof(Inmueble.Latitud)},
                                {nameof(Inmueble.Longitud)} = @{nameof(Inmueble.Longitud)},
                                {nameof(Inmueble.IdPropietario)} = @{nameof(Inmueble.IdPropietario)},
                                {nameof(Inmueble.Disponible)} = @{nameof(Inmueble.Disponible)}
                            WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Tipo)}", inmueble.Tipo ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Uso)}", inmueble.Uso ?? "");
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Disponible)}", inmueble.Disponible);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", inmueble.IdInmueble);

                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"UPDATE Inmuebles
            SET {nameof(Inmueble.Disponible)} = false
            WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }


    public List<Inmueble> ObtenerTodosOPorFiltro(int? idPropietario = null, bool? disponible = null)
    {
        var lista = new List<Inmueble>();

        using (var connection = new MySqlConnection(ConectionString))
        {
            var sql = "SELECT * FROM Inmuebles WHERE 1=1";

            if (idPropietario.HasValue)
                sql += " AND IdPropietario = @idPropietario";

            if (disponible.HasValue)
                sql += " AND Disponible = @disponible";

            using (var command = new MySqlCommand(sql, connection))
            {
                if (idPropietario.HasValue)
                    command.Parameters.AddWithValue("@idPropietario", idPropietario.Value);

                if (disponible.HasValue)
                    command.Parameters.AddWithValue("@disponible", disponible.Value);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            Direccion = reader.GetString("Direccion"),
                            Tipo = reader.GetString("Tipo"),
                            Uso = reader.GetString("Uso"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Latitud = reader.GetDecimal("Latitud"),
                            Longitud = reader.GetDecimal("Longitud"),
                            IdPropietario = reader.GetInt32("IdPropietario"),
                            Disponible = reader.GetBoolean("Disponible")
                        };
                        lista.Add(inmueble);
                    }
                }
                connection.Close();
            }
        }

        return lista;
    }
}

