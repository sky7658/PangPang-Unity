using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PangPang.Board;
using PangPang.Quest;

namespace PangPang.Action
{
    public static class BlockAction
    {
        public static IEnumerator MoveToAction(Block baseBlock, Vector2 to, float duration)
        {
            Vector2 startPos = baseBlock.transform.position;

            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                elapsed += Time.smoothDeltaTime;
                baseBlock.transform.position = Vector2.Lerp(startPos, to, elapsed / duration);
                yield return null;
            }

            baseBlock.transform.position = to;

            yield break;
        }
        public static IEnumerator BlockPangAction(Block pangBlock, float toScale, float speed)
        {
            Transform pangBlockT = pangBlock.transform;

            float factor;
            while (pangBlockT.localScale.x > toScale)
            {
                factor = Time.deltaTime * speed;
                pangBlockT.localScale = new Vector3(pangBlockT.localScale.x - factor, pangBlockT.localScale.y - factor, pangBlockT.localScale.z);
                yield return null;
            }

            BlockPool.instance.ReturnBlock(pangBlock);

            yield break;
        }

        public static IEnumerator SpecialBlockAction(Block specialBlock, Vector2 to, float duration)
        {
            Vector2 startPos = specialBlock.transform.position;

            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                elapsed += Time.smoothDeltaTime;
                specialBlock.transform.position = Vector2.Lerp(startPos, to, elapsed / duration);
                yield return null;
            }

            BlockPool.instance.ReturnBlock(specialBlock);

            yield break;
        }

        public static IEnumerator FadeInOutAction(SpriteRenderer spr, float duration)
        {
            float elapsed = duration;

            while(true)
            {
                // Fade Out
                while (elapsed > 0f)
                {
                    elapsed -= Time.smoothDeltaTime;
                    spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, elapsed / duration);
                    yield return null;
                }

                elapsed = 0.0f;

                // Fade In
                while (elapsed < duration)
                {
                    elapsed += Time.smoothDeltaTime;
                    spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, elapsed / duration);
                    yield return null;
                }
            }

            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
            yield break;
        }
    }
}
