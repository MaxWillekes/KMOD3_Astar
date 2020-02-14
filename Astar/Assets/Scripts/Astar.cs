using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        Node StartNode = new Node(startPos, null, 0, (int)Vector2Int.Distance(startPos, endPos));
        Node EndNode;

        List<Node> OpenList = new List<Node>();
        List<Node> ClosedList = new List<Node>();

        Node[,] nodeGrid;
        nodeGrid = new Node[grid.GetLength(0), grid.GetLength(1)];

        //Make nodes for all the cells in the grid
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                //Might need to change the 0 later.
                nodeGrid[x, y] = new Node(grid[x, y].gridPosition, null, int.MaxValue, (int)Vector2Int.Distance(grid[x, y].gridPosition, endPos));
            }
        }

        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];

            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[1].FScore < CurrentNode.FScore || OpenList[i].FScore == CurrentNode.FScore && OpenList[i].HScore < CurrentNode.HScore)
                {
                    CurrentNode = OpenList[i];
                }
            }
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode.HScore == 0)
            {
                List<Vector2Int> FinalPath = new List<Vector2Int>();

                EndNode = CurrentNode;

                while (CurrentNode != StartNode)
                {
                    FinalPath.Add(CurrentNode.position);
                    CurrentNode = CurrentNode.parent;
                }

                FinalPath.Reverse();


                return FinalPath;
            }

            foreach (Node NeighborNode in GetNeighboringNodes(CurrentNode, grid, nodeGrid))
            {
                if (ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
                int MoveCost = (int)CurrentNode.GScore + GetManHattenDistance(CurrentNode, NeighborNode);

                if (MoveCost < NeighborNode.GScore)
                {
                    NeighborNode.GScore = MoveCost;
                    NeighborNode.HScore = GetManHattenDistance(NeighborNode, nodeGrid[endPos.x, endPos.y]);
                    NeighborNode.parent = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }

        return null;
    }

    public List<Node> GetNeighboringNodes(Node a_Node, Cell[,] grid, Node[,] nodeGrid)
    {
        List<Node> NeighboringNodes = new List<Node>();

        int xCheck;
        int yCheck;

        //Right side
        xCheck = a_Node.position.x + 1;
        yCheck = a_Node.position.y;
        if (xCheck >= 0 && xCheck < grid.GetLength(0) && !grid[a_Node.position.x, a_Node.position.y].HasWall(Wall.RIGHT))
        {
            if (yCheck >= 0 && yCheck < grid.GetLength(1))
            {
                NeighboringNodes.Add(nodeGrid[xCheck, yCheck]);
            }
        }

        //Left side
        xCheck = a_Node.position.x - 1;
        yCheck = a_Node.position.y;
        if (xCheck >= 0 && xCheck < grid.GetLength(0) && !grid[a_Node.position.x, a_Node.position.y].HasWall(Wall.LEFT))
        {
            if (yCheck >= 0 && yCheck < grid.GetLength(1))
            {
                NeighboringNodes.Add(nodeGrid[xCheck, yCheck]);
            }
        }

        //Top side
        xCheck = a_Node.position.x;
        yCheck = a_Node.position.y + 1;
        if (xCheck >= 0 && xCheck < grid.GetLength(0) && !grid[a_Node.position.x, a_Node.position.y].HasWall(Wall.UP))
        {
            if (yCheck >= 0 && yCheck < grid.GetLength(1))
            {
                NeighboringNodes.Add(nodeGrid[xCheck, yCheck]);
            }
        }

        //Bottoms side
        xCheck = a_Node.position.x;
        yCheck = a_Node.position.y - 1;
        if (xCheck >= 0 && xCheck < grid.GetLength(0) && !grid[a_Node.position.x, a_Node.position.y].HasWall(Wall.DOWN))
        {
            if (yCheck >= 0 && yCheck < grid.GetLength(1))
            {
                NeighboringNodes.Add(nodeGrid[xCheck, yCheck]);
            }
        }

        return NeighboringNodes;
    }

    int GetManHattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.position.x - a_nodeB.position.x);
        int iy = Mathf.Abs(a_nodeA.position.y - a_nodeB.position.y);

        return ix + iy;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
