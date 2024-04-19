namespace backend.RoutePlanning.Algorith
{
    public class Planner
    {
        private readonly PriorityQueue<Node, int /*distanceFromSource*/> priorityQueue = new();

        public void PlanDijkstra(Node[,] graph, (int x, int y) sourcePosition)
        {
            Node sourceNode = graph[sourcePosition.x, sourcePosition.y];

            Setup(sourceNode);

            while (priorityQueue.Count > 0)
            {
                Node highScoreNode;
                int highScore_DistanceFromSource;

                bool hasResult = priorityQueue.TryDequeue(out highScoreNode!, out highScore_DistanceFromSource);

                if (!hasResult)
                    continue;

                CalulateDijkstra(highScoreNode, highScore_DistanceFromSource);
            }
        }

        private void CalulateDijkstra(Node highScoreNode, int highScore_DistanceFromSource)
        {
            for (int i = 0; i < highScoreNode.Neighbors.Count; i++)
            {
                (Node node, int edgeCost) /*edge*/ = highScoreNode.Neighbors[i];

                int newDistance = highScore_DistanceFromSource + edgeCost;

                if (newDistance < node.CostToSource && edgeCost >= 0)
                {
                    node.Previous = highScoreNode;
                    node.CostToSource = newDistance;

                    priorityQueue.Enqueue(node, node.CostToSource);
                }
            }
        }

        private void Setup(Node sourceNode)
        {
            //normaly CostToSource is innit to int.maxValue(number closest to inf)
            sourceNode.CostToSource = 0;

            if (priorityQueue.Count != 0)
                priorityQueue.Clear();

            priorityQueue.Enqueue(sourceNode, 0);
        }


    }
}
