namespace EFSession.StoredProcedures
{
    public partial class StoredProceduresContainer
    {
        public static StoredProceduresContainer Instance { get; }

        static StoredProceduresContainer()
        {
            Instance = new StoredProceduresContainer();
        }
    }
}