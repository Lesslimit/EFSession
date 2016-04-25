using EFSession.Schema.Parameters.Contracts;

namespace EFSession.Schema.Parameters
{
    public struct DefaultCriteria : ISchemaCriteria
    {
        public static DefaultCriteria Instance;

        public string Value => "dbo";

        static DefaultCriteria()
        {
            Instance = new DefaultCriteria();
        }
    }
}