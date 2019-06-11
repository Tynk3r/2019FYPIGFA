using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapTreeGenerator
{
    public static int MAX_LEAF_SIZE = 20;

    // This function only generates the room layout and not the details
    public static List<Leaf> GenerateLeaves(int _mapWidth, int _mapHeight)
    {
        List<Leaf> leaves = new List<Leaf>();
        Leaf leaf; // A reference to current leaf
        int i = 0; // Index on current leaf.
        int count = 1;// Number of elements in the array
        bool splitting = true;

        // First, create a root leaf
        leaves.Add(new Leaf(0, 0, _mapWidth, _mapHeight));
        // Loop through every leaf and resize whenever split occurs
        while (splitting && i < count)
        {
           // Debug.Log("Iteration no: " + i + "| count is " + count + "| length is " + leaves.Count);
            splitting = false;
            leaf = leaves[i++];

            if (leaf.leftChild == null && leaf.rightChild == null)
            {
                // If the leaf is too big, or 75% chance
                if (leaf.width > MAX_LEAF_SIZE || leaf.height > MAX_LEAF_SIZE || Random.value > 0.25f)
                {
                    if (leaf.Split())
                    {
                        // If we split, push the child leafs to the list, increase count
                        count += 2;
                        leaves.Add(leaf.leftChild);
                        leaves.Add(leaf.rightChild);
                        splitting = true;
                    }
                }
            }
        }
        return leaves;
    }



    //public static List<Leaf> GenerateRooms(int _mapWidth, int _mapHeight)
    //{
    //    List<Leaf> leaves = new List<Leaf>();
    //    bool splitting = true;

    //    // TODO: change to allow parameters
    //    // First, create a root leaf
    //    leaves.Add(new Leaf(0, 0, _mapWidth, _mapHeight));
    //    // Loop through every leaf and resize whenever split occurs
    //    while (splitting)
    //    {
    //        splitting = false;
    //        foreach(Leaf i in leaves)
    //        {
    //            if (i.leftChild == null && i.rightChild == null)
    //            {
    //                // If leaf is too big, or 75% chance
    //                if (i.width > MAX_LEAF_SIZE || i.height > MAX_LEAF_SIZE || Random.value > 0.25)
    //                {
    //                    if (i.Split()) //split the leaf
    //                    {
    //                        // If we did split, add the child leaves into list
    //                        leaves.Add(i.leftChild);
    //                        leaves.Add(i.rightChild);
    //                        splitting = true;
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    return leaves;
    //}
}
