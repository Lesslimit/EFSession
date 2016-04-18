namespace EFSession
{
    public interface ICanPreserveConnection
    {
        bool ConnectionIsPreserved { get; }

        void MarkConnectionAsPreserved();

        void ReleasePreservedConnection();
    }
}