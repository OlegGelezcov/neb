// ComponentMessages.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:58:00 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Engine {
    public static class ComponentMessages {

        //sended by enemy to damagers when enemy death occured
        public const string OnEnemyDeath = "OnEnemyDeath"; //param - NebulaObject of enemy which death 

        //sended to self components when go to in combat state
        public const string InCombat = "InCombat";

        //Base weapon send to self when object make critical hit
        public const string OnCriticalHit = "OnCriticalHit";

        //Sended to target when current object take to target this object
        public const string OnTargetUnsubscribeMe = "OnTargetUnsubscribeMe";

        public const string OnTargetSubscribeMe = "OnTargetSubscribeMe";

        //OnNewDamage sended when object receive damage (paramter - damager)
        public const string OnNewDamage = "OnNewDamage";

        //Sended by combat bot to self when return to start position state changed (parameter bool start or end return)
        public const string OnReturnStateChanged = "OnReturnStateChanged";

        //Sended to owned event when evented object death occured
        public const string OnDeath = "OnDeath";

        //Sended to owned event when evented object receive damage
        public const string OnReceiveDamage = "OnReceiveDamage";

        //Sended to player when group removed event receive from group service
        public const string OnGroupRemoved = "OnGroupRemoved";

        //Sended to self when GameLogicDeathOccured
        public const string Death = "Death";

        //send self when Weapon.Heal called
        public const string OnMakeHeal = "OnMakeHeal";

        //send self when critical heal occured
        public const string OnCriticalHeal = "OnCriticalHeal";

        //send self when invisibility changed
        public const string OnInvisibilityChanged = "OnInvisibilityChanged";

        //send self when make shot
        public const string OnMakeFire = "OnMakeFire";

        public const string OnWasKilled = "OnWasKilled";

        /// <summary>
        /// Sended to self when entered to station
        /// </summary>
        public const string OnEnterStation = "OnEnterStation";

        public const string OnStationExited = "OnStationExited";

        /// <summary>
        /// OnStartAttack message sended to damager from receiver (parameter is receiver nebula object)
        /// </summary>
        public const string OnStartAttack = "OnStartAttack";

        /// <summary>
        /// Player send to yourself when he craft module from scheme
        /// </summary>
        public const string OnModuleCrafted = "OnModuleCrafted";

        /// <summary>
        /// Sended to self when asteroid collected (paarameter is List[AsteroidContent])
        /// </summary>
        public const string OnAsteroidCollected = "OnAsteroidCollected";

        /// <summary>
        /// Sended to yourself when player created structure (parameter is QuestStructureType enum and strcuture GameObject itself)
        /// </summary>
        public const string OnStructureCreated = "OnStructureCreated";
    }
}
