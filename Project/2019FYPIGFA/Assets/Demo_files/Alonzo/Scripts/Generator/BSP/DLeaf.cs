using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLeaf : MonoBehaviour
{
    private static uint minLeafSize = 6;
    static float dimensionThresh = 1.25f;

    public int x, y;
    public int width, height;

    public DLeaf leftChild, rightChild;
    // TODO: variable for rectangle room (Rectangle room;)
    // TODO: variable for hallway Vector halls

    public DLeaf(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public bool Split()
    {
        // Split the leaf into 2 children
        if (leftChild != null || rightChild != null)
            return false; // Already was split

        // Determine direction of split
        // if the width is >25% larger than height, we split vertically
        // if the height is >25% larger than the width, we split horizontally
        // else, split randomly
        bool splitHorizontally = Random.value > 0.5f;
        if (width > height && width / height >= dimensionThresh)
            splitHorizontally = false;
        else if (height > width && height / width >= dimensionThresh)
            splitHorizontally = true;

        // determine the maximum height or width
        int max = (splitHorizontally ? height : width) - (int)minLeafSize;
        if (max <= minLeafSize)
            return false; // area's too small to split anymore

        int split = (int)Random.Range(minLeafSize, max); // Determine where to split

        // Create our left and right children based on the direction of split
        if (splitHorizontally)
        {
            leftChild = new DLeaf(x, y, width, split);
            rightChild = new DLeaf(x, y + split, width, height - split);
        }
        else
        {
            leftChild = new DLeaf(x, y, split, height);
            rightChild = new DLeaf(x + split, y, width - split, height);
        }
        return true; // split successful!
    }
}
