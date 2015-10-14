using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    /// <summary>
    /// Skill effect type
    /// </summary>
    public enum SkillType : byte
    {
        /// <summary>
        /// Skill instantly used add perform some action on data
        /// </summary>
        OneUse = 0,
        /// <summary>
        /// Skill active some limited interval and after expired he is off
        /// </summary>
        Durable = 1,
        /// <summary>
        /// Skill active while toggled on unlimited time
        /// </summary>
        Persistent = 2
    }
}
