from pymongo import MongoClient
import pickle




client = MongoClient("mongodb://localhost:27017")
db = client.user_logins
print(db.collection_names())
cursor = db.userstats.find()

stats = [document for document in cursor]
print(stats)
bdata = pickle.dumps(stats)
print(bdata)
loaded_obj = pickle.loads(bdata)
print(loaded_obj)

# for document in cursor:
#     stats.append(UserStat(document))
#
# for stat in stats:
#     print(stat)
#
# stat0 = stats[0]
# print('sessions after point:', stat0.session_count_after(1474528289))



