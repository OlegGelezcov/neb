using Common;
using Space.Server;

namespace Space.Game
{
    public static class Extensions
    {


        public static bool CheckItem<T>(this Item item) where T : class
        {
            if (item == null)
                return false;
            if (item.Disposed)
                return false;
            return (item is T);
        }

        public static bool CheckActor<T>(this Actor actor) where T : class
        {
            if (actor == null)
                return false;
            if (actor.Avatar == null)
                return false;
            if (actor.Avatar.Disposed)
                return false;
            return (actor is T);
        }

        public static bool CheckMmoActor<T>(this MmoActor actor) where T : class
        {
            if (actor == null)
                return false;
            if (actor.Avatar == null)
                return false;
            if (actor.Avatar.Disposed)
                return false;
            return (actor is T);
        }
    }
}
