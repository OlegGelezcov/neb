using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public class GameTimeInfo {
        public int passes { get; private set; }
        public int currentTime { get; private set; }
        public int expireTime { get; private set; }

        public void SetFrom(GameTimeInfo other) {
            passes = other.passes;
            currentTime = other.currentTime;
            expireTime = other.expireTime;
        }

        public void SetPasses(int iPasses) {
            passes = iPasses;
        }

        public void SetCurrentTime(int iCurTime) {
            currentTime = iCurTime;
        }

        public void SetExpireTime(int iExpireTime) {
            expireTime = iExpireTime;
        }

        public bool hasTime {
            get {
                return currentTime < expireTime;
            }
        }

        public int numberHoursRemain {
            get {
                if(currentTime >= expireTime  ) {
                    return 0;
                }
                TimeSpan interval = TimeSpan.FromSeconds(expireTime - currentTime);
                return (int)Math.Ceiling(interval.TotalHours);
            }
        }

        public int numberMinutesRemain {
            get {
                if(currentTime >= expireTime ) {
                    return 0;
                }
                TimeSpan interval = TimeSpan.FromSeconds(expireTime - currentTime);
                return (int)Math.Ceiling(interval.TotalMinutes);
            }
        }
    }
}
