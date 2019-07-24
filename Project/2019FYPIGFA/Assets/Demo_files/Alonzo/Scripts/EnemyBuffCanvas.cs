using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyBuffCanvas : MonoBehaviour
{
    const float scale = 2.5906f;
    
    private Player player;
    private List<BuffImage> m_buffList; // List of buff icon game objects
    EnemyBuffImageManager imageManager;
    // Start is called before the first frame update
    void Start()
    {
        player = (Player)FindObjectOfType(typeof(Player));
        imageManager = (EnemyBuffImageManager)FindObjectOfType(typeof(EnemyBuffImageManager));
        if (null == player)
            Debug.LogError("Could not find player for canvas");
        if (null == imageManager)
            Debug.LogError("Could not find buff image manager");
        AddBuff(Buffable.CHAR_BUFF.DEBUFF_BURN);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);
    }
    public void RemoveBuff(Buffable.CHAR_BUFF _buff)
    {
        foreach (BuffImage image in m_buffList)
        {
            if (_buff != image.buff)
                continue;
            m_buffList.Remove(image);
        }
    }
    public void AddBuff(Buffable.CHAR_BUFF _buff)
    {
        if (CheckForExistingBuff(_buff))
            return;
        CheckForExistingBuff(_buff);
        EnemyBuffImageManager.BuffImage buffImage = imageManager.GetBuffImage(_buff);
        if (Buffable.CHAR_BUFF.NONE == buffImage.buff)
            Debug.LogError("No image matching the buff has been found");
        GameObject newImage = new GameObject(buffImage.buff + "buff");
        newImage.AddComponent<Image>();
        BuffImage newBuffImage = new BuffImage(newImage, _buff);
        m_buffList.Add(newBuffImage);
    }
    /// <summary>
    /// Function to check if given buff exists in canvas
    /// </summary>
    /// <param name="_buff">buff to look for in list</param>
    /// <returns>true if buff already exists</returns>
    bool CheckForExistingBuff(Buffable.CHAR_BUFF _buff)
    {
        foreach(BuffImage image in m_buffList)
        {
            if (_buff == image.buff)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Sorts the buff list. Should be called every time the list is modified
    /// </summary>
    void SortBuffList()
    {
        // TODO: use a constant variable for height. Scale and size to be set here.
        // CHECK: should the size be dynamic?
    }
    public struct BuffImage
    {
        public GameObject GO;
        public Buffable.CHAR_BUFF buff;

        public BuffImage(GameObject GO, Buffable.CHAR_BUFF buff)
        {
            this.GO = GO;
            this.buff = buff;
        }
    }
}
