using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;

namespace RemoteReupload.Core
{
    public static class Extension
    {
        public static Transform Clear(this Transform transform, bool ChangeScaleToZero,bool SetActive, bool Destroy, params string[] keep)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (!keep.Contains(child.name))
                {
                    if (ChangeScaleToZero)
                    {
                        child.localScale = Vector3.zero;
                    }
                    child.gameObject.SetActive(SetActive);
                    if (Destroy) 
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
            return transform;
        }
        public static RectTransform MoveFrom(this RectTransform recttransform, GameObject other, bool x = false, bool y = false, float spacing = 1) 
        {
            RectTransform otherrect = other.GetComponent<RectTransform>();
            recttransform.anchoredPosition = new Vector2(x ? otherrect.anchoredPosition.x + (otherrect.sizeDelta.x * spacing) : otherrect.anchoredPosition.x , y ? otherrect.anchoredPosition.y + (otherrect.sizeDelta.y * spacing) : otherrect.anchoredPosition.y);
            return recttransform;
        }
        public static void StartRenderElementsCoroutine(this UiVRCList instance, Il2CppSystem.Collections.Generic.List<ApiAvatar> avatarList, int offset = 0, bool endOfPickers = true, VRCUiContentButton contentHeaderElement = null)
        {
            if (!instance.gameObject.activeInHierarchy || !instance.isActiveAndEnabled || instance.isOffScreen ||
                !instance.enabled)
                return;

            if (instance.scrollRect != null)
            {
                instance.scrollRect.normalizedPosition = new Vector2(0f, 0f);
            }
            instance.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0(avatarList, offset, endOfPickers, contentHeaderElement);
        }
    }
}
