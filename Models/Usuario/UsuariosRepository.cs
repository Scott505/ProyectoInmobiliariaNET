using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace _Net.Models
{
    public class UsuariosRepository
    {
        private readonly string ConectionString;

        public UsuariosRepository(IConfiguration config)
        {
            ConectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using var conn = new MySqlConnection(ConectionString);
            string sql = "SELECT idUsuario, email, password, nombre, rol, activo FROM usuarios;";
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new Usuario
                {
                    IdUsuario = r.GetInt32("idUsuario"),
                    Email = r.GetString("email"),
                    Password = r.GetString("password"),
                    Nombre = r.IsDBNull(r.GetOrdinal("nombre")) ? null : r.GetString("nombre"),
                    Rol = r.GetString("rol"),
                    Activo = r.GetBoolean("activo")
                });
            }
            conn.Close();
            return lista;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;
            using var conn = new MySqlConnection(ConectionString);
            string sql = "SELECT idUsuario, email, password, nombre, rol, activo FROM usuarios WHERE idUsuario=@Id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                u = new Usuario
                {
                    IdUsuario = r.GetInt32("idUsuario"),
                    Email = r.GetString("email"),
                    Password = r.GetString("password"),
                    Nombre = r.IsDBNull(r.GetOrdinal("nombre")) ? null : r.GetString("nombre"),
                    Rol = r.GetString("rol"),
                    Activo = r.GetBoolean("activo")
                };
            }
            conn.Close();
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using var conn = new MySqlConnection(ConectionString);
            string sql = "SELECT idUsuario, email, password, nombre, rol, activo FROM usuarios WHERE email=@Email AND activo=1;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                u = new Usuario
                {
                    IdUsuario = r.GetInt32("idUsuario"),
                    Email = r.GetString("email"),
                    Password = r.GetString("password"),
                    Nombre = r.IsDBNull(r.GetOrdinal("nombre")) ? null : r.GetString("nombre"),
                    Rol = r.GetString("rol"),
                    Activo = r.GetBoolean("activo")
                };
            }
            conn.Close();
            return u;
        }

        public int Alta(Usuario u)
        {
            int id = -1;
            using var conn = new MySqlConnection(ConectionString);
            string sql = @"INSERT INTO usuarios (email, password, nombre, rol, activo)
                           VALUES (@Email, @Password, @Nombre, @Rol, @Activo);
                           SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Nombre", (object?)u.Nombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@Activo", u.Activo);
            conn.Open();
            id = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            return id;
        }

        public int Modificar(Usuario u)
        {
            int res = -1;
            using var conn = new MySqlConnection(ConectionString);
            string sql = @"UPDATE usuarios SET email=@Email, password=@Password, nombre=@Nombre, rol=@Rol, activo=@Activo
                           WHERE idUsuario=@IdUsuario;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Nombre", (object?)u.Nombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@Activo", u.Activo);
            cmd.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
            conn.Open();
            res = cmd.ExecuteNonQuery();
            conn.Close();
            return res;
        }

        public int BajaLogica(int id)
        {
            int res = -1;
            using var conn = new MySqlConnection(ConectionString);
            string sql = "UPDATE usuarios SET activo=0 WHERE idUsuario=@Id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            res = cmd.ExecuteNonQuery();
            conn.Close();
            return res;
        }
    }
}
