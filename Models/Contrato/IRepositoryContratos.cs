namespace _Net.Models

{
	public interface IRepositoryContratos: IRepositorio<Contrato>
	{
        IList<Contrato> ObtenerTodosOPorFiltros(int? dias = null, bool? vigente = null, int? idInquilino = null);
        bool ExisteSuperposicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoExcluir = null);
    }
}