﻿using InnoMasketss.Models;
using System.Data.SqlClient;
using System.Data;

namespace InnoMasketss.Data.Servicios
{
    public class UsuarioServicio
    {
        private readonly Contexto _contexto;

        public UsuarioServicio(Contexto con)
        {
            _contexto = con;
        }

        public void ActualizarToken(string correo)
        {
            using (SqlConnection con = new(_contexto.Conexion))
            {
                using (SqlCommand cmd = new("ActualizarToken", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    DateTime fecha = DateTime.UtcNow.AddMinutes(5);
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    var token = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("@Token", token.ToString());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    Email email = new();
                    if (correo != null)
                        email.Enviar(correo, token.ToString());

                }
            }
        }
        public List<Roles> ListarRoles()
        {
            var model = new List<Roles>();
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                using (SqlCommand cmd = new("ListRoles", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rol = new Roles();
                            rol.RolId = Convert.ToInt32(reader["RolId"]);
                            rol.Nombre = Convert.ToString(reader["Nombre"]);
                            model.Add(rol);
                        }
                    }
                }
            }
            return model;
        }

        public List<Usuario> ListarUsuarios()
        {
            var usuarios = new List<Usuario>();
            using (SqlConnection con = new(_contexto.Conexion))
            {
                using (SqlCommand cmd = new("ListarUsuarios", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var usuario = new Usuario
                        {
                            UsuariosId = (int)rdr["UsuariosId"],
                            Nombre = rdr["Nombre"].ToString(),
                            Apellido = rdr["Apellido"].ToString(),
                            Correo = rdr["Correo"].ToString(),
                            Contrasena = rdr["Contrasena"].ToString(),
                            RolId = (int)rdr["RolId"],
                            NombreUsuario = rdr["NombreUsuario"].ToString(),
                            Estado = Convert.ToBoolean(rdr["Estado"]),
                            Token = rdr["Token"].ToString(),
                            FechaExpiracion = Convert.ToDateTime(rdr["FechaExpiracion"])
                        };
                        usuarios.Add(usuario);

                    }
                }
            }
            return usuarios;
        }

        public Usuario ObtenerUsuarioId(int id)
        {
            Usuario usuario = new();

            using (SqlConnection con = new(_contexto.Conexion))
            {
                using (SqlCommand cmd = new("ObtenerUsuarioId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuariosId", id);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        usuario = new Usuario
                        {
                            UsuariosId = id,
                            Nombre = rdr["Nombre"].ToString(),
                            Apellido = rdr["Apellido"].ToString(),
                            Correo = rdr["Correo"].ToString(),
                            Contrasena = rdr["Contrasena"].ToString(),
                            RolId = (int)rdr["RolId"],
                            NombreUsuario = rdr["NombreUsuario"].ToString(),
                            Estado = Convert.ToBoolean(rdr["Estado"]),
                            Token = rdr["Token"].ToString(),
                            FechaExpiracion = Convert.ToDateTime(rdr["FechaExpiracion"]),

                        };
                    }
                }
            }
            return usuario;

        }
    }
}
