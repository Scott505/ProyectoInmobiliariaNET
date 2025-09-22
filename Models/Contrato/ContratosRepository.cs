using System.Data;
using MySql.Data.MySqlClient;

namespace _Net.Models;

public class ContratosRepository : RepositoryBase
{
    public ContratosRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public Contrato? ObtenerPorId(int id)
    {
        Contrato? contrato = null;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Contrato.IdContrato)},
                                   {nameof(Contrato.IdInquilino)},
                                   {nameof(Contrato.IdInmueble)},
                                   {nameof(Contrato.FechaInicio)},
                                   {nameof(Contrato.FechaFin)},
                                   {nameof(Contrato.ValorMensual)},
                                   {nameof(Contrato.Vigente)}
                            FROM Contratos
                            WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        contrato = new Contrato
                        {
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                            IdInmueble = reader.GetInt32(nameof(Contrato.IdInmueble)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            ValorMensual = reader.GetDouble(nameof(Contrato.ValorMensual)),
                            Vigente = reader.GetBoolean(nameof(Contrato.Vigente))
                        };
                    }
                }
                connection.Close();
            }
        }
        return contrato;
    }

    public List<Contrato> ObtenerTodosOPorFiltros(int? dias = null, bool? vigente = null)
    {
        List<Contrato> contratos = new List<Contrato>();
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"SELECT {nameof(Contrato.IdContrato)},
                               {nameof(Contrato.IdInquilino)},
                               {nameof(Contrato.IdInmueble)},
                               {nameof(Contrato.FechaInicio)},
                               {nameof(Contrato.FechaFin)},
                               {nameof(Contrato.ValorMensual)},
                               {nameof(Contrato.Vigente)}
                        FROM Contratos
                        WHERE 1=1";

            if (dias.HasValue)
            {
                sql += " AND DATEDIFF(FechaFin, CURDATE()) <= @dias";
            }

            if (vigente.HasValue)
            {
                sql += " AND Vigente = @vigente";
            }

            using (var command = new MySqlCommand(sql, connection))
            {
                if (dias.HasValue)
                    command.Parameters.AddWithValue("@dias", dias.Value);

                if (vigente.HasValue)
                    command.Parameters.AddWithValue("@vigente", vigente.Value);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contratos.Add(new Contrato
                    {
                        IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                        IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                        IdInmueble = reader.GetInt32(nameof(Contrato.IdInmueble)),
                        FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                        FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                        ValorMensual = reader.GetDouble(nameof(Contrato.ValorMensual)),
                        Vigente = reader.GetBoolean(nameof(Contrato.Vigente))
                    });
                }
                connection.Close();
            }
        }
        return contratos;
    }

    public int Alta(Contrato contrato)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"INSERT INTO Contratos 
                            ({nameof(Contrato.IdInquilino)}, 
                             {nameof(Contrato.IdInmueble)}, 
                             {nameof(Contrato.FechaInicio)}, 
                             {nameof(Contrato.FechaFin)}, 
                             {nameof(Contrato.ValorMensual)}, 
                             {nameof(Contrato.Vigente)})
                            VALUES (@{nameof(Contrato.IdInquilino)}, 
                                    @{nameof(Contrato.IdInmueble)}, 
                                    @{nameof(Contrato.FechaInicio)}, 
                                    @{nameof(Contrato.FechaFin)}, 
                                    @{nameof(Contrato.ValorMensual)}, 
                                    @{nameof(Contrato.Vigente)});
                            SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInquilino)}", contrato.IdInquilino);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaFin)}", contrato.FechaFin);
                command.Parameters.AddWithValue($"@{nameof(Contrato.ValorMensual)}", contrato.ValorMensual);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Vigente)}", contrato.Vigente);

                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                contrato.IdContrato = res;
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Contrato contrato)
    {
        int res = -1;
        using (var connection = new MySqlConnection(ConectionString))
        {
            string sql = $@"UPDATE Contratos
                            SET {nameof(Contrato.IdInquilino)} = @{nameof(Contrato.IdInquilino)},
                                {nameof(Contrato.IdInmueble)} = @{nameof(Contrato.IdInmueble)},
                                {nameof(Contrato.FechaInicio)} = @{nameof(Contrato.FechaInicio)},
                                {nameof(Contrato.FechaFin)} = @{nameof(Contrato.FechaFin)},
                                {nameof(Contrato.ValorMensual)} = @{nameof(Contrato.ValorMensual)},
                                {nameof(Contrato.Vigente)} = @{nameof(Contrato.Vigente)}
                            WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInquilino)}", contrato.IdInquilino);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaFin)}", contrato.FechaFin);
                command.Parameters.AddWithValue($"@{nameof(Contrato.ValorMensual)}", contrato.ValorMensual);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Vigente)}", contrato.Vigente);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", contrato.IdContrato);

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
            string sql = $@"UPDATE Contratos
                            SET {nameof(Contrato.Vigente)} = false
                            WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }
}
