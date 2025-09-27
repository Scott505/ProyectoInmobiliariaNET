namespace _Net.Models
{
    public class AuditoriaContrato
    {
        public int IdRegistro { get; set; }
        public int IdContrato { get; set; }

        public int IdUsuarioCreador { get; set; }
        public DateTime FechaCreacion { get; set; }

        public int? IdUsuarioFinalizador { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
    }
}
