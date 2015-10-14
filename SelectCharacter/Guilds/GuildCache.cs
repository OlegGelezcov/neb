using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NebulaCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Guilds {
    public class GuildCache {

        private ConcurrentDictionary<string, DbObjectWrapper<Guild>> mGuilds = new ConcurrentDictionary<string, DbObjectWrapper<Guild>>();

        public void SetGuild(Guild guild) {
            mGuilds[guild.ownerCharacterId] = new DbObjectWrapper<Guild> { Changed = true, Data = guild };
        }

        public bool TryGetGuild(string ownerCharacterId, out DbObjectWrapper<Guild> guild) {
            return mGuilds.TryGetValue(ownerCharacterId, out guild);
        }

        public bool TryRemoveGuild(string ownerCharacterID, MongoCollection<Guild> c) {
            DbObjectWrapper<Guild> oldGuild;
            
            if( mGuilds.TryRemove(ownerCharacterID, out oldGuild) ) {
                c.Remove(Query<Guild>.EQ(g => g.ownerCharacterId, oldGuild.Data.ownerCharacterId));
                return true;
            }
            return false;
        }


        public void SaveModified(MongoCollection<Guild> collection) {
            foreach(var pGuild in mGuilds) {
                if(pGuild.Value.Changed) {
                    collection.Save(pGuild.Value.Data);
                    
                    pGuild.Value.Changed = false;
                }
            }
        }

        /// <summary>
        /// Mark guild as modified for resaving later
        /// </summary>
        /// <param name="guildID"></param>
        public void MarkModified(string guildID) {
            DbObjectWrapper<Guild> targetGuild;
            if(TryGetGuild(guildID, out targetGuild)) {
                targetGuild.Changed = true;
            }
        }
    }
}
