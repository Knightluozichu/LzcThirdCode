using UnityEngine;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace RedRedJiang.Unity
{
    public static class UnityTool
    {
        public static Color TranslucenceColor = new Color(1, 1, 1, 0.75f);
        public static GameObject FindChild(GameObject _Parent, string _childName)
        {

            Transform[] childrens = _Parent.GetComponentsInChildren<Transform>();

            bool isFinded = false;

            Transform child = null;

            foreach (Transform i in childrens)
            {
                if (i.name == _childName)
                {
                    if (isFinded)
                    {
                        Debug.Log("同名的存在多个" + i + ",只返回找到的第一个");
                    }
                    isFinded = true;
                    child = i;
                }
            }

            if (isFinded) return child.gameObject;
            return null;
        }

        /// <summary>
        /// 矩形检查碰撞
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static bool IsRectTransformOverlap(RectTransform rect1, RectTransform rect2)
        {
            float rect1MinX = rect1.position.x - rect1.rect.width / 2;
            float rect1MaxX = rect1.position.x + rect1.rect.width / 2;
            float rect1MinY = rect1.position.y - rect1.rect.height / 2;
            float rect1MaxY = rect1.position.y + rect1.rect.height / 2;

            float rect2MinX = rect2.position.x - rect2.rect.width / 2;
            float rect2MaxX = rect2.position.x + rect2.rect.width / 2;
            float rect2MinY = rect2.position.y - rect2.rect.height / 2;
            float rect2MaxY = rect2.position.y + rect2.rect.height / 2;

            bool xNotOverlap = rect1MaxX <= rect2MinX || rect2MaxX <= rect1MinX;
            bool yNotOverlap = rect1MaxY <= rect2MinY || rect2MaxY <= rect1MinY;

            bool notOverlap = xNotOverlap || yNotOverlap;

            return !notOverlap;
        }

        /// <summary>
        /// 点是否在矩形内
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsRectOverPoint(Vector2 p1, Vector2 p2, float range)
        {
            return !(p2.x > p1.x + range || p2.y > p1.y + range || p2.x < p1.x - range || p2.y < p1.y - range);
        }

        public static UnityEngine.GameObject CreateGameObject(string _PrefabsName, Transform _PrefabsParent)
        {
            UnityEngine.GameObject gobPrefabs = ResourcesSystem.Instance.ResObj<UnityEngine.GameObject>(_PrefabsName, true);
            if (!gobPrefabs)
            {
                Debug.Log("要加载的对象不存在！" + "_PrefabsName:" + _PrefabsName + "     _PrefabsParent:" + _PrefabsParent + "    gobPrefabs:" + gobPrefabs);
                return null;
            }
            gobPrefabs = UnityEngine.GameObject.Instantiate<UnityEngine.GameObject>(gobPrefabs);

            gobPrefabs.transform.SetParent(_PrefabsParent, false);
            gobPrefabs.transform.localPosition = UnityEngine.Vector3.zero;
            gobPrefabs.transform.localRotation = UnityEngine.Quaternion.identity;
            gobPrefabs.transform.localScale = UnityEngine.Vector3.one;
            gobPrefabs.name = _PrefabsName;
            //gobPrefabs.SetActive(false);

            if (gobPrefabs == null)
                Debug.Log("没加载出来 检查参数！" + "_PrefabsName:" + _PrefabsName + "    _PrefabsParent:" + _PrefabsParent);

            return gobPrefabs;
        }

    }
}
