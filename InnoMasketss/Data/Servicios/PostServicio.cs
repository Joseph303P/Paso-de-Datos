﻿using InnoMasketss.Data.Enums;
using InnoMasketss.Models;
using System.Data.SqlClient;
using System.Data;

namespace InnoMasketss.Data.Servicios
{
    public class PostServicio
    {
        private readonly Contexto _contexto;

        public PostServicio(Contexto con)
        {
            _contexto = con;
        }

        //Controlador para obtener pos mediantes un identificador 
        public Post ObtenerPostPorId(int id)
        {
            var post = new Post();
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("ObtenerPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"]
                            };

                        }

                        reader.Close();
                    }
                }

                return post;
            }

        }

        //Constructor para Obtener todos los post en general
        public List<Post> ObtenerPosts()
        {
            var posts = new List<Post>();
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                using (SqlCommand cmd = new("ObtenerPost", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"]
                            };

                            posts.Add(post);
                        }

                    }
                }
            }

            return posts;
        }

        //Obtiene los post por categoria
        public List<Post> ObtenerPostsCategoria(CategoriaEnum categoria)
        {
            var posts = new List<Post>();
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                using (SqlCommand cmd = new("ObtenerPostCategoria", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Categoria", categoria.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"]
                            };

                            posts.Add(post);
                        }

                    }
                }
            }

            return posts;
        }

        //Obtener post por su titulo
        public List<Post> ObtenerPostsTitulo(string titulo)
        {
            var posts = new List<Post>();
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                using (SqlCommand cmd = new("ObtenerPostTitulo", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Titulo", titulo);
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            posts.Add(new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"]),
                                FechaCreacion = (DateTime)reader["FechaCreacion"]
                            });

                        }

                    }
                }
            }

            return posts;
        }

        //Obtiene comentarios que se encuentra en un post por medio de un identificador
        public List<Comentario> ObtenerComentariosPorPostId(int id)
        {
            var Comments = new List<Comentario>();
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("ObtenerComentariosPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Comment = new Comentario
                            {
                                ComentarioId = (int)reader["ComentarioId"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                UsuariosId = (int)reader["UsuariosId"],
                                PostId = (int)reader["PostId"],
                                NombreUsuario = (string)reader["NombreUsuario"]
                            };
                            Comments.Add(Comment);
                        }
                        reader.Close();
                    }
                }
            }
            return Comments;
        }

        //Se encarga de recuperar comentarios secundarios asociados a los comentarios principales 
        public List<Comentario> ObtenerComentariosHijos(List<Comentario> comments)
        {
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    using (var command = new SqlCommand("ObtenerComentarioSecundarioComentarioId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ComentarioId", comment.ComentarioId);
                        using (var reader = command.ExecuteReader())
                        {
                            var comentariosecundarios = new List<Comentario>();
                            while (reader.Read())
                            {
                                var comentariosecundario = new Comentario
                                {
                                    ComentarioId = (int)reader["ComentarioId"],
                                    Contenido = (string)reader["Contenido"],
                                    FechaCreacion = (DateTime)reader["FechaCreacion"],
                                    UsuariosId = (int)reader["UsuariosId"],
                                    PostId = (int)reader["PostId"],
                                    NombreUsuario = (string)reader["NombreUsuario"],
                                    ComentarioPrincipalId = comment.ComentarioId
                                };
                                comentariosecundarios.Add(comentariosecundario);
                            }
                            reader.Close();
                            comment.ComentarioSecundarios = comentariosecundarios;
                        }
                    }
                }

            }

            return comments;
        }

        public List<Comentario> ObtenerComentariosNietos(List<Comentario> comments)
        {
            using (var connection = new SqlConnection(_contexto.Conexion))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    if (comment.ComentarioSecundarios is not null)
                    {
                        foreach (var comentariosecundario in comment.ComentarioSecundarios)
                        {
                            using (var command = new SqlCommand("ObtenerComentarioSecundarioComentarioId", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@ComentarioId", comentariosecundario.ComentarioId);
                                using (var reader = command.ExecuteReader())
                                {
                                    var comentarionietos = new List<Comentario>();
                                    while (reader.Read())
                                    {
                                        var comentarionieto = new Comentario
                                        {
                                            ComentarioId = (int)reader["ComentarioId"],
                                            Contenido = (string)reader["Contenido"],
                                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                                            UsuariosId = (int)reader["UsuariosId"],
                                            PostId = (int)reader["PostId"],
                                            NombreUsuario = (string)reader["NombreUsuario"],
                                            ComentarioPrincipalId = comentariosecundario.ComentarioId,
                                            ComentarioTerciariosId = comment.ComentarioId
                                        };
                                        comentarionietos.Add(comentarionieto);
                                    }
                                    reader.Close();
                                    comentariosecundario.ComentarioSecundarios = comentarionietos;
                                }
                            }
                        }
                    }

                }

            }

            return comments;
        }
    }
}
