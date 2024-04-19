using backend.Controllers;
using backend.RoutePlanning.Algorith;
using System.Collections.Generic;

namespace backend.RoutePlanning
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }

    public class Plan
    {
        public static Graph GraphBuilder { get; } = new();

        public Planner Planner { get; set; }      = new();

        private readonly List<Position> Walls     = new();

        public Node[,] Graph
        {
            get 
            {
                return GraphBuilder.MakeGraph()!;
            } 
        }


        public void PlanDijkstra(Node[,] graph, Position source)
        {
            Planner.PlanDijkstra(graph, (source.X, source.Y));
        }

        public List<Position> PlanToTarget(Node[,] graph, Position source, Position target)
        {
            Planner.PlanDijkstra(graph, (source.X, source.Y));

            return GetRoute(graph, target);
        }

        public List<Position> GetRoute(Node[,] graph, Position target)
        {
            int dummyDistance = 0;

            return GetRoute(graph, target, ref dummyDistance);
        }

        public List<Position> GetRoute(Node[,] graph, Position target, ref int distance)
        {
            Node targetNode = graph[target.X, target.Y];
            Node? currentNode = targetNode;

            distance = targetNode.CostToSource;


            List<Position> route = new()
            {
                new (targetNode.Position.x, targetNode.Position.y)
            };

            while (currentNode?.Previous != null)
            {
                currentNode = currentNode.Previous;

                (int cX, int cY) = currentNode.Position;

                route.Add(new Position(cX, cY));
            }

            route.Reverse();

            return route;
        }

        public (Bot, List<Position>) GiveABotPickUpRoute(Node[,] graph, List<Bot> bots, Position?[] endOfRouteBotPositions, Position pickup, Position drop)
        {
            PlanDijkstra(graph, pickup);

            Node[] botTargetNodes = GetBotTargetNodes(graph, bots, endOfRouteBotPositions);
            foreach((Bot bot, int index) in bots.WithIndex())
            {
                Node targetNode = botTargetNodes[index];

                bot.DistanceFromSource = targetNode?.CostToSource ?? int.MaxValue;
            }

            Bot bestBot = PickBestBot(bots);
            List<Position> route = GetFullRoute(graph, bestBot, endOfRouteBotPositions, drop);

            return (bestBot, route);
        }

        private List<Position> GetFullRoute(Node[,] graph, Bot bestBot, Position?[] endOfRouteBotPositions, Position drop)
        {
            List<Position> botToPickup = GetRoute(graph, endOfRouteBotPositions[bestBot.BotID] ?? bestBot.Position);
            botToPickup.Reverse();

            List<Position> pickUpToDrop = GetRoute(graph, drop);
            
            if(botToPickup.Count >= 1)
                pickUpToDrop.RemoveAt(0);

            List<Position> botToPickupToDrop = botToPickup.Concat(pickUpToDrop).ToList();
            return botToPickupToDrop;
        }

        private static Node[] GetBotTargetNodes(Node[,] graph, List<Bot> bots, Position?[] endOfRouteBotPositions)
        {
            Node[] nodes = new Node[4];

            foreach( (Bot bot, int index) in bots.WithIndex() )
            {
                Position? position = endOfRouteBotPositions[index];

                //if route is empty
                if (position == null)
                {
                    Node nodeOfCurrentBotPosision = graph[bot.Position.X, bot.Position.Y];
                    bot.DistanceFromSource = nodeOfCurrentBotPosision.CostToSource;
                    continue;
                }

                Node nodeOfBot = graph[position.X, position.Y];
                nodes[index] = nodeOfBot;
            }

            return nodes;
        }

        public void AddWalls(Node[,] graph, List<Position> walls)
        {
            walls.ForEach(wall => AddWall(graph, wall));
        }

        public void AddWall(Node[,] graph, Position wall)
        {
            if (ContainsListPosition(Walls, wall))
                return;

            Walls.Add(wall);

            SetWallsInGraph(graph);
        }
        private void SetWallsInGraph(Node[,] graph)
        {
            Walls.ForEach(wall =>
            {
                GraphBuilder.BlockPosition(graph, (wall.X, wall.Y));
            });
        }

        public void ClearAllWalls(Node[,] graph)
        {
            Walls.ForEach(wall =>
            {
                GraphBuilder.UnBlockPostion(graph, (wall.X, wall.Y));
            });

            Walls.Clear();
        }

        public void ClearWalls(Node[,] graph, List<Position> walls)
        {
            walls.ForEach(wall => ClearWall(graph, wall));
        }

        public bool ClearWall(Node[,] graph, Position wall)
        {
            int index = Walls.IndexOf(wall);

            if (index == -1)
                return false;

            Walls.RemoveAt(index);
            GraphBuilder.UnBlockPostion(graph, (wall.X, wall.Y));

            return true;
        }

        private static Bot PickBestBot(List<Bot> bots)
        {
            int smallestQueue = bots.Min(bot => bot.RouteQueue.Count);

            Bot bestBot = bots.Where(bot => bot.RouteQueue.Count <= smallestQueue)
                              .OrderBy(bot => bot.DistanceFromSource)
                              .First();

            return bestBot;
        }

        private static bool ContainsListPosition(List<Position> list, Position position)
        {
            return list.Any(el => el.X == position.X && el.Y == position.Y);
        }

    }
}
