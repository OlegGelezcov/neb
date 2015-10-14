using System.Collections;
using Space;
using System.Xml.Serialization;
using Space.Game;
using Space.Mmo.Server;
using Photon.SocketServer;
using Space.Server;
using Common;

//[XmlInclude(typeof(HeavyRocket))]
//[XmlInclude(typeof(AcidCharge))]
//[XmlInclude(typeof(LightRocket))]
//[XmlInclude(typeof(DecreasePrecision))]
//[XmlInclude(typeof(IncreaseDamage))]
//[XmlInclude(typeof(Jump))]


public abstract class UseSkill : ActiveSkill
{
    public abstract void Use(ICombatActor item);

    public virtual void Touch()
    {

    }

    public virtual void Release() { 
    
    }
}





