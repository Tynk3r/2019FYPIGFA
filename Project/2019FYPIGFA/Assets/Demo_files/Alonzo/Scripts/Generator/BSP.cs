using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BSP
{
    public class Leaf
    {
        public Vector2 pos;
        public Vector2 size;
        public Leaf leftChild;
        public Leaf rightChild;

        public Leaf(Vector2 pos, Vector2 size)
        {
            this.pos = pos;
            this.size = size;
        }
        public bool Split(ref List<Leaf> _leaves)
        {
            
            return false;
        }
        /// <summary>
        /// Gives 2 leaves that are results from splitting leaf that calls this method
        /// First leaf is Bottom Left
        /// </summary>
        /// <param name="_line">Line position where leaf is cut, depending on _horizontal</param>
        /// <param name="_horizontal">Direction of cut. (Horizontal is a leaf on top of the other)</param>
        /// <returns></returns>
        public Leaf[] SplitLeaf(float _line, bool _horizontal)
        {
            var children = new Leaf[2];
            if (_horizontal)    // |-|
            {
                float lowestY = pos.y - size.y * 0.5f;
                float lineHeight = _line - lowestY;
                float remainderHeight = size.y - lineHeight;
                children = new Leaf[2] { new Leaf(new Vector2(pos.x, lowestY + lineHeight * 0.5f), new Vector2(size.x, lineHeight)),
                                         new Leaf(new Vector2(pos.x, _line + remainderHeight * 0.5f), new Vector2(size.x, remainderHeight)) };
            }
            else                // |||
            {
                float lowestX = pos.x - size.x * 0.5f;
                float lineWidth = _line - lowestX;
                float remainderWidth = size.x - lineWidth;
                children = new Leaf[2] { new Leaf(new Vector2(lowestX + lineWidth * 0.5f, pos.y), new Vector2(lineWidth, size.y)),
                                         new Leaf(new Vector2(_line + remainderWidth * 0.5f, pos.y), new Vector2(remainderWidth, size.y)) };
            }
            return children;
        }
        public void SplitRecursively(float _minWidth, ref List<Leaf> _leaves)
        {
            // if the size is big enough, add leaves and call this function on them
            if (size.x > _minWidth * 2f)
            {
                Leaf leftChild = new Leaf(new Vector2(pos.x - size.x * 0.75f, pos.y), new Vector2(size.x * 0.5f, size.y));
                Leaf rightChild = new Leaf(new Vector2(pos.x + size.x * 0.75f, pos.y), new Vector2(size.x * 0.5f, size.y));
                leftChild.SplitRecursively(_minWidth, ref _leaves);
                rightChild.SplitRecursively(_minWidth, ref _leaves);
                return;
            }
            // Add self instead
            _leaves.Add(this);
        }
        public Leaf[] SplitLeafMirrored(float _line, bool _horizontal)
        {
            var children = new Leaf[2];
            if (_horizontal)    // |=|
            {
                float lowestY = pos.y - size.y * 0.5f;
                float lineHeight = _line - lowestY;
                float remainderHeight = size.y - lineHeight * 2f;
                if (lineHeight > pos.y)
                    Debug.LogError("Line height is too high");
                children = new Leaf[3] { new Leaf(new Vector2(pos.x, lowestY + lineHeight * 0.5f), new Vector2(size.x, lineHeight)),
                                         new Leaf(new Vector2(pos.x, _line + remainderHeight * 0.5f), new Vector2(size.x, remainderHeight)),
                                         new Leaf(new Vector2(pos.x, _line + remainderHeight + lineHeight * 0.5f), new Vector2(size.x, lineHeight))};
            }
            else                // ||||
            {
                float lowestX = pos.x - size.x * 0.5f;
                float lineWidth = _line - lowestX;
                float remainderWidth = size.x - lineWidth * 2f;
                if (lineWidth > pos.x)
                    Debug.LogError("Line height is too high");
                children = new Leaf[3] { new Leaf(new Vector2(lowestX + lineWidth * 0.5f, pos.y), new Vector2(lineWidth, size.y)),
                                         new Leaf(new Vector2(_line + remainderWidth * 0.5f, pos.y), new Vector2(remainderWidth, size.y)),
                                         new Leaf(new Vector2(_line + remainderWidth + lineWidth * 0.5f, pos.y), new Vector2(lineWidth, size.y))};
            }
            return children;
        }
        public enum LeafType
        {

            PARENT_END,

            CHILD,
            COLD_VEG,
            COLD_MEAT,
            COLD_PASTRIES,

        }
    }
    // TODO: DELETE THIS
    public static GameObject GenerateLayout(Vector2 _pos, Vector2 _size, ref List<MapGenerator.Prop> _propList)
    {
        // NOTE: OBJECTS FACE -Z direction
        var leaves = new List<Leaf>();

        Leaf parent = new Leaf(_pos, _size); // Create the first root leaf
        float backDepth = _size.y * 0.75f; // This will be at where the back unit starts
        Leaf[] leavesH1 = parent.SplitLeaf(backDepth, true); // Split horizontally to get back side
        // TODO: determine shelf information here
        float sideWidth = leavesH1[0].size.x * 0.2f;
        Leaf[] leavesV1 = leaves[0].SplitLeafMirrored(leavesH1[0].size.x * 0.2f, false);// Split vertically to get left and right side

        // First find the size of the centre leaf
        MapGenerator.Prop prop = _propList[Random.Range(0, _propList.Count)];
        // split horizontally in the centre leaf to get the shelf leaves
        List<Leaf> shelfLeaves = SplitShelfLeaves(leavesV1[1]);

        GameObject propObjects = new GameObject();
        // Spawn shelves into each shelfLeaf
        foreach(Leaf leaf in shelfLeaves)
        {
            GameObject newShelf = Object.Instantiate(prop.prefab);
            newShelf.transform.position = new Vector3(leaf.pos.x, prop.minBounds.y, leaf.pos.y);
            newShelf.transform.parent = propObjects.transform;
        }
        return propObjects;
    }
    
    static List<Leaf> SplitShelfLeaves(Leaf _shelfLeaf)
    {
        const float MIN_WIDTH = 4f; 
        List<Leaf> shelfLeaves = new List<Leaf>();
        _shelfLeaf.SplitRecursively(MIN_WIDTH, ref shelfLeaves);
        return shelfLeaves;
    }

    // Complete when more props are made
    static List<MapGenerator.Prop> GetFittingProp(Vector2 _leafSize, in List<MapGenerator.Prop> _propList)
    {
        MapGenerator.Prop prop = _propList[Random.Range(0, _propList.Count)];
        //float width = -prop.minBounds.x + prop.maxBounds.x;
        float depth = -prop.minBounds.z + prop.maxBounds.z;
        

        return null;
    }
}
