using UnityEngine;
using Assets.Script.Enum;

/*
 * @author LuoZichu
 * @time 2019/7/1
 */

namespace Assets.Script.ClassStatic
{
    public static class StaticRectangular
    {
        /// <summary>
        /// 矩形碰撞
        /// </summary>
        /// <param name="_Rect1"></param>
        /// <param name="_Rect2"></param>
        /// <returns></returns>
        public static bool isCashRect(Rect _Rect1, Rect _Rect2)
        {
            if (_Rect1.x + _Rect1.width > _Rect2.x &&
                _Rect2.x + _Rect2.width > _Rect1.x &&
                _Rect1.y + _Rect1.height > _Rect2.y &&
                _Rect2.y + _Rect2.height > _Rect1.y
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 八个方向
        /// </summary>
        /// <param name="_Angle"></param>
        /// <returns></returns>
        public static EightDir GetDir(float _Angle)
        {
            if (_Angle >= 337.5f || _Angle < 22.5f)
            {
                return EightDir.Left;
            }
            else if (_Angle >= 22.5f && _Angle < 67.5f)
            {
                return EightDir.LeftDown;
            }
            else if (_Angle >= 67.5f && _Angle < 112.5f)
            {
                return EightDir.Down;
            }
            else if (_Angle >= 112.5f && _Angle < 157.5f)
            {
                return EightDir.RightDown;
            }
            else if (_Angle >= 157.5f && _Angle < 202.5f)
            {
                return EightDir.Right;
            }
            else if (_Angle >= 202.5f && _Angle < 247.5f)
            {
                return EightDir.RightUp;
            }
            else if (_Angle >= 247.5f && _Angle < 292.5f)
            {
                return EightDir.Up;
            }
            else
            {
                return EightDir.LeftUp;
            }

        }
    }
}
