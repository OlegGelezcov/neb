namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    /// <summary>
    /// Start 10 small rocket with total damage on 20% larger than usual shoot
    /// </summary>
    public class ScatteredShoot : UseSkill
    {
        public ScatteredShoot()
        {
            ID = "SK0118";
            _cooldown = 10.0f;
            active_time = 0;
            effect = 1.2f;
            _energy = 8.0f;
        }

        public override void Use(ICombatActor actor)
        {
            if (ready)
            {
                if (!actor.ShipDestroyed)
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
