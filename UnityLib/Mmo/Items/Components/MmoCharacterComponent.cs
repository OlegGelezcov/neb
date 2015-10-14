namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoCharacterComponent : MmoBaseComponent {

        //void test() {
        //    var mmoCharacter = item.GetMmoComponent(ComponentID.Character) as MmoCharacterComponent;
        //    if(mmoCharacter != null ) {
        //        var myMmoCharacter = G.Game.Avatar.GetMmoComponent(ComponentID.Character) as MmoCharacterComponent;
        //        myMmoCharacter.fraction
        //    }
        //}

        //void test() {
        //    var mmoCharacter = item.GetMmoComponent(ComponentID.Character) as MmoCharacterComponent;
        //    if(mmoCharacter != null ) {
        //        Debug.Log(mmoCharacter.login);
        //    }
        //}

        public Workshop workshop {
            get {
                if (item != null) {
                    byte w;
                    if (item.TryGetProperty<byte>((byte)PS.Workshop, out w)) {
                        return (Workshop)w;
                    }
                }
                return Workshop.Arlen;
            }
        }

        public int level {
            get {
                if (item != null) {
                    int lvl;
                    if (item.TryGetProperty<int>((byte)PS.Level, out lvl)) {
                        return lvl;
                    }
                }
                return 0;
            }
        }

        public FractionType fraction {
            get {

                if (item != null) {
                    int f;
                    if (item.TryGetProperty<int>((byte)PS.Fraction, out f)) {
                        return (FractionType)f;
                    }
                }
                return FractionType.PlayerHumans;
            }
        }

        public string login {
            get {
                if (item != null) {
                    string login;
                    if (item.TryGetProperty<string>((byte)PS.Login, out login)) {
                        return login;
                    }
                }
                return string.Empty;
            }
        }

        public Race race {
            get {
                if (item != null) {
                    byte r;
                    if (item.TryGetProperty<byte>((byte)PS.Race, out r)) {
                        return (Race)r;
                    }
                }
                return Race.None;
            }
        }

        public string characterID {
            get {
                if (item == null) { return string.Empty; }
                string cID = string.Empty;
                if (!item.TryGetProperty<string>((byte)PS.CharacterID, out cID)) {
                    return string.Empty;
                }
                return cID;
            }
        }

        public string characterName {
            get {
                if (item != null) {
                    string cName = string.Empty;
                    if (item.TryGetProperty<string>((byte)PS.CharacterName, out cName)) {
                        return cName;
                    }
                }
                return string.Empty;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Character;
            }
        }
    }
}