using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public class Node 
    {
        public int F, G, H; //g: 起始节点到任意节点的代价； h: 任意节点到目标节点的启发式评估代价
        public Node parent;

        public int X, Y;

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Node(int x, int y, int g, int h, Node parent)
        {
            X = x; Y = y;
            G = g; H = h;
            F = G + H;

            this.parent = parent;
        }

        public void SetGH(int g, int h) 
        {
            G = g;
            H = h;
            F = G + H;
        }

        public void SetParent(Node parent)
        {
            this.parent = parent;
        }

        public void ResetG(int g, Node parent) 
        {
            G = g;
            F = G + H;

            this.parent = parent;
        }
    }

    private const int OneUnitCost = 1;
    public delegate void NotifyPointChange(Vector3Int point);

    private static List<Vector3Int> adjacentPos = new List<Vector3Int>(){ 
        new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0)
    };

    public static List<Vector3Int> AStarAlgorithm(Vector3Int start, Vector3Int end, List<Vector3Int> obstacles, NotifyPointChange callback = null)
    {
        List<Vector3Int> ret = new List<Vector3Int>();

        List<Node> open = new List<Node>();
        List<Node> close = new List<Node>();

        open.Add(GetNode(start.x, start.y, start, end));

        while(open.Count > 0)
        {
            //1. 从open列表里取出一个
            Node node = GetMiniNode(open, close);

            close.Add(node);
            open.Remove(node);

            if (node.X == end.x && node.Y == end.y)
                break;

            //2. 获取此节点的相邻点
            foreach (var pos in adjacentPos)
            {
                Vector3Int point = pos + new Vector3Int(node.X, node.Y, 0);
                if (IsPointInObstacles(point.x, point.y, obstacles) || IsPointInNodeList(point.x, point.y, close) != null)
                    continue;

                Node adjacentNode = IsPointInNodeList(point.x, point.y, open);
                if (adjacentNode != null)
                {
                    if (adjacentNode.G > node.G + OneUnitCost)
                    {
                        adjacentNode.ResetG(node.G + OneUnitCost, node);
                    }
                }
                else 
                {
                    Node newNode = GetNode(point.x, point.y, start, end);
                    newNode.SetParent(node);

                    open.Add(newNode);

                    callback?.Invoke(new Vector3Int(point.x, point.y, 0));
                }
            }
        }

        Node endNode = IsPointInNodeList(end.x, end.y, close);
        while(endNode != null)
        {
            ret.Add(new Vector3Int(endNode.X, endNode.Y, 0));
            endNode = endNode.parent;
        }

        return ret;
    }

    private static bool IsPointInObstacles(int x, int y, List<Vector3Int> list)
    {
        foreach (var item in list)
        {
            if (item.x == x && item.y == y)
                return true;
        }
        return false;
    }

    private static Node IsPointInNodeList(int x, int y, List<Node> list)
    {
        foreach (var item in list)
        {
            if (item.X == x && item.Y == y)
                return item;
        }
        return null;
    }

    
    private static Node GetMiniNode(List<Node> openNodes, List<Node> closeNodes)
    {
        int f = int.MaxValue;
        int index = -1;
        for(int i = openNodes.Count - 1; i >= 0; i--)
        {
            if (openNodes[i].F < f)
            {
                f = openNodes[i].F;
                index = i;
            }
        }

        return openNodes[index];
    }

    private static Node GetNode(int x, int y, Vector3Int start, Vector3Int end)
    {
        Node node = new Node(x, y);

        int g = (int)Mathf.Abs(x - start.x) + (int)Mathf.Abs(y - start.y);
        int h = (int)Mathf.Abs(x - end.x) + (int)Mathf.Abs(y - end.y);
        node.SetGH(g, h);
        
        return node;
    }
}
