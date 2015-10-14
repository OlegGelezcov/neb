using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using Space.Game.Resources;
using GameMath;

namespace Space.Game
{
    public static class SpaceGameExtensions
    {
        //public static List<AttackTargetType> GetAttackTargetList(this XElement e, string name)
        //{
        //    List<AttackTargetType> list = new List<AttackTargetType>();
        //    string str = e.Attribute(name).Value;
        //    string[] strArr = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string s in strArr)
        //    {
        //        AttackTargetType type = (AttackTargetType)Enum.Parse(typeof(AttackTargetType), s);
        //        list.Add(type);
        //    }
        //    return list;
        //}

        public static IdleStateType GetIdleType(this XElement e, string name)
        {
            string s = e.Attribute(name).Value;
            return (IdleStateType)Enum.Parse(typeof(IdleStateType), s);
        }
    }

    
}
