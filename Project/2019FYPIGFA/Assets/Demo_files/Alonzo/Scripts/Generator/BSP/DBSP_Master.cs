using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DBSP_Master
{
    private static uint maxLeafSize = 20;

    public static DLeaf[] GenerateMap(int _mapWidth, int _mapHeight)
    {
        var leafs = new DLeaf[1];

        // First, create a leaf to be the 'root' of all leaves
        DLeaf root = new DLeaf(0, 0, _mapWidth, _mapHeight);
        leafs[0] = root;

        bool didSplit = true;
        // Loop through every leaf in our Vector over and over again
        int i = 0;
        while (didSplit)
        {
            didSplit = false;

            DLeaf curr = leafs[i++];
            if (curr.leftChild == null && curr.rightChild == null)
            {
                // if leaf is too big or 75% chance
                if (curr.width > maxLeafSize || curr.height > maxLeafSize || Random.value > 0.25f)
                {
                    if (curr.Split()) // split the leaf!
                    {
                        // Resize the array 
                        System.Array.Resize(ref leafs, leafs.Length + 2);
                        // if we did split, push the child leaves into list so it can be looped into
                        leafs[leafs.Length - 2] = curr.leftChild;
                        leafs[leafs.Length - 1] = curr.rightChild;
                        didSplit = true;
                    }
                }
            }
        }
        return leafs;
    }
}
