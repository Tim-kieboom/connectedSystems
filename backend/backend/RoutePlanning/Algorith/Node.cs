using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.RoutePlanning.Algorith;
public class Node
{
    public Node() { }

    public Node(List<(Node node, int cost)> neighbors)
    {
        Neighbors = neighbors;
    }

    public List<(Node node, int cost)> Neighbors { get; set; } = new();

    public int CostToSource { get; set; } = int.MaxValue;

    public Node? Previous { get; set; }

    public (int x, int y) Position { get; set; }
}
