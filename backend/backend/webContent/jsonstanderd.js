/*
!!this js file is not code but documentation for the jsonStanderd between webots and the server in this project!!
    values are for the example

variables explained(if needed):
    botID = id of the bot (bot1 = 0, bot2 = 1, bot3 = 2, bot4 = 3).
    direction = the ddirection the bot is facing (up = 0, down = 1, left = 2, right = 3, idle = 4).

    botPosition = the current position of all the bots (bot1 = [0], bot2 = [1], bot3 = [2], bot4 = [3]).
    nextPosition = the next position the bot has to move to.
    targePosition = the position the bots wants to move to.

    detectedObsticals = the obsticals that are detected by the bots sensors.
*/

//(send order) server -> webots
[methode = post, path = "api/sendTarget"]
sendTargetJson_request =
{
    botID: 0,
    targetPosition:
    {
        x: 0,
        y: 0
    }
}
//what the server wants back from webots
sendTargetJson_repsonse =
{
    botID:             0,
    currentPosition:   {x: 0, y: 0},

    direction:         0,
    detectedObsticals: 
    [
        {x: 1, y: 1},
        {x: 2, y: 1},
        {x: 3, y: 1}
    ]
};

//(send order)webots -> server -> dashboard, hardware
[methode = post, path = "api/nextPosition"]
nextPositionJson =
{
    botID:             0,
    nextPosition:       {x: 0, y: 0},
    targetPosition:     {x: 4, y: 4},


    direction:         0,
    detectedObsticals: 
    [
        {x: 1, y: 1},
        {x: 2, y: 1},
        {x: 3, y: 1}
    ]
};

//(send order)server -> webots
[methode = get, path = "api/getBotPositions"]
getBotPositionsJson =
{
    botPositions:
    [
        {
            botID: 0, 
            direction: 0,
            currentPosition: {x: 0, y: 0}
        },
        {
            botID: 1, 
            direction: 0,
            currentPosition: {x: 0, y: 9}
        },        
        {
            botID: 2, 
            direction: 0,
            currentPosition: {x: 9, y: 0}
        },        
        {
            botID: 3, 
            direction: 0,
            currentPosition: {x: 9, y: 9}
        },
    ]
};

//Queue for dashboard
AddToQueueResponseBody =
{
    botID: 0, //for color of pickup drop in forntend
    botQueues:
    [
        "bot1: (x: 0, y: 0) -> (x: 4, y: 4)",
        "bot1: (x: 4, y: 4) -> (x: 4, y: 9)",
        "bot2: (x: 0, y: 9) -> (x: 4, y: 9)"
    ]
}
