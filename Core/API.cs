using MelonLoader;
using Newtonsoft.Json;
using RemoteReupload.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;
using ReMod.Core.VRChat;

namespace RemoteReupload.Core
{
    public class API
    {
        private static string API_URL { get; set; }
        private static string API_KEY { get; set; }
        private static HttpClient Client { get; set; }

        [Obsolete]
        public API(string apiurl, string apikey)
        {
            API_URL = apiurl;
            API_KEY = apikey;
            Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("RemoteReupload/1.0");
        }

        internal void GetAvatars(string tag, Action<List<ApiAvatar>> p)
        {
            string body = Client.GetStringAsync(API_URL + "/" + tag).Result;
            List<Avatar> avatars = JsonConvert.DeserializeObject<List<Avatar>>(body);
            List<ApiAvatar> apiAvatars = new List<ApiAvatar>();
            foreach (var avatar in avatars)
            {
                ApiAvatar ava = new ApiAvatar();
                ava.id = avatar.Id;
                ava.name = avatar.Name;
                ava.assetUrl = avatar.AssetUrl;
                ava.thumbnailImageUrl = avatar.Thumbnail;
                ava.imageUrl = avatar.Thumbnail;
                apiAvatars.Add(ava);
            }
            p(apiAvatars);
        }

        internal void Delete(string id)
        {
            Client.DeleteAsync(API_URL + "?id=" + id).Wait();
        }

        internal void UploadByIdent(string s)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("Ident", s),
                new KeyValuePair<string, string>("Key", API_KEY),
                new KeyValuePair<string, string>("Group", Main.config.Group)
            });
                var resp = Client.PostAsync(API_URL + "/ReuploadByIdent", content).Result;
                resp.EnsureSuccessStatusCode();
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("Plz Wait", "Reuploading may take some time depending on the size of the file and server load.\nYou can close this window and continue to play Mirror Simulator.\nRefresh avatar list in 2 minutes or so.", 10);
            }
            catch (Exception ex)
            {
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("awww shit", "Something went wrong. \nCheck Console for more info.", 10);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }

        internal void Reupload(ApiAvatar apiavatar)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Asseturl", apiavatar.assetUrl),
                    new KeyValuePair<string, string>("Imageurl", apiavatar.imageUrl),
                    new KeyValuePair<string, string>("Name", apiavatar.name),
                    new KeyValuePair<string, string>("Group", Main.config.Group)
                });
                var resp = Client.PostAsync(API_URL + "/Reupload", content).Result;
                resp.EnsureSuccessStatusCode();
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("Plz Wait", "Reuploading may take some time depending on the size of the file and server load.\nYou can close this window and continue to play Mirror Simulator.\nRefresh avatar list in 2 minutes or so.", 10);
            }
            catch (Exception ex)
            {
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowAlert("awww shit", "Something went wrong. \nCheck Console for more info.", 10);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }
    }
}
