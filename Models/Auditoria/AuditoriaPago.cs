namespace _Net.Models
{
    public class AuditoriaPago
    {
        public int IdRegistro { get; set; }
        public int IdPago { get; set; }
        public int IdUsuarioCreador { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? IdUsuarioAnulador { get; set; }
        public DateTime? FechaAnulacion { get; set; }
    }
}
