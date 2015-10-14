using Common;
using System.Collections;
using System.Collections.Generic;

namespace Master.News {
    public class NewsService {

        public const int RECACHE_NEWS_INTERVAL = 18000;
        public MasterApplication application { get; private set; }

        public List<PostEntry> cachedNews { get; private set; }

        private int mLastNewsRequestedTime = 0;


        public NewsService(MasterApplication app) {
            application = app;
        }

        private void RecacheNews() {
            if(cachedNews == null ||
                (CommonUtils.SecondsFrom1970() - mLastNewsRequestedTime > RECACHE_NEWS_INTERVAL)) {
                mLastNewsRequestedTime = CommonUtils.SecondsFrom1970();
                cachedNews = application.database.GetAllPosts();
            }
        }


        public Hashtable GetNews(string lang) {
            RecacheNews();
            Hashtable result = new Hashtable();
            foreach(var post in cachedNews) {
                if(post.lang == lang ) {
                    result.Add(post.postID, post.GetInfo());
                }
            }
            return result;
        }
    }
}
