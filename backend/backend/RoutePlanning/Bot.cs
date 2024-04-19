using backend.Controllers;
using backend.Controllers.API_Bodys.GetBotPostion;

namespace backend.RoutePlanning
{
    public class Bot
    {
        public Bot(int botID, Position position) 
        { 
            BotID = botID;
            Position = position;
        }

        public int BotID { get; }
        
        public Position Position { get; set; }

        public int routeIndex { get; set; }

        public Queue<List<Position>> RouteQueue { get; set; } = new();

        public Direction Direction { get; set; } = Direction.idle;

        public int DistanceFromSource { get; set; }

        public bool IsRouteQueueEmpty() 
        {
            return !RouteQueue.Any();
        }
    }
}
