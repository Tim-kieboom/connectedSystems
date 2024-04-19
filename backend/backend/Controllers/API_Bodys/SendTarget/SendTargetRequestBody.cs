namespace backend.Controllers.API_Bodys.SendTarget
{
    public class SendTargetRequestBody
    {
        public int BotID { get; set; }
        public Position? CurrentPosition { get; set; }
        public Position? TargetPosition { get; set; }
    }
}