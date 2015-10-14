// MmoTargetComponent.cs
// Nebula
//
// Created by Oleg Zheleztsov on Wednesday, September 9, 2015 2:10:20 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Mmo.Items.Components {
    using System;
    using Common;
    using Nebula.Mmo.Games;
    using UnityEngine;

    public class MmoTargetComponent : MmoBaseComponent {

        private float mUpdateAgroIconTime = 0;

        public override ComponentID componentID {
            get {
                return ComponentID.Target;
            }
        }

        public bool hasTarget {
            get {
                if (item != null) {
                    bool has = false;
                    if (item.TryGetProperty<bool>((byte)PS.HasTarget, out has)) {
                        return has;
                    }
                }
                return false;
            }
        }

        public bool inCombat {
            get {
                if (item != null) {
                    bool comb = false;
                    if (item.TryGetProperty<bool>((byte)PS.InCombat, out comb)) {
                        return comb;
                    }
                }
                return false;
            }
        }

        public string targetID {
            get {
                if (item == null) {
                    return string.Empty;
                }
                string tID = string.Empty;
                if (!item.TryGetProperty<string>((byte)PS.TargetId, out tID)) {
                    return string.Empty;
                }
                return tID;
            }
        }

        public byte targetType {
            get {
                if (item == null) { return 0; }
                byte tType;
                if (!item.TryGetProperty<byte>((byte)PS.TargetType, out tType)) {
                    return 0;
                }
                return tType;
            }
        }

    }
}
