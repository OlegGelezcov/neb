namespace Space.Server
{

    using Photon.SocketServer;
    using Space.Game;
    using Space.Mmo.Server;
    /// <summary>
    ///   An interface for all mmo items and NPC items.
    /// </summary>
    public interface IMmoItem
    {
        #region Public Methods

        /// <summary>
        ///   Checks wheter the <paramref name = "actor" /> is allowed to change the item.
        ///   <see cref = "MmoActor" />s have write access to their own <see cref = "MmoItem" />s only.
        /// </summary>
        /// <param name = "actor">
        ///   The accessing actor.
        /// </param>
        /// <returns>
        ///   true of false
        /// </returns>
        bool GrantWriteAccess(MmoActor actor);

        /// <summary>
        ///   Receives an event.
        ///   Used for operation <see cref = "RaiseGenericEvent" />.
        /// </summary>
        /// <param name = "eventData">
        ///   The event data.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <returns>
        ///   False if the item cannot process the event, otherwise true.
        /// </returns>
        bool ReceiveEvent(EventData eventData, SendParameters sendParameters);


        #endregion
    }
}