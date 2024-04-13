#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

#pragma warning disable IDE0003

namespace DA_Assets.FCU
{
    [Serializable]
    public class FigmaSession : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public string Token { get; set; }
        public FigmaUser CurrentFigmaUser { get; set; }

        private List<FigmaSessionItem> sessionItems = new List<FigmaSessionItem>();
        private const string playerPrefsKey = "figmaSessions";
        private const int maxItems = 10;

        public bool IsAuthed()
        {
            if (this.CurrentFigmaUser.Name.IsEmpty() || this.Token.IsEmpty())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void AddNew(string token)
        {
            GetCurrentFigmaUser(token, x =>
            {
                if (x.Success)
                {
                    this.CurrentFigmaUser = x.Object;

                    SetLastSession(new FigmaSessionItem
                    {
                        Name = this.CurrentFigmaUser.Name,
                        Email = this.CurrentFigmaUser.Email,
                        Token = token
                    });

                    this.Token = token;
                    DALogger.LogSuccess(FcuLocKey.log_auth_complete.Localize());
                }
                else
                {
                    DALogger.LogError(FcuLocKey.log_cant_auth.Localize(x.Error.Status, x.Error.Message, x.Error.Exception));
                }
            }).StartDARoutine(monoBeh);
        }

        private IEnumerator GetCurrentFigmaUser(string token, Return<FigmaUser> @return)
        {
            DARequest request = new DARequest
            {
                Query = "https://api.figma.com/v1/me",
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {token}"
                }
            };

            yield return monoBeh.RequestSender.SendRequest(request, @return);
        }

        public void TryRestoreSession()
        {
            if (monoBeh.IsJsonNetExists() == false)
                return;

            if (IsAuthed() == false)
            {
                FigmaSessionItem item = GetLastSessionItem();

                if (item.IsDefault() == false)
                {
                    AddNew(item.Token);
                }
            }
        }

        private void SetLastSession(FigmaSessionItem sessionItem)
        {
            FigmaSessionItem targetItem = sessionItems.FirstOrDefault(item => item.Token == sessionItem.Token);
            sessionItems.Remove(targetItem);
            sessionItems.Insert(0, sessionItem);

            if (sessionItems.Count > maxItems)
            {
                sessionItems = sessionItems.Take(maxItems).ToList();
            }

            SaveDataToPrefs();
        }


        private FigmaSessionItem GetLastSessionItem()
        {
            LoadDataFromPrefs();
            return sessionItems.FirstOrDefault();
        }

        public FigmaSessionItem[] GetItems()
        {
            LoadDataFromPrefs();
            return sessionItems.ToArray();
        }

        private void LoadDataFromPrefs()
        {
#if UNITY_EDITOR
            string json = EditorPrefs.GetString(playerPrefsKey, "");

            if (json.IsEmpty())
                return;

            try
            {
                sessionItems = DAJson.FromJson<List<FigmaSessionItem>>(json);
            }
            catch
            {

            }
#endif
        }

        private void SaveDataToPrefs()
        {
#if UNITY_EDITOR && JSONNET_EXISTS
            string json = JsonConvert.SerializeObject(sessionItems);
            EditorPrefs.SetString(playerPrefsKey, json);
            LoadDataFromPrefs();
#endif
        }
    }

    public struct FigmaSessionItem
    {
#if JSONNET_EXISTS
        [JsonProperty("name")]
#endif
        public string Name { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("email")]
#endif
        public string Email { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("token")]
#endif
        public string Token { get; set; }
    }
}
