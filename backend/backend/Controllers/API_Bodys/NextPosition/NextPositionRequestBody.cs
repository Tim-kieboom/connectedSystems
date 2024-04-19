namespace backend.Controllers.API_Bodys.NextPosition
{
    public class NextPositionRequestBody
    {
        public int BotID { get; set; }
        public Position? NextPosition { get; set; }
        public Position? TargetPosition { get; set; }
        public int Direction { get; set; }
        public List<Position>? DetectedObjects { get; set; }
    }
}
