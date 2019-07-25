using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffImageManager : MonoBehaviour
{
    [SerializeField]
    BuffImage[] spriteList;

    public BuffImage GetBuffImage(in Buffable.CHAR_BUFF _buff)
    {
        foreach(BuffImage image in spriteList)
        {
            if (_buff != image.buff)
                continue;
            return image;
        }
        return new BuffImage(null, Buffable.CHAR_BUFF.NONE);
    }

    [System.Serializable]
    public struct BuffImage
    {
        public Sprite image;
        public Buffable.CHAR_BUFF buff;
        public BuffImage(Sprite image, Buffable.CHAR_BUFF buff)
        {
            this.image = image;
            this.buff = buff;
        }
    }
}
