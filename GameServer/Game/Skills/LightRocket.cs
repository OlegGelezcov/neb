﻿namespace Space.Game.Skills
{
    using System;
    using Common;
    using System.Collections;


    public class LightRocket : UseSkill
    {

        public LightRocket()
        {
            ID = "SK0115";
            _cooldown = 0.0f;
            active_time = 0.0f;
            effect = 0.8f;
            _energy = 100;
        }

        public override void updateCooldown(ICombatActor actor)
        {
            if (actor != null)
                _cooldown = actor.GetShip.GetWeapon.Cooldown * 0.5f;
            else
                ConsoleLogging.Get.Print("actor null in skill");
        }

        public override void Use(ICombatActor actor)
        {
            if (ready)
            {

                if (actor.ShipDestroyed == false)
                {

                    ICombatActor targetActor;
                    if (actor.GetTarget.TargetIsICombatActor(out targetActor))
                    {

                        if (actor.GetShip.Energy.EnoughAdd(_energy))
                        {
                            UseTimeUpdate();
                            bool isHitted = actor.GetShip.GetWeapon.TryHit();
                            float actualDamage = 0;

                            if (isHitted)
                            {
                                float damage = actor.GetMyWeaponDamage() * effect;
                                actualDamage = targetActor.ApplyDamageToMe(actor.Avatar.Type, actor.Avatar.Id, damage);
                            }
                            StartClientEvent(actor, new Hashtable() { 
                                {GenericEventProps.IsHItted, isHitted},
                                {GenericEventProps.actual_damage, actualDamage}
                            });
                        }
                        else
                        {
                            SendClientErrorEvent(actor, ReturnCode.Fatal, string.Format("source has {0} energy but need: {1}", actor.GetShip.Energy.Energy, _energy));
                        }
                    }
                    else
                    {
                        SendClientErrorEvent(actor, ReturnCode.Fatal, "Target not valid");
                    }
                }
                else
                {
                    SendClientErrorEvent(actor, ReturnCode.Fatal, "Actor ship destroyed");
                }
            }
            else
            {
                SendClientErrorEvent(actor, ReturnCode.Fatal, "Skill not ready");
            }
        }
    }

}