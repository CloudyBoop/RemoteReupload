using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.Core;
using VRC.UI;
using ReMod.Core.UI;
using VRC;
using System.Net.Http;
using ReMod.Core.VRChat;
using RemoteReupload.Core;

[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonInfo(typeof(RemoteReupload.Main), "Remote Reupload", "1", "CloudyBoop")]
namespace RemoteReupload
{
    public class Main : MelonMod, IAvatarListOwner
    {
        private Core.API api;
        private static List<ApiAvatar> avatars = new List<ApiAvatar>();
        private bool InGameAvatars = false;

        public void Clear(ReAvatarList avatarList)
        {
            throw new NotImplementedException();
        }

        public Il2CppSystem.Collections.Generic.List<ApiAvatar> GetAvatars(ReAvatarList avatarList)
        {
            Il2CppSystem.Collections.Generic.List<ApiAvatar> list = new Il2CppSystem.Collections.Generic.List<ApiAvatar>();
            foreach (ApiAvatar avatar in avatars)
            {
                list.Add(avatar);
            }
            return list;
        }

        public static Config config;
        [Obsolete]
        public override void OnApplicationStart()
        {
            config = Config.Load();
            if (config.API_URL == "Replace me") 
            {
                Console.Clear();
                MelonLogger.Warning("UPDATE CONFIG FILE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                MelonLogger.Warning("Config file is located in ...\\VRChat\\UserData\\RemoteReuploadConfig.json");
                MelonLogger.Warning("Open with any text editor and replace the API_URL with your own/provided API_URL");
                MelonLogger.Warning("Change any other settings as you see fit");
                return;
            }
            api = new Core.API(config.API_URL, config.API_Key);
            api.GetAvatars(config.Group, (avi) => { avatars = avi; });
            MelonCoroutines.Start(WaitForUI());

            
        }

        private IEnumerator WaitForUI()
        {
            if (config.API_URL == "Replace me") yield return null;
            while (VRCUiManager.prop_VRCUiManager_0 == null)
                yield return null;

            
            ScrollRect tabs = GameObject.Find("/UserInterface/MenuContent/Backdrop/Header/Tabs").AddComponent<ScrollRect>();
            tabs.vertical = false;
            tabs.content = GameObject.Find("/UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content").GetComponent<RectTransform>();
            tabs.viewport = GameObject.Find("/UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort").GetComponent<RectTransform>();

            GameObject SettingsPageButtonRef = GameObject.Find("/UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/SettingsPageTab");
            GameObject KustomAvatarButton = GameObject.Instantiate(SettingsPageButtonRef, SettingsPageButtonRef.transform.parent);
            KustomAvatarButton.transform.SetSiblingIndex(7);
            KustomAvatarButton.name = "KustomAvatarButton";
            KustomAvatarButton.GetComponent<LayoutElement>().minWidth = 250;
            KustomAvatarButton.GetComponent<LayoutElement>().preferredWidth = 250;
            KustomAvatarButton.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 105);
            KustomAvatarButton.GetComponentInChildren<Text>().text = "EPIK AVATARS";
            KustomAvatarButton.GetComponent<VRCUiPageTab>().field_Public_String_1 = "/UserInterface/MenuContent/Screens/KustomAvatarPage";

            GameObject AvatarPageRef = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar");
            GameObject MyAvatarPage = GameObject.Instantiate(AvatarPageRef, AvatarPageRef.transform.parent);
            MyAvatarPage.name = "KustomAvatarPage";
            MyAvatarPage.transform.FindChild("AvatarPreviewBase").localScale = new Vector3(0.7f, 0.7f, 0.7f);
            MyAvatarPage.transform.FindChild("Vertical Scroll View/Viewport/Content").Clear(false, true, true, "Avatar Worlds (Random)");
            MyAvatarPage.transform.FindChild("Vertical Scroll View/Viewport/Content/Avatar Worlds (Random)").Clear(false, false, false);
            MyAvatarPage.transform.Clear(true, true, false, "TitlePanel (1)", "Vertical Scroll View", "AvatarPreviewBase", "Change Button", "AvatarUiPrefab2");

            ReAvatarList avatarList = new ReAvatarList("My Reuploads", this, MyAvatarPage.transform.FindChild("Vertical Scroll View/Viewport/Content"), false, false);
            avatarList.RefreshAvatars();
            avatarList.GameObject.GetComponent<UiAvatarList>().field_Public_SimpleAvatarPedestal_0 = MyAvatarPage.transform.FindChild("AvatarPreviewBase/MainRoot/MainModel").GetComponent<SimpleAvatarPedestal>();
            var avatarlistviewport = avatarList.RectTransform.FindChild("ViewPort").GetComponent<RectTransform>();
            avatarlistviewport.sizeDelta = new Vector2(avatarlistviewport.sizeDelta.x, 500);
            avatarList.RectTransform.FindChild("ViewPort/Content").GetComponent<GridLayoutGroup>().constraintCount = 4;
            
            GameObject ChangeButton = MyAvatarPage.transform.FindChild("Change Button").gameObject;
            ChangeButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ChangeButton.GetComponent<Button>().onClick.AddListener((Action)delegate { MyAvatarPage.GetComponent<PageAvatar>().ChangeToSelectedAvatar(); });

            GameObject RemoveAvatar = GameObject.Instantiate(ChangeButton, ChangeButton.transform.parent);
            RemoveAvatar.GetComponent<RectTransform>().MoveFrom(ChangeButton, false, true, -1);
            RemoveAvatar.GetComponentInChildren<Text>().text = "Remove Avatar";
            RemoveAvatar.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            RemoveAvatar.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                api.Delete(MyAvatarPage.GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id);
                
            });

            GameObject ChangeGroupTag = GameObject.Instantiate(ChangeButton, ChangeButton.transform.parent);
            ChangeGroupTag.GetComponent<RectTransform>().MoveFrom(RemoveAvatar, false, true, -1);
            ChangeGroupTag.GetComponentInChildren<Text>().text = "Change Group";
            ChangeGroupTag.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ChangeGroupTag.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowInputPopupWithCancel("Change Group", "Enter new group", InputField.InputType.Standard,false, "Change", (s, k, t) =>
                {
                    config.Group = s;
                    config.Save();
                    avatarList.RefreshAvatars();
                }, null, "Enter new group...");
                
            });

            GameObject ChangeAPIKey = GameObject.Instantiate(ChangeButton, ChangeButton.transform.parent);
            ChangeAPIKey.GetComponent<RectTransform>().MoveFrom(ChangeButton, false, true, 8).MoveFrom(ChangeAPIKey, true, false, 1);
            ChangeAPIKey.GetComponent<RectTransform>().anchoredPosition += new Vector2(-20, 45);
            ChangeAPIKey.GetComponentInChildren<Text>().text = "Change API Key";
            ChangeAPIKey.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ChangeAPIKey.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowInputPopupWithCancel("Change API Key", "Enter new API Key", InputField.InputType.Standard, false, "Change", (s, k, t) =>
                {
                    config.API_Key = s;
                    config.Save();
                }, null, "Enter new API Key...");

            });

            GameObject UploadByIdent = GameObject.Instantiate(ChangeButton, ChangeButton.transform.parent);
            UploadByIdent.GetComponent<RectTransform>().MoveFrom(ChangeButton, false, true, 8).MoveFrom(UploadByIdent, true, false, 3);
            UploadByIdent.GetComponent<RectTransform>().anchoredPosition += new Vector2(-200, 45);
            UploadByIdent.GetComponentInChildren<Text>().text = "Upload By Ident";
            UploadByIdent.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            UploadByIdent.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowInputPopupWithCancel("Upload By Ident (Ripper.store)", "Enter Ident", InputField.InputType.Standard, false, "Upload", (s, k, t) =>
                {
                    api.UploadByIdent(s);
                }, null, "Enter Ident...");

            });
            GameObject ReuploadSelectedAvatar = GameObject.Instantiate(ChangeButton, ChangeButton.transform.parent);
            ReuploadSelectedAvatar.GetComponent<RectTransform>().MoveFrom(ChangeButton, false, true, 8).MoveFrom(ReuploadSelectedAvatar, true, false, 4);
            ReuploadSelectedAvatar.GetComponent<RectTransform>().anchoredPosition += new Vector2(-220, 45);
            ReuploadSelectedAvatar.GetComponentInChildren<Text>().text = "Reupload Selected Avatar";
            ReuploadSelectedAvatar.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ReuploadSelectedAvatar.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                api.Reupload(MyAvatarPage.GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0);
            });
            GameObject ToggleAvatarList = GameObject.Instantiate(ChangeButton, ChangeButton.transform.parent);
            ToggleAvatarList.GetComponent<RectTransform>().MoveFrom(ChangeButton, false, true, 8);
            ToggleAvatarList.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 45);
            ToggleAvatarList.GetComponentInChildren<Text>().text = "In Game Avatars";
            ToggleAvatarList.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ToggleAvatarList.GetComponent<Button>().onClick.AddListener((Action)delegate
            {
                InGameAvatars = !InGameAvatars;
                avatars.Clear();
                if (InGameAvatars)
                {
                    avatarList.Title = "In Game Avatars";
                    ToggleAvatarList.GetComponentInChildren<Text>().text = "Show My Avatars";
                    foreach (var ply in PlayerManager.prop_PlayerManager_0.prop_ArrayOf_Player_0)
                    {
                        avatars.Add(ply.prop_ApiAvatar_0);
                    }
                    avatarList.RefreshAvatars();
                }
                else
                {
                    avatarList.Title = "My Reuploads";
                    ToggleAvatarList.GetComponentInChildren<Text>().text = "In Game Avatars";
                    api.GetAvatars(config.Group, (avi) => { avatars = avi; });
                    avatarList.RefreshAvatars();
                }
            });
            


            }
    }
}
