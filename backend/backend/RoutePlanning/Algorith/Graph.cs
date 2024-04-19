using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace backend.RoutePlanning.Algorith
{
    public class Graph
    {
        private readonly Node[] nodes;
        public Node[,] MyGraph { get; }

        public int Cost { get; set; } = 1;

        private bool doneGraph = false;

        public Graph()
        {
            nodes = new Node[100];
            MyGraph = new Node[10, 10];
        }

        public Node[,]? MakeGraph()
        {
            if (doneGraph)
            {
                ResetGraphNodes();
                return MyGraph;
            }

            for (int i = 0; i < 100; i++)
            {
                nodes[i] = new Node();
            }

            int j = 0;
            int sizeRow = MyGraph.GetLength(0);
            int sizeColunm = MyGraph.GetLength(1);

            for (int y = 0; y < sizeRow; y++)
            {
                for (int x = 0; x < sizeColunm; x++)
                {
                    Node node = nodes[j];
                    node.Neighbors = GetNeighbors(x, y, j);

                    node.Position = (x, y);
                    MyGraph[x, y] = node;
                    j++;
                }
            }
            doneGraph = true;

            return MyGraph;
        }

        private List<(Node, int)> GetNeighbors(int x, int y, int j)
        {
            List<(Node, int)> neighbors = new();

            int sizeRow = MyGraph.GetLength(0);
            int sizeColunm = MyGraph.GetLength(1);

            bool isBeginX = x == 0;
            bool isEndX = x == sizeColunm - 1;

            bool isBeginY = y == 0;
            bool isEndY = y == sizeRow - 1;

            if (isBeginX)
            {
                neighbors.Add(NeighborRight(j));
            }
            else if (isEndX)
            {
                neighbors.Add(NeighborLeft(j));
            }
            else
            {
                neighbors.Add(NeighborLeft(j));
                neighbors.Add(NeighborRight(j));
            }

            if (isBeginY)
            {
                neighbors.Add(NeighborDown(j));
            }
            else if (isEndY)
            {
                neighbors.Add(NeighborUp(j));
            }
            else
            {
                neighbors.Add(NeighborDown(j));
                neighbors.Add(NeighborUp(j));
            }

            return neighbors;
        }

        private void ResetGraphNodes()
        {
            foreach (Node node in nodes)
            {
                node.Previous = null;
                node.CostToSource = int.MaxValue;
            }
        }

        public void BlockPosition(Node[,] graph, (int x, int y) position)
        {
            SetCost(graph, position, -1);
        }

        public void UnBlockPostion(Node[,] graph, (int x, int y) position)
        {
            SetCost(graph, position, Cost);
        }

        private void SetCost(Node[,] graph, (int x, int y) position, int value)
        {
            Node node = graph[position.x, position.y];

            for(int i = 0; i < node.Neighbors.Count; i++)
            {
                node.Neighbors[i] = (node.Neighbors[i].node, value);
            }

            foreach ((Node neighbor, _) in node.Neighbors)
            {
                SetNeighborsEdgeOf(neighbor, value, node);
            }
        }

        private void SetNeighborsEdgeOf(Node node, int value, Node setNode)
        {
            var myEdgeWithIndex = node.Neighbors.WithIndex()
                                                .Where(tuple => tuple.item.node.Position == setNode.Position)
                                                .First();

            node.Neighbors[myEdgeWithIndex.index] = (myEdgeWithIndex.item.node, value);
        }

        private (Node, int) NeighborLeft(int myIndex)
        {
            return (nodes![myIndex - 1], Cost);
        }

        private (Node, int) NeighborRight(int myIndex)
        {
            return (nodes![myIndex + 1], Cost);
        }

        private (Node, int) NeighborDown(int myIndex)
        {
            return (nodes![myIndex + 10], Cost);
        }
        private (Node, int) NeighborUp(int myIndex)
        {
            return (nodes![myIndex - 10], Cost);
        }

    }
}
