using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Eses.UI
{

    public class RaycastAlphaMask : MonoBehaviour, ICanvasRaycastFilter
    {

        [Header("Settings")]
        public float tolerance = 0.05f;
        public bool correctAspectRatio;
        public AspectRatioFitter.AspectMode aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

        [Header("Debug")]
        [SerializeField] Rect rtRect;
        [SerializeField] Vector2 localPos;
        [SerializeField] Rect spriteRect;
        [SerializeField] Vector2 spritePos;
        [SerializeField] Vector4 border;

        // local
        RectTransform rt;
        Image image;
        Sprite sprite;
        Texture2D spriteTexture;


        void Awake()
        {
            rt = this.transform as RectTransform;
            image = this.GetComponent<Image>();
            sprite = gameObject.GetComponent<Image>().sprite;
            spriteTexture = (Texture2D)sprite.texture;

            if (correctAspectRatio)
            {
                CorrectAspectRatio();
            }
        }


        public bool IsRaycastLocationValid(Vector2 sp, Camera cam)
        {
            if (image.type == Image.Type.Simple)
            {
                return HitSimple(sp, cam);
            }

            if (image.type == Image.Type.Sliced)
            {
                return HitSliced(sp, cam);
            }

            return false;
        }


        void CorrectAspectRatio()
        {

            // NOTE: 
            // If simple mode is used calculations 
            // won't match if RectTransform dimensions
            // don't match image dimensions.
            // This happens when Image mode is set to "Simple"
            // and Image / Preserve Aspect is active

            // NOTE: 
            // This call can be removed if RectTransform
            // size is manually matched to Image size or Image
            // is not using Preserve Aspect.

            if (image.preserveAspect && image.type == Image.Type.Simple)
            {
                var asf = gameObject.AddComponent<AspectRatioFitter>();
                asf.aspectMode = aspectMode;
            }
        }


        bool HitSimple(Vector2 sp, Camera cam)
        {
            // NOTE: 
            // current limitation:
            // If Image Component uses Preserve Aspect
            // its rect must match sprite visible rect

            spriteRect = sprite.textureRect;

            // point in rt
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, sp, cam, out localPos);

            // Account for non default sprite size
            var mx = spriteRect.width / rt.rect.width;
            var my = spriteRect.height / rt.rect.height;

            // Account for non zero pivot
            var px = rt.pivot.x * rt.rect.width;
            var py = rt.pivot.y * rt.rect.height;

            // RectTransform space to sprite space
            spritePos.x = spriteRect.x + ((localPos.x + px) * mx);
            spritePos.y = spriteRect.y + ((localPos.y + py) * my);

            // Read pixel
            var col = spriteTexture.GetPixel((int)spritePos.x, (int)spritePos.y);

            return col.a > tolerance;
        }


        bool HitSliced(Vector2 sp, Camera cam)
        {

            // calculate localPos
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, sp, cam, out localPos);

            // local pos with pivot 
            localPos.x = localPos.x + (rt.pivot.x * rt.rect.width);
            localPos.y = localPos.y + (rt.pivot.y * rt.rect.height);

            // shorthands
            spriteRect = sprite.textureRect;
            border = sprite.border;

            // borders -> x:L z:R y:B w:T
            var x = 0f;
            var y = 0f;


            // x-axis -----

            // left edge
            if (localPos.x <= border.x)
            {
                x = spriteRect.x + localPos.x;
            }

            // center
            if (localPos.x > border.x && localPos.x < rt.rect.width - border.z)
            {
                var relativePosInRectCenterPart = (localPos.x - border.x) / (rt.rect.width - border.x - border.z);
                var absolutePosInSpriteCenterPart = (spriteRect.width - border.x - border.z) * relativePosInRectCenterPart;
                x = spriteRect.x + border.x + absolutePosInSpriteCenterPart;
            }

            // right edge
            if (localPos.x > rt.rect.width - border.z)
            {
                var dist = border.z - (rt.rect.width - localPos.x);
                x = (spriteRect.x + spriteRect.width - border.z) + dist;
            }



            // y-axis ------

            // bottom edge
            if (localPos.y < border.y)
            {
                y = spriteRect.y + localPos.y;
            }

            // center
            if (localPos.y > border.y && localPos.y < rt.rect.height - border.w)
            {
                var relativePosInRectCenterPart = (localPos.y - border.y) / (rt.rect.height - border.y - border.w);
                var absolutePosInSpriteCenterPart = (spriteRect.height - border.y - border.w) * relativePosInRectCenterPart;
                y = spriteRect.y + border.y + absolutePosInSpriteCenterPart;
            }

            // top edge
            if (localPos.y > rt.rect.height - border.w)
            {
                y = spriteRect.y + spriteRect.height - (rt.rect.height - localPos.y);
            }


            // sprite pos (debug)
            spritePos.x = x;
            spritePos.y = y;

            // Read pixel
            var col = spriteTexture.GetPixel(Mathf.FloorToInt(x), Mathf.FloorToInt(y));

            return col.a > tolerance;
        }

    }

}