namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;
    public class AcidCharge : UseSkill
    {

        public AcidCharge()
        {
            ID = "SK0107";
            _cooldown = 20;
            active_time = 0.0f;
            effect = 1.25f;
            _energy = 100;
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
                                //actor.ApplyDamageToMe(actualDamage * 0.1f);
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
