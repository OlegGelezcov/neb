// Skill_000007D6.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 11:09:16 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Game;
using Space.Server;
using System.Collections;
using System.Collections.Concurrent;

namespace Nebula.Game.Skills {

    //Heal allies and damage on enemies
    public class Skill_000007D6 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            bool castOnTarget = true;
            if (source.Target().hasTarget) {
                if (FriendTargetInvalid(source)) {
                    castOnTarget = false;
                } else {
                    if (NotCheckDistance(source)) {
                        info.SetSkillUseState(Common.SkillUseState.tooFar);
                        return false;
                    }
                }
            } else {
                castOnTarget = false;
            }

            float radius = skill.GetFloatInput("radius");
            float healMult = skill.GetFloatInput("heal_mult");
            float dmgMult = skill.GetFloatInput("dmg_mult");

            bool mastery = RollMastery(source);
            if(mastery) {
                healMult *= 2;
                dmgMult *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            NebulaObject enemyCenterObj = null;
            float healVal = source.Weapon().GetDamage().totalDamage;
            if (castOnTarget ) {

                var tObj = source.Target().targetObject;

                var heal = source.Weapon().Heal(tObj, healMult * healVal, skill.idInt);
                source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
                var nearestFriend = GetNearestFriend(tObj, source, radius);
                info.Add((int)SPC.Var1, new Hashtable { { (int)SPC.ItemId, tObj.Id }, { (int)SPC.ItemType, tObj.Type } });


                if (nearestFriend != null ) {
                    heal = source.Weapon().Heal(nearestFriend, healMult * healVal, skill.idInt);
                    source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
                    info.Add((int)SPC.Var2, new Hashtable { { (int)SPC.ItemId, nearestFriend.Id }, { (int)SPC.ItemType, nearestFriend.Type } });
                }
                enemyCenterObj = source.Target().targetObject;
                

            } else {
                var heal = source.Weapon().HealSelf(healVal, skill.idInt);
                source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
                info.Add((int)SPC.Var1, new Hashtable { { (int)SPC.ItemId, source.Id }, { (int)SPC.ItemType, source.Type } });

                var nearestFriend = GetNearestFriend(source, source, radius);
                if(nearestFriend != null ) {
                    heal = source.Weapon().Heal(nearestFriend, healMult * healVal, skill.idInt);
                    source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
                    info.Add((int)SPC.Var2, new Hashtable { { (int)SPC.ItemId, nearestFriend.Id }, { (int)SPC.ItemType, nearestFriend.Type } });
                }
                enemyCenterObj = source;
            }

            var sWeapon = source.Weapon();
            var sMessage = source.MmoMessage();
            foreach(var enemy in GetNearestEnemies(enemyCenterObj, source, radius)) {
                WeaponHitInfo hit;
                var fire = sWeapon.Fire(enemy.Value, out hit, skill.idInt, dmgMult);
                sMessage.SendShot(Common.EventReceiver.OwnerAndSubscriber, fire);
                log.InfoFormat("send fire to object {0}->{1}".Olive(), enemy.Value.Id, enemy.Value.getItemType());
            }
            //var weapon = source.Weapon();
            //var message = source.MmoMessage();
            //var targetObject = source.Target().targetObject;

            //var healValue = weapon.GetDamage(false).totalDamage * healMult;
            //var firstHeal = weapon.Heal(targetObject, healValue, skill.data.Id);
            //message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, firstHeal);

            //var healTargets = GetHealTargets(source, targetObject, radius);
            //foreach(var pHealTarget in healTargets) {
            //    var secondHeal = weapon.Heal(pHealTarget.Value, healValue, skill.data.Id);
            //    message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, secondHeal);
            //    break;
            //}

            //var dmgTargets = GetTargets(source, targetObject, radius);
            //foreach(var pDmgTarget in dmgTargets) {
            //    WeaponHitInfo hit;
            //    var shot = weapon.Fire(pDmgTarget.Value, out hit, skill.data.Id, dmgMult);
            //    message.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
            //}
            return true;
        }

        private ConcurrentDictionary<string, Item> GetNearestEnemies(NebulaObject centerObj, NebulaObject myObj, float radius ) {
            var myCharacter = myObj.Character();
            return centerObj.mmoWorld().GetItems(item => {
                if (item.Character() && item.Damagable() && item.Bonuses()) {
                    if (centerObj.transform.DistanceTo(item.transform) <= radius) {
                        var relation = myCharacter.RelationTo(item.Character());
                        if (relation == Common.FractionRelation.Enemy || relation == Common.FractionRelation.Neutral) {
                            return true;
                        }
                    }
                }
                return false;
            });
        }

        private NebulaObject GetNearestFriend(NebulaObject centerObj, NebulaObject myObj, float radius ) {
            var centerCharacter = centerObj.Character();
            var myCharacter = myObj.Character();

            var playerItems = centerObj.mmoWorld().GetItems(item => {
                if (item.getItemType() == Common.ItemType.Avatar) {
                    if (myCharacter.RelationTo(item.Character()) == Common.FractionRelation.Friend) {
                        if (item.Id != myObj.Id && item.Id != centerObj.Id) {
                            return true;
                        }
                    }
                }
                return false;
            });

            Item nearestPlayerItem = null;
            float curDist = float.MaxValue;
            foreach(var kvp in playerItems ) {
                float dist = kvp.Value.transform.DistanceTo(centerObj.transform);
                if(dist < curDist ) {
                    nearestPlayerItem = kvp.Value;
                    curDist = dist;
                }
            }

            if(curDist <= radius ) {
                if(nearestPlayerItem != null  ) {
                    return nearestPlayerItem;
                }
            }

            var nonPlayerItems = centerObj.mmoWorld().GetItems(item => {
                if(item.getItemType() != Common.ItemType.Avatar ) {
                    var itemCharacter = item.Character();
                    var itemDamagable = item.Damagable();
                    var itemBonuses = item.Bonuses();
                    if(itemCharacter && itemDamagable && itemBonuses ) {
                        if (item.Id != myObj.Id && item.Id != centerObj.Id) {
                            if (myCharacter.RelationTo(itemCharacter) == Common.FractionRelation.Friend) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            });

            Item nearestNonPlayerItem = null;
            curDist = float.MaxValue;
            foreach(var kvp in nonPlayerItems ) {
                float dist = centerObj.transform.DistanceTo(kvp.Value.transform);
                if(dist < curDist ) {
                    nearestNonPlayerItem = kvp.Value;
                    curDist = dist;
                }
            }

            if(curDist <= radius ) {
                return nearestNonPlayerItem;
            }

            return null;
        }
    }
}
