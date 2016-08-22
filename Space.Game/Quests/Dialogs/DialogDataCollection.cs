using Common;
using Nebula.Quests.Actions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests.Dialogs {
    public class DialogDataCollection : ConcurrentDictionary<string, DialogData> {

        public void Load(string file) {
            Clear();
            XDocument document = XDocument.Load(file);
            PostActionParser postActionParser = new PostActionParser();
            var dumpList = document.Element("dialogs").Elements("dialog").Select(dialogElement => {
                string id = dialogElement.GetString("id");
                List<PostAction> postActions = null;
                XElement postActionsElement = dialogElement.Element("post_actions");
                if(postActionsElement != null ) {
                    postActions = postActionParser.ParseList(postActionsElement);
                } else {
                    postActions = new List<PostAction>();
                }
                DialogData dialogData = new DialogData(id, postActions);
                TryAdd(dialogData.id, dialogData);
                return dialogData;
            }).ToList();
        }

        public DialogData GetDialogData(string id) {
            DialogData result = null;
            if(TryGetValue(id, out result)) {
                return result;
            }
            return null;
        }
    }
}
