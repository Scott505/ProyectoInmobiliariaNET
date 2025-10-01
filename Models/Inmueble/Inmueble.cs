using System.ComponentModel.DataAnnotations;

namespace _Net.Models;

public class Inmueble
{
    [Key]
    [Display(Name = "N° Inmueble")]
    public int IdInmueble { get; set; }

    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }

    [Display(Name = "Uso")]
    public string? Uso { get; set; }

    [Display(Name = "Cantidad de Ambientes")]
    public int Ambientes { get; set; }

    [Display(Name = "Latitud")]
    public decimal Latitud { get; set; }

    [Display(Name = "Longitud")]
    public decimal Longitud { get; set; }

    [Display(Name = "Propietario")]
    public int IdPropietario { get; set; }

    [Display(Name = "Disponible")]
    public bool Disponible { get; set; } = true;

    public string? PropietarioNombre { get; set; }
    public string? PropietarioApellido { get; set; }
    public string PropietarioNombreCompleto => $"{PropietarioNombre} {PropietarioApellido}";


    public override string ToString()
    {
        var res = $"Dirección: {Direccion}, Uso: {Uso}, Tipo: {Tipo}, Ambientes: {Ambientes}";
        return res;
    }
}

