using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    public static float g_threshold = 1.25f; // Edit if rooms need different split range
    public int level;
    public float x, y;
    public float width, height;
    public Leaf rightChild, leftChild;
    public Room room;
    

    public Leaf(float x, float y, float width, float height, int level)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        rightChild = leftChild = null;
        room = null;
        this.level = level;
    }
    // Returns a leaf containing the room
    public Leaf GetLowestLeaf()
    {
        if (room != null)
        {
            return this;
        }
        if (leftChild == null || rightChild == null)
        {
            Debug.LogAssertion("Unable to find any room or children in leaf: " + this);
            return null;
        }
        return (Random.value > 0.5f ? leftChild.GetLowestLeaf() : rightChild.GetLowestLeaf());
    }

    public bool Split(float _minLeafSize, float _maxLeafSize, int _index, ref List<Leaf> _leafList)
    {
        // Split the leaf into 2 children but stop if there are already children. The lord does not want too many
        if (leftChild != null || rightChild != null)
        {
            Debug.LogError("These unwise leaves need to be punished for their sins!");
        }
        //else if (Random.value < 0.2f)   // NERF SIZES
        //    return false;
        // Determine direction of split (horizontal or vertical)
        // If the width is [> threshold] larger than height, split vertically
        // If the height is [> threshold] larger than width, split horizontally
        // Else split randomly
        //TODO: why the fuck does this need > comparisons?
        bool splitHorizontally;
        if (width > height && width / height >= g_threshold)
            splitHorizontally = false;
        else if (height > width && height / width >= g_threshold)
            splitHorizontally = true;
        else
            splitHorizontally = Random.value > 0.5f;

        // Determine the max height or width
        float max = (splitHorizontally ? height : width) - _minLeafSize;
        if (max <= _minLeafSize)
            return false; // No longer able to be split

        float split = Random.Range(_minLeafSize, max); // Determine where to split
        // Create left and right children based on direction of split
        if (splitHorizontally)
        {
            float lPosY, rPosY;
            lPosY = y + height / 2 - split / 2;
            rPosY = y - height / 2 + (height - split) / 2;
            leftChild = new Leaf(x, lPosY, width, split, level + 1);
            rightChild = new Leaf(x, rPosY, width, height - split, level + 1);
        }
        else
        {
            float lPosX, rPosX;
            lPosX = x - width / 2 + split / 2;
            rPosX = x + width / 2 - (width - split) / 2;
            leftChild = new Leaf(lPosX, y, split, height, level + 1);
            rightChild = new Leaf(rPosX, y, width - split, height, level + 1);
        }
        // Now add bring these leaves to the almighty list
        _leafList.Add(leftChild);
        _leafList.Add(rightChild);
        // Now make the children split up for the good of the future
        leftChild.Split(_minLeafSize, _maxLeafSize, _index + 1, ref _leafList);
        rightChild.Split(_minLeafSize, _maxLeafSize, _index + 1, ref _leafList);
        return true;
    }
}
