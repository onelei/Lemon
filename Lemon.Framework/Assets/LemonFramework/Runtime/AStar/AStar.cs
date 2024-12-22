using System.Collections.Generic;

namespace LemonFramework.AStar
{
    public static class AStar
    {
        private const int Barrier = 3;

        /// <summary>
        /// 打开列表；
        /// </summary>
        private static readonly List<Node> OpenNodes = new();

        /// <summary>
        /// 关闭列表；
        /// </summary>
        private static readonly List<Node> CloseNodes = new();

        private static int[,] _map;
        private static int _width;
        private static int _height;

        /// <summary>
        /// 初始化地图数据；
        /// </summary>
        public static void SetData(int[,] map, int width, int height)
        {
            _map = map;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// 开始寻路
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static bool FindPath(Node start, Node end, ref List<Node> nodes)
        {
            nodes.Clear();

            if (start.Equal(end))
            {
                nodes.Add(start);
                return true;
            }

            //清除打开和关闭列表；
            OpenNodes.Clear();
            CloseNodes.Clear();

            start.G = 0;
            start.H = start.GetH(end);
            start.Parent = null;

            OpenNodes.Add(start);

            while (OpenNodes.Count > 0)
            {
                // 找出open表里面最小的F;
                var currenNode = GetMinF(OpenNodes);
                if (currenNode.Equal(null))
                {
                    return false;
                }

                // 判断当前的节点是不是目标点,是的话就找到了路径;
                if (currenNode.Equal(end))
                {
                    while (!currenNode.Equal(start))
                    {
                        nodes.Add(currenNode);
                        currenNode = currenNode.Parent;
                    }

                    nodes.Add(start);
                    //from last get
                    //nodes.Reverse();
                    OpenNodes.Clear();
                    CloseNodes.Clear();
                    return true;
                }

                // 没有找到就移除Open列表里面;
                OpenNodes.Remove(currenNode);
                CloseNodes.Add(currenNode);

                // 判断邻居节点;
                CheckNeighbors(currenNode, end);
            }

            nodes.Clear();
            OpenNodes.Clear();
            CloseNodes.Clear();
            return false;
        }

        private static void CheckNeighbors(Node currentNode, Node targetNode)
        {
            var neighbors = currentNode.GetNeighbors();

            foreach (var neighborNode in neighbors)
            {
                // 排除边界点;
                var x = neighborNode.Position.X;
                var y = neighborNode.Position.Y;
                if (x < 0 || x >= _height || y < 0 || y >= _width)
                {
                    continue;
                }

                // 排除障碍物和关闭列表中的点;
                if (_map[x, y] == Barrier || Contains(CloseNodes, neighborNode))
                {
                    continue;
                }

                // 在Open表格里面;
                if (TryGetNode(OpenNodes, neighborNode, out Node node))
                {
                    //判断和邻居自检的G
                    int newG = currentNode.GetG(node) + currentNode.G;
                    if (newG < node.G)
                    {
                        node.Parent = currentNode;
                        node.G = newG;
                    }
                }
                else
                {
                    //不在Open表格里面;
                    neighborNode.Parent = currentNode;
                    int newG = currentNode.GetG(neighborNode) + currentNode.G;
                    neighborNode.G = newG;
                    neighborNode.H = neighborNode.GetH(targetNode);
                    OpenNodes.Add(neighborNode);
                }
            }
        }

        /// <summary>
        ///  // 找出open表里面最小的F;
        /// </summary>
        /// <returns></returns>
        private static Node GetMinF(List<Node> openNodes)
        {
            Node result = null;
            var fValue = int.MaxValue;
            foreach (var t in openNodes)
            {
                if (t.F < fValue)
                {
                    fValue = t.F;
                    result = t;
                }
            }

            return result;
        }

        private static bool Contains(List<Node> nodes, Node key)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node.Position == key.Position)
                    return true;
            }

            return false;
        }

        private static bool TryGetNode(List<Node> nodes, Node key, out Node result)
        {
            foreach (var node in nodes)
            {
                if (node.Position == key.Position)
                {
                    result = node;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}