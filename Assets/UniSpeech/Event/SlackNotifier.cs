using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UniSpeech.Event
{
    public class SlackNotifier : MonoBehaviour
    {

        public bool NewNotification;
        public string ID;

        public IEnumerator InitGetNotification()
        {
            string TOKEN = "xoxp-407985696885-412261255793-425155204212-015eabfdc0f5b5751b55869f8d61c12b";
            string channel = "GC5MKM5QX";

            string url = string.Format(
           "https://slack.com/api/groups.history?token={0}&channel={1}&count=1",
           TOKEN, channel
           );

            this.NewNotification = false;

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string s = request.downloadHandler.text.Replace("[{", "\"hi\", ").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
                s = "{" + s + "}";
                Item item = JsonUtility.FromJson<Item>(s);
                ID = item.client_msg_id;
                //SubItem subItem = JsonUtility.FromJson<SubItem>(item.messages);
                //Debug.Log("Debug test: "+subItem.text);
            }
        }

        public IEnumerator GetNotification()
        {
            string TOKEN = "xoxp-407985696885-412261255793-425155204212-015eabfdc0f5b5751b55869f8d61c12b";
            string channel = "GC5MKM5QX";

            string url = string.Format(
           "https://slack.com/api/groups.history?token={0}&channel={1}&count=1",
           TOKEN, channel
           );

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                //Debug.Log(request.downloadHandler.text);
                string s = request.downloadHandler.text.Replace("[{", "\"hi\", ").Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
                s = "{" + s + "}";
                //Debug.Log(s);
                Item item = JsonUtility.FromJson<Item>(s);
                if (ID == null && ID != item.client_msg_id) {
                    ID = item.client_msg_id;
                }

                else if (ID != item.client_msg_id)
                {
                    this.NewNotification = true;
                    ID = item.client_msg_id;
                   //Debug.Log("Different");
                }
                else
                {
                    //Debug.Log("Same");
                }
                
            }
        }

        [System.Serializable]
        public class Item
        {
            public bool ok;
            public string messages;
            public string type;
            public string user;
            public string text;
            public string client_msg_id;
            public string ts;
            public bool has_more;
        }

        public void SetBool(bool val)
        {
            this.NewNotification = val;
        }

        public void ShowNewNotification()
        {
            Debug.Log("Debug notification: " + this.NewNotification);
        }
    }
}