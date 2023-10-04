using System.ComponentModel.DataAnnotations;

namespace MasterDetailCRUD.Validations
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] _tipoValidosPermitidos;

        public TipoArchivoValidacion(string[] tipoValidosPermitidos)
        {
            this._tipoValidosPermitidos = tipoValidosPermitidos;
        }
    }
}
