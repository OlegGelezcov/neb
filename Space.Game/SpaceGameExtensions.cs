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

        public static List<T> Shuffle<T>(this List<T> source) {
            int i = source.Count;
            while( i > 1) {
                i -= 1;
                int j = Rand.Int(0, i - 1);
                T temp = source[j];
                source[j] = source[i];
                source[i] = temp;
            }
            return source;
        }

        public static T AnyElement<T>(this List<T> list) {
            int index = Rand.Int(0, list.Count - 1);
            return list[index];
        }
    }

    
}
