using backend.Controllers;
using backend.RoutePlanning;
using System.Timers;
using System;
using backend.Controllers.API_Bodys.GetBotPostion;
using backend.RoutePlanning.Algorith;
using System.Reflection;
using backend.Controllers.API_Bodys.toggleMoveBots;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace backend
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }

    public class UpdatePositions
    {
        private System.Timers.Timer? timer;
        private readonly ILogger<UpdatePositions> logger;

        private readonly object locker = new();

        public Bot[] Bots { get; private set; }
        
        public Position[] PreviousPositions { get; private set; } = new Position[4];

        public List<Position>?[] BotCurrentRoutes { get; private set; } = new List<Position>[4];

        public Plan RoutePlanner { get; private set; } = new();

        public bool IsMoveBotsOn { get; private set; } = false;

        /*
         x: 0, y: 0
         x: 9, y: 0
         x: 0, y: 9
         x: 9, y: 9         
        */

        public UpdatePositions(ILogger<UpdatePositions> logger)
        {

            TestObstacles();

            Bots = new Bot[4]
            {
                new Bot(0, new Position(x: 0, y: 0)),
                new Bot(1, new Position(x: 9, y: 0)),
                new Bot(2, new Position(x: 0, y: 9)),
                new Bot(3, new Position(x: 9, y: 9))
            };

            //TestColistion();

            this.logger = logger;
            SetUpdateTimer(300/*ms*/);

        }

        public List<Position> PlanTarget(Position current, Position target)
        {
            lock (locker)
            {
                return RoutePlanner.PlanToTarget(RoutePlanner.Graph, current, target);
            }
        }

        public void SetIsMoveBotsOn(bool toggle)
        {
            lock (locker)
            {
                IsMoveBotsOn = toggle;
            }
        }

        public void AddRoute(int botID, List<Position> route)
        {
            lock (locker)
            {
                Bots[botID].RouteQueue.Enqueue(route);
            }
        }

        public void SetUpdateTimer(int time)
        {
            // Create a timer with time millieSecond interval.
            timer = new System.Timers.Timer(time);

            // Hook up the Elapsed event for the timer. 
            timer.Elapsed += UpdateCurrentPositions;

            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public (Bot chosenBot, List<Position> route, bool isInQueue) PickBotForQueue(Position pickup, Position drop)
        {
            bool isInQueue = true;

            Position?[] endOfRouteBotPositions = new Position[4];

            foreach ((List<Position>? myRoute, int index) in BotCurrentRoutes.WithIndex()) 
            {
                if (myRoute == null)
                {
                    endOfRouteBotPositions[index] = Bots[index].Position;
                    continue;
                }

                endOfRouteBotPositions[index] = myRoute.Last();
            }

            (Bot bestBot, List<Position> route) = RoutePlanner.GiveABotPickUpRoute(RoutePlanner.Graph, Bots.ToList(), endOfRouteBotPositions, pickup, drop);

            if (BotCurrentRoutes[bestBot.BotID] == null)
            {
                if (bestBot.RouteQueue.Count <= 1)
                    isInQueue = false;
            }
            else
            {
                if(!bestBot.RouteQueue.Any())
                    isInQueue = false;
            }

            AddRoute(bestBot.BotID, route);

            return (bestBot, route, isInQueue);
        }

        private void TestObstacles()
        {
            var graph = RoutePlanner.Graph;

            RoutePlanner.AddWalls(graph, new List<Position> 
            {
                new(1, 1),  new(6, 1),
                new(2, 1),  new(7, 1),
                new(3, 1),  new(8, 1),
                            
                new(1, 3),  new(6, 3),
                new(2, 3),  new(7, 3),
                new(3, 3),  new(8, 3),
                            
                            
                new(1, 6),  new(6, 6),
                new(2, 6),  new(7, 6),
                new(3, 6),  new(8, 6),
                            
                new(1, 8),  new(6, 8),
                new(2, 8),  new(7, 8),
                new(3, 8),  new(8, 8)
            });   
        }

        private void TestCollision()
        {
            List<Position> beginPositions = new() 
            {
                new(x: 0, y: 7),
                new(x: 0, y: 9),
                new(x: 1, y: 9),
                new(x: 3, y: 9)
            };

            foreach(var (bot, index) in Bots.WithIndex())
            {
                bot.Position = beginPositions[index];
            }

            /*
             * x: 0, y: 7
             * x: 0, y: 9
             * x: 1, y: 9
             * x: 3, y: 9
             */

            Position[] targets = new Position[4]
            {
                new(0, 0),//9
                new(5, 9),
                new(0, 8),
                new(9, 9)
            };

            foreach (var (bot, index) in Bots.WithIndex())
            {
                BotCurrentRoutes[index] = RoutePlanner.PlanToTarget(RoutePlanner.Graph, bot.Position, targets[index]);
            }
        }

        private void UpdateCurrentPositions(object? source, ElapsedEventArgs TimerArgs)
        {
            lock (locker)
            {
                List<Position> nextPositions = new()
                {
                    Bots[0].Position,
                    Bots[1].Position,
                    Bots[2].Position,
                    Bots[3].Position
                };

                GetBotCurrentRoute();

                //stops bots from updating their positions
                if (!IsMoveBotsOn)
                    return;

                var notEmptyIndexes = BotCurrentRoutes.WithIndex()
                                                     .Where(tuple => tuple.item != null)
                                                     .Where(tuple => tuple.item!.Any())
                                                     .Select(tuple => tuple.index);

                foreach (int index in notEmptyIndexes)
                {
                    var routesWithIndex = BotCurrentRoutes.Select(route => route?.WithIndex());
                    int? routeIndex;

                    try
                    {
                        routeIndex = routesWithIndex.ElementAt(index)?.ElementAt(Bots[index].routeIndex).index;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        BotCurrentRoutes[index] = null;
                        continue;
                    }

                    if (routeIndex == null)
                        continue;

                    PreviousPositions[index] = Bots[index].Position;

                    if (routeIndex >= BotCurrentRoutes[index]!.Count - 1)
                    {
                        nextPositions[index] = Bots[index].Position;
                        continue;
                    }

                    nextPositions[index] = BotCurrentRoutes[index]!.ElementAt((int)routeIndex + 1);

                }

                var botsCollisions = GetCollisions(nextPositions);

                bool[] moveBots = new bool[4]
                {
                    true,
                    true,
                    true,
                    true
                };

                if (botsCollisions.Any())
                    moveBots = ReRouteBots(nextPositions, botsCollisions);

                foreach(var (canMove, index) in moveBots.WithIndex()) 
                {
                    if (canMove)
                    {
                        Bots[index].Position = nextPositions[index];
                        Bots[index].routeIndex++;
                    }

                    SetBotDirection(Bots[index]);
                }

            }
        }

        private bool[] ReRouteBots(List<Position> nextPositions, IEnumerable<int> botsIncollission)
        {
            var moveBotArray = new bool[4]
            {
                false,
                false,
                false,
                false
            };

            foreach (int botIndex in botsIncollission)
            {
                (int distance, List<Position> newRoute) = ReRoute(nextPositions, botIndex);

                if (distance != int.MaxValue)
                {
                    BotCurrentRoutes[botIndex] = newRoute;

                    moveBotArray[botIndex] = true;
                    SetBotDirection(Bots[botIndex]);
                    return moveBotArray;
                }
            }

            foreach(var (_, index) in Bots.WithIndex())
            {
                if (botsIncollission.Contains(index))
                    continue;

                (int distance, List<Position> newRoute) = ReRoute(nextPositions, index);

                if (distance != int.MaxValue)
                {
                    BotCurrentRoutes[index] = newRoute;
                    Bots[index].routeIndex = 1;

                    moveBotArray[index] = true;
                }
            }

            return moveBotArray;
        }

        private (int distance, List<Position> route) ReRoute(List<Position> nextPositions, int botIndex)
        {
            if (BotCurrentRoutes[botIndex] == null || !BotCurrentRoutes[botIndex]!.Any())
                return (int.MaxValue, new());

            var graph = RoutePlanner.Graph;

            var botObstacles = Bots.Where(bot => bot.BotID != botIndex)
                                   .Select(bot => bot.Position)
                                   .ToList();

            RoutePlanner.AddWalls(graph, botObstacles);

            Position target = BotCurrentRoutes[botIndex]![^1];
            var newRoute = RoutePlanner.PlanToTarget(graph, Bots[botIndex].Position, target);

            RoutePlanner.ClearWalls(graph, botObstacles);

            int distance = graph[target.X, target.Y].CostToSource;
            return (distance, newRoute);
        }

        private void GetBotCurrentRoute()
        {
            var queuedBotIndexes = Bots.Select((bot, index) => (bot, index))
                                      .Where(tuple  => !tuple.bot.IsRouteQueueEmpty())
                                      .Select(tuple => tuple.index);

            foreach (int index in queuedBotIndexes)
            {
                Bot bot = Bots[index];

                if (BotCurrentRoutes[index] == null)
                {
                    BotCurrentRoutes[index] = bot.RouteQueue.Dequeue();
                    bot.routeIndex = 0;
                    continue;
                }

                if (bot.routeIndex == BotCurrentRoutes[index]?.Count-1)
                {
                    BotCurrentRoutes[index] = bot.RouteQueue.Dequeue();
                    bot.routeIndex = 0;
                }
            }
        }

        private IEnumerable<int> GetCollisions(List<Position> nextPositions)
        {
            Dictionary<Position, int> frontCollisions = new();

            // same position
            var sameGridCollisions = nextPositions.Select((position, index) => (position, index))
                                                  .GroupBy(tuple => tuple.position)
                                                  .Where(group => group.Count() > 1)
                                                  .SelectMany(group => group);

            foreach (var (position, index) in sameGridCollisions)
            {
                frontCollisions[position] = index;
            }

            // both current are both previous
            foreach(var (item, index) in nextPositions.WithIndex())
            {
                Position position = item;
                int positionIndex = index;

                for(int botIndex = 0; botIndex <  Bots.Length; botIndex++)
                {
                    if (botIndex == index)
                        continue;

                    if (position == PreviousPositions[botIndex] && nextPositions[botIndex] == PreviousPositions[positionIndex])
                    {
                        frontCollisions[position] = positionIndex;
                        frontCollisions[nextPositions[botIndex]] = botIndex;
                    }     
                }
            }

            return frontCollisions.Select(kvp => kvp.Value);
        }

        private void SetBotDirection(Bot bot)
        {
            Position next = PreviousPositions[bot.BotID] ?? bot.Position;
            Position current = bot.Position;

            bot.Direction = GetDirection(current, next);
        }

        private static Direction GetDirection(Position current, Position next)
        {
            int xDifference = current.X - next.X;
            int yDifference = current.Y - next.Y;

            if (xDifference == 0 && yDifference == 0)
                return Direction.idle;

            switch (xDifference)
            {
                case 1:
                    return Direction.right;

                case -1:
                    return Direction.left;

                default:
                    break;
            }

            switch (yDifference)
            {
                case 1:
                    return Direction.down;

                case -1:
                    return Direction.up;

                default:
                    break;
            }

            return Direction.idle;
        }

        private static bool IsPositionTheSame(Position one, Position two)
        {
            return one.X == two.X && one.Y == two.Y;
        }
    }
}
