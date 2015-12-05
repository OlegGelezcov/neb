// ClientItemTargetInfo.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Friday, December 5, 2014 1:00:22 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//


namespace Nebula.Client {

    public class ClientItemTargetInfo {
        public ClientItemTargetInfo(bool hasTarget, string targetId, byte targetType) {
            this.HasTarget = hasTarget;
            this.TargetId = targetId;
            this.TargetType = targetType;
        }


        public bool HasTarget { get; private set; }
        public string TargetId { get; private set; }
        public byte TargetType { get; private set; }

        public static ClientItemTargetInfo Default {
            get {
                return new ClientItemTargetInfo(false, string.Empty, (byte)0);
            }
        }

        public void SetTargetId(string targetId) {
            this.TargetId = targetId;
        }

        public void SetTargetType(byte targetType) {
            this.TargetType = targetType;
        }

        public void SetHasTarget(bool hasTarget) {
            this.HasTarget = hasTarget;
        }
    }
}
