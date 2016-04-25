using System.Data.SqlClient;

namespace EFSession.StoredProcedures
{
    public interface ISqlParametersManager
    {
        SqlParameter[] PrepareParameters(object parameters, params SqlParameter[] outputParameters);
    }
}