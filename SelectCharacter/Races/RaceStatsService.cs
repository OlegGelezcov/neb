using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SelectCharacter.Races {
    public class RaceStatsService : IInfoSource  {

        public SelectCharacterApplication application { get; private set; }

        //use properti
        private RaceStats mStats;
        private bool mStatsChanged;

        public RaceStatsService(SelectCharacterApplication app) {
            application = app;
        }

        public RaceStats stats {
            get {
                if(mStats == null ) {
                    mStats = application.DB.raceStats.FindOne();
                    if(mStats == null) {
                        mStats = new RaceStats();
                        application.DB.raceStats.Save(mStats);
                    }
                }
                return mStats;
            }
        }

        public void Save() {
            if(mStatsChanged) {
                application.DB.raceStats.Save(stats);
                mStatsChanged = false;
            }
        }

        public void Clear() {
            stats.Clear();
            mStatsChanged = true;
        }

        public void AddPoints(Race race, int pt) {
            stats.AddPoints(race, pt);
            mStatsChanged = true;
        }

        public void SetPoints(Race race, int pt) {
            stats.SetPoints(race, pt);
            mStatsChanged = true;
        }

        public Hashtable GetInfo() {
            return stats.GetInfo();
        }
    }
}
