using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
     public class Pokemon
    {
        //Anotations
        public int Id { get; set; }

        [DisplayName("Número")] //Anotation que sirve para modificar el nombre de la columna por el que vos quieras.
        public int Numero { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Descripción")] //Anotation que sirve para modificar el nombre de la columna por el que vos quieras.
        public string Descripcion { get; set; }

        [DisplayName("Url Imágen")] //Anotation que sirve para modificar el nombre de la columna por el que vos quieras.
        public string UrlImagen { get; set; }

        public Elemento Tipo { get; set; }

        public Elemento Debilidad { get; set; }

        public int Activo { get; set; }
    }
}
