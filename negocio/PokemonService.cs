using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;


namespace negocio
{
    public class PokemonService
    {

        public List<Pokemon> Listar()
        {
            List<Pokemon> lista = new List<Pokemon>();

            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector; //inicializo el lector

            try
            {
                conexion.ConnectionString = "server=.; database=POKEDEX_DB; integrated security=true"; //string de conexion  Si le pongo . o (local) me lo toma.
                comando.CommandType = System.Data.CommandType.Text; //establece el tipo de comando
                comando.CommandText = "select numero, nombre, P.descripcion, urlImagen, E.Descripcion as Tipo,  D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id, P.Activo FROM POKEMONS P, ELEMENTOS E, ELEMENTOS D WHERE E.Id = P.IdTipo and D.Id = P.IdDebilidad";
                comando.Connection = conexion; //el comando lo ejecuta en la conexión que establecí en linea 23.


                conexion.Open();
                lector = comando.ExecuteReader(); //le asigno al lector la consulta ejecutada.

                while (lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)lector["Id"];
                    aux.Numero = lector.GetInt32(0);
                    aux.Nombre = ((string)lector["Nombre"]).Trim(); //conversión explicita de un objeto a un string. .Trim elimina los espacios en blanco innecesarios
                    aux.Descripcion = (string)lector["Descripcion"];
                    aux.Activo = (int)lector["Activo"];


                    //if (! (lector.IsDBNull(lector.GetOrdinal("UrlImagen"))) )  ////  VALIDAR NULOS. valida si lo que tiene el lector, en la columna url imagen es distinto de nulo
                    //                                                           //  Lo guarda. Sino no
                    //{
                    //    aux.UrlImagen = (string)lector["UrlImagen"]; 
                    //}

                    if (!(lector["UrlImagen"] is DBNull))                        //// VALIDAR NULOS. opcion 2. Solo para columnas que admitan nulos. Para casos de consultas JOIN
                    {
                        aux.UrlImagen = (string)lector["UrlImagen"];
                    }
                   



                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];

                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    if (aux.Activo == 1)    // OTRA ALTERNATIVA MAS EFICIENTE PARA NO HACER TODO ESTO ES MODIFICAR LA CONSULTA SELECT Y AGREGARLE OTRO WHERE
                                            // QUE SEA AND P.Activo = 1. ASI EVITAMOS TENER QUE HACER LA LECTURA DEL ATRIBUTO ACTIVO Y HACER LA VALIDACIÓN PARA LISTAR.
                    {
                        lista.Add(aux);
                    }
                    
                }



                return lista;
            }

            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conexion.Close(); //tanto si tiene exito, como si falla, cierro la conexión.
            }





        }

        public void agregarPokemon(Pokemon nuevo)
        { 
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta($"INSERT INTO POKEMONS (Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen) VALUES (" +nuevo.Numero+ ", '"+nuevo.Nombre + "', '"+nuevo.Descripcion+ "', 1, @IdTipo, @IdDebilidad, @UrlImagen)");
                //datos.setearConsulta($"INSERT INTO POKEMONS (Numero, Nombre, Descripcion, Activo) VALUES ({nuevo.Numero}, '{nuevo.Nombre}', '{nuevo.Descripcion}', 1)");
                datos.setearParametro("@IdTipo",nuevo.Tipo.Id);
                datos.setearParametro("@IdDebilidad", nuevo.Debilidad.Id);
                datos.setearParametro("@UrlImagen", nuevo.UrlImagen);



                datos.ejecutarAccion();


            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

        }

        public void modificarPokemon(Pokemon poke)
        {

            AccesoDatos datos = new AccesoDatos();
            try
            {
              datos.setearConsulta("UPDATE POKEMONS SET Numero = @Numero, Nombre = @Nombre, Descripcion = @Desc, UrlImagen = @img, IdTipo = @IdTipo, IdDebilidad = @IdDebilidad WHERE Id = @Id");
                datos.setearParametro("@Numero", poke.Numero);
                datos.setearParametro("@Nombre", poke.Nombre);
                datos.setearParametro("@Desc", poke.Descripcion);
                datos.setearParametro("@img", poke.UrlImagen);
                datos.setearParametro("@IdTipo", poke.Tipo.Id);
                datos.setearParametro("@IdDebilidad", poke.Debilidad.Id);
                datos.setearParametro("@Id", poke.Id);

                datos.ejecutarAccion();

            }
            catch (Exception ex )
            {

                throw ex;
            }

            finally
            {
                datos.cerrarConexion();
            }

        }
        public void eliminarPokemon(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                
                datos.setearConsulta("DELETE FROM POKEMONS WHERE ID=@id");
                datos.setearParametro("id", id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminarLogico(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE POKEMONS SET Activo = 0 WHERE Id = @id");
                datos.setearParametro("id", id);

                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "select numero, nombre, P.descripcion, urlImagen, E.Descripcion as Tipo,  D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id, P.Activo FROM POKEMONS P, ELEMENTOS E, ELEMENTOS D WHERE E.Id = P.IdTipo and D.Id = P.IdDebilidad and Activo = 1 and ";
               if(campo == "Número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Numero < " + filtro;
                            break;

                        default:
                            consulta += "Numero = " + filtro;
                            break;
                    }
                }
                else if(campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;

                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                } else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "P.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "P.Descripcion like '%" + filtro + "'";
                            break;

                        default:
                            consulta += "P.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = datos.Lector.GetInt32(0);
                    aux.Nombre = ((string)datos.Lector["Nombre"]).Trim(); //conversión explicita de un objeto a un string. .Trim elimina los espacios en blanco innecesarios
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Activo = (int)datos.Lector["Activo"];


                    //if (! (lector.IsDBNull(lector.GetOrdinal("UrlImagen"))) )  ////  VALIDAR NULOS. valida si lo que tiene el lector, en la columna url imagen es distinto de nulo
                    //                                                           //  Lo guarda. Sino no
                    //{
                    //    aux.UrlImagen = (string)lector["UrlImagen"]; 
                    //}

                    if (!(datos.Lector["UrlImagen"] is DBNull))                        //// VALIDAR NULOS. opcion 2. Solo para columnas que admitan nulos. Para casos de consultas JOIN
                    {
                        aux.UrlImagen = (string)datos.Lector["UrlImagen"];
                    }




                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];

                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];

                    if (aux.Activo == 1)    // OTRA ALTERNATIVA MAS EFICIENTE PARA NO HACER TODO ESTO ES MODIFICAR LA CONSULTA SELECT Y AGREGARLE OTRO WHERE
                                            // QUE SEA AND P.Activo = 1. ASI EVITAMOS TENER QUE HACER LA LECTURA DEL ATRIBUTO ACTIVO Y HACER LA VALIDACIÓN PARA LISTAR.
                    {
                        lista.Add(aux);
                    }

                }


                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
