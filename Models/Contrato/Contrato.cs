using System.ComponentModel.DataAnnotations;

namespace _Net.Models;

public class Contrato
{
    [Key]
    [Display(Name = "N° de Contrato")]
    public int IdContrato { get; set; }

    [Display(Name = "Inquilino")]
    public int IdInquilino { get; set; } 

    [Display(Name = "Inmueble")]
    public int IdInmueble { get; set; } 

    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    [Display(Name = "Fecha de finalización")]
    public DateTime FechaFin { get; set; }

    [Display(Name = "Valor mensual")]
    public double ValorMensual { get; set; }

    [Display(Name = "¿Vigente?")]
    public bool Vigente { get; set; } = true;

    public override string ToString()
    {
        var res = $"Inquilino ID: {IdInquilino}, Inmueble ID: {IdInmueble}, " +
                  $"Desde: {FechaInicio.ToShortDateString()} Hasta: {FechaFin.ToShortDateString()}, " +
                  $"Monto Mensual: {ValorMensual}, Activo: {Vigente}";
        return res;
    }
}
