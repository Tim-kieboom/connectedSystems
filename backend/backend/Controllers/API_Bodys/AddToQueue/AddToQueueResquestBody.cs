namespace backend.Controllers.API_Bodys.AddToQueue
{
    public class AddToQueueResquestBody
    {
        public Position? Pickup { get; set; }
        public Position? Drop { get; set; }
    }
}
