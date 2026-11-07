using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ControlMastologia.Web.Models
{
    [Table("pacientes")] // Nombre exacto de la tabla en Supabase
    public class PacienteNube : BaseModel
    {
        [PrimaryKey("id")] // Coincide con la columna "id"
        public int Id { get; set; }

        [Column("Cedula")]
        public long Cedula { get; set; } // Supabase dice tipo numérico

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("Correo Electronic")] // Ojo con el espacio si así está en Supabase
        public string CorreoElectronico { get; set; } = string.Empty;

        [Column("Enfermedad Actual")]
        public string EnfermedadActual { get; set; } = string.Empty;

        [Column("Antecedentes Personales")]
        public string AntecedentesPersonales { get; set; } = string.Empty;

        // Ajusta las demás columnas siguiendo este formato de nombre exacto
    }
}