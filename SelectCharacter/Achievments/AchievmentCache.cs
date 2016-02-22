using Common;
using MongoDB.Driver.Builders;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SelectCharacter.Achievments {
    public class AchievmentCache {
        private const float UPDATE_FROM_DB_INTERVAL = 600;

        private ConcurrentDictionary<string, AchievmentCacheObject> m_Achievments;
        private SelectCharacterApplication m_App;

        public AchievmentCache(SelectCharacterApplication app) {
            m_App = app;
            m_Achievments = new ConcurrentDictionary<string, AchievmentCacheObject>();
        }

        public void Remove(string characterId ) {
            AchievmentCacheObject obj;
            m_Achievments.TryRemove(characterId, out obj);
        }

        public Hashtable GetAchievments(string characterId ) {
            AchievmentCacheObject cacheObject;
            if(m_Achievments.TryGetValue(characterId, out cacheObject)) {
                if(CommonUtils.SecondsFrom1970() - cacheObject.updateFromDBTime > UPDATE_FROM_DB_INTERVAL ) {
                    cacheObject.updateFromDBTime = CommonUtils.SecondsFrom1970();
                    cacheObject.document = GetDocumentFromDB(characterId);
                }
                if(cacheObject.document != null && cacheObject.document.variables != null ) {
                    return cacheObject.document.variables;
                }
                return new Hashtable();
            } else {
                AchievmentCacheObject newObj = new AchievmentCacheObject {
                    characterId = characterId,
                    updateFromDBTime = CommonUtils.SecondsFrom1970(),
                    document = GetDocumentFromDB(characterId)
                };

                if(m_Achievments.TryAdd(characterId, newObj)) {
                    if(newObj.document != null && newObj.document.variables != null ) {
                        return newObj.document.variables;
                    }
                }
                return new Hashtable();
            }
        }

        private AchievmentDocument GetDocumentFromDB(string characterId ) {
            var document = m_App.DB.achievments.FindOne(Query<AchievmentDocument>.EQ(a => a.characterId, characterId));
            if(document == null ) {
                return new AchievmentDocument {
                    characterId = characterId,
                    isNewDocument = false,
                    variables = new Hashtable(),
                    visitedZones = new List<string>()
                };
            }
            return document;
        }
    }
}
