
using System.Collections;


namespace Space.Game
{
	public class Skill
	{
		public string ID{ get; set; }
		public int lvl{ get; set; }
		public int progress{ get; set; }
		public int expForFirstLevel{ get; set; }
		public int skillFactor{ get; set; }
		public float effect{get;set;}

//		[System.Flags]
//		public enum SkillState
//		{
//			open,
//			closed
//		}
//		public SkillState state;
         
		public void AddExp(int value)
		{
			progress += value;
			if(progress >= needExp)
			{
				progress = progress - needExp;
				lvl++;
			}
		}
		
		public int needExp
		{
			get
			{
				return expForFirstLevel * fib(lvl+1) * skillFactor;
			}
		}

		public virtual float getEffect
		{
			get
			{
				return effect;
			}
		}
		
		private int fib(int n)
		{
			int[] f = new int[3];
			f[1] = 2;
			for (int i = 2; i <= n; i++)
			{
				f[i%3] = f[(i + 1)%3] + f[(i + 2)%3];
			}
			return f[n%3];
		}

        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }
	}
}