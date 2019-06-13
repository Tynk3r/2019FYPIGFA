using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf
{
    public static float g_threshold = 1.25f; // Edit if rooms need different split range
    public static int MIN_LEAF_SIZE = 6;

    public int x, y;
    public int width, height;
    public Leaf rightChild, leftChild;
    public Room room;
    

    public Leaf(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        rightChild = leftChild = null;
        room = null;
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

    public bool Split()
    {
        // Split the leaf into 2 children but stop if there are already children
        if (leftChild != null || rightChild != null)
            return false;

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
        int max = (splitHorizontally ? height : width) - MIN_LEAF_SIZE;
        if (max <= MIN_LEAF_SIZE)
            return false; // No longer able to be split

        int split = Random.Range(MIN_LEAF_SIZE, max); // Determine where to split
        // Create left and right children based on direction of split
        if (splitHorizontally)
        {
            int lPosY, rPosY;
            lPosY = y + height / 2 - split / 2;
            rPosY = y - height / 2 + (height - split) / 2;
            leftChild = new Leaf(x, lPosY, width, split);
            rightChild = new Leaf(x, rPosY, width, height - split);
        }
        else
        {
            int lPosX, rPosX;
            lPosX = x - width / 2 + split / 2;
            rPosX = x + width / 2 - (width - split) / 2;
            leftChild = new Leaf(lPosX, y, split, height);
            rightChild = new Leaf(rPosX, y, width - split, height);
        }
        return true;
    }
}
