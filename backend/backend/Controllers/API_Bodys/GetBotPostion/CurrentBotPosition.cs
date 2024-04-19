using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace backend.Controllers.API_Bodys.GetBotPostion
{
    public enum Direction
    {
        right = 0,
        up = 1,
        left = 2,
        down = 3,
        idle = 4
    }

    public class CurrentBotPosition
    {
        public CurrentBotPosition(int botID, Direction direction, Position position)
        {
            BotID = botID;
            Direction = (int)direction;
            CurrentPosition = position;
        }

        public int BotID { get; set; }
        public int Direction { get; set; }
        public Position CurrentPosition { get; set; } = new Position();
    }
}
