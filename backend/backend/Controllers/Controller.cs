using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using backend.RoutePlanning;
using backend.Controllers.API_Bodys.GetBotPostion;
using backend.Controllers.API_Bodys.NextPosition;
using backend.Controllers.API_Bodys.SendTarget;
using backend.Controllers.API_Bodys.AddToQueue;
using System.Text;
using backend.Controllers.API_Bodys.toggleMoveBots;
using backend.Controllers.API_Bodys.GetFirst5Queues;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api")]
    public class Controller : ControllerBase
    {
        private UpdatePositions updatePositions;

        private readonly ILogger<Controller> _logger;

        public Controller(ILogger<Controller> logger, UpdatePositions updatePositions)
        {
            _logger = logger;
            this.updatePositions = updatePositions;
        }

        //(dashboard) sets Target for bot and calulates route
        [HttpPost("setTarget")]
        public ActionResult SetTarget(SendTargetRequestBody request)
        {
            LogBody("Post", "setTarget", request);

            List<Position> route = updatePositions.PlanTarget(request.CurrentPosition!, request.TargetPosition!);
            updatePositions.AddRoute(request.BotID, route);


            string routeMessage = RouteToString(route, request.BotID);
            LogBody("Post", "setTarget-getRoute", routeMessage);

            return Ok("{ message: \""+routeMessage+"\" }");
        }

        [HttpGet("getBotPositions")]
        public ActionResult GetBotPostion()
        {
            _logger.LogInformation("get");
            var body = new GetBotPositionResponseBody();

            Bot[] bots = updatePositions.Bots;

            for (int id = 0; id < bots.Length; id++)
            {
                Direction direction = bots[id].Direction;
                Position position = bots[id].Position;

               body.BotPositions.Add(new CurrentBotPosition(id, direction, position));
            }

            return Ok(body);
        }

        [HttpPost("addToQueue")]
        public ActionResult AddToQueue(AddToQueueResquestBody request)
        {
            LogBody("Post", "addToQueue", request);

            if (request.Pickup == null || request.Drop == null)
                return BadRequest();

            (Bot pickedBot, List<Position> route, bool isInQueue) = updatePositions.PickBotForQueue(request.Pickup, request.Drop);


            string message = RouteToString(route, pickedBot.BotID);
            message = string.Format("\n\tisInQueue: {0}", isInQueue) + message;

            LogBody("Post", "queue", message);

            return Ok(new AddToQueueResponseBody()
            {
                BotID = pickedBot.BotID,
                IsInQueue = isInQueue
            });
        }

        [HttpPost("nextPosition")]
        public ActionResult NextPosition(NextPositionRequestBody request)
        {
            string httpType = "Post";
            string URLFinalPath = "nextPosition";

            LogBody(httpType, URLFinalPath, request);

            int botID = request.BotID;

            if (request.NextPosition == null)
            {
                string errorMessage = "nextPosition is null";
                LogError(httpType, URLFinalPath, errorMessage);
                return BadRequest(errorMessage);
            }

            //updatePositions.botPositions[botID].CurrentPosition = request.NextPosition!;

            return Ok();
        }

        [HttpPost("getFirst5Queues")]
        public ActionResult GetQueues()
        {
            return Ok(new GetFirst5QueuesResponseBody() { Queues = GetStringOfFirst5Queued()});
        }

        private List<string> GetStringOfFirst5Queued()
        {
            int[] queuesCount = updatePositions.Bots.Select(bot => bot.RouteQueue.Count).ToArray();

            var firstQueued = updatePositions.Bots.Where(bot => bot.RouteQueue.Any())
                                                   .Select(bot => (bot.RouteQueue.First(), bot.BotID))
                                                   .ToArray();

            string[] stringList = new string[4];

            foreach ( (var queue, int botID) in firstQueued)
            {
                if (queue == null)
                {
                    stringList[botID] = "empty";
                    continue;
                }

                Position first = queue.First();
                Position last = queue.Last();


                stringList[botID] = string.Format("(x: {0}, y: {1}) -> (x: {2}, y: {3})", first.X, first.Y, last.X, last.Y);

                if (queuesCount[botID] > 1)
                    stringList[botID] += " ...";
            }

            for(int i = 0; i < 4; i++)
                if (stringList[i] == null)
                    stringList[i] = "empty";

            return stringList.ToList();
        }

        [HttpPost("toggleMoveBots")]
        public ActionResult Toggle(ToggleMoveBotsRequestBody request) 
        {
            LogBody("Post", "toggleMoveBots", request);

            updatePositions.SetIsMoveBotsOn(request.Mode);

            return Ok();
        }

        private static string RouteToString(List<Position> route, int botID)
        {
            StringBuilder sb = new(string.Format("\n\tbotID: {0} \n\troute: ", botID));

            foreach ( (Position position, int index) in route.WithIndex())
            {
                sb.Append(string.Format("(x: {0}, y: {1})", position.X, position.Y));

                if (index == route.Count-1)
                    return sb.ToString();

                sb.Append(" -> ");
            }

            return sb.ToString();
        }

        private void LogError(string httpType, string URLFinalPath, string error)
        {
            string dateTime = DateTime.Now.ToString();
            string message = string.Format("[Http{0}]{1}: {2} \ntime: {3}", httpType, URLFinalPath, error, dateTime);

            _logger.LogError(message);
        }

        private void LogBody(string httpType, string URLFinalPath, object body)
        {
            string dateTime = DateTime.Now.ToString();

            string requestString;


            if (body.GetType() == typeof(string))
            {
                requestString = (string)body;
            }
            else
            {
                requestString = "\t" + JsonSerializer.Serialize(body) ?? "null";
            }

            string message = string.Format("[Http{0}]{1}: {2} \ndate: {3}", httpType, URLFinalPath, requestString, dateTime);

            _logger.LogInformation(message);
        }

    }
}