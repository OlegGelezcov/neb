namespace Space.Game
{
    using Space.Server;

    public class MmoItemCache : ItemCache
    {
        public MmoItemCache() : base(Settings.MaxLockWaitTimeMilliseconds)
        {

        }
    }
}
