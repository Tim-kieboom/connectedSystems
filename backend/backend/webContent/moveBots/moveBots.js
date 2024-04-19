const canvas = document.getElementById("CheckerBoard-Canvas");
const ctx = canvas.getContext("2d");

var updateBotsIntervals;
var isStart = false;

//routes[4] that contains route = [{x: 0, y: 0}]
var routes = [];

var obsticalPostions = 
[
    {x: 2, y: 2}, {x: 3, y: 2}, {x: 4, y: 2},
    {x: 2, y: 4}, {x: 3, y: 4}, {x: 4, y: 4},

    {x: 7, y: 2}, {x: 8, y: 2}, {x: 9, y: 2},
    {x: 7, y: 4}, {x: 8, y: 4}, {x: 9, y: 4},


    {x: 2, y: 7}, {x: 3, y: 7}, {x: 4, y: 7},
    {x: 2, y: 9}, {x: 3, y: 9}, {x: 4, y: 9},

    {x: 7, y: 7}, {x: 8, y: 7}, {x: 9, y: 7},
    {x: 7, y: 9}, {x: 8, y: 9}, {x: 9, y: 9},
];

const obsticals = new Obsticals("brown", obsticalPostions);

var board = new CheckerBoard(obsticals, canvas);

const robots = 
[
    new Bot(x = 0, y = 0, canvas, botNumber = 0),
    new Bot(x = 9, y = 0, canvas, botNumber = 1),
    new Bot(x = 0, y = 9, canvas, botNumber = 2),
    new Bot(x = 9, y = 9, canvas, botNumber = 3)
];

function drawRobots() 
{
    robots.forEach((bot, botNumber) => 
    {
        bot.drawBot(board, botNumber);
    });
}

function updateBotsInfo()
{
    const botLocation = document.getElementById('botsLocation');
    
    for(let i = 0; i < 4; i++)
    {
        const bot = robots[i];
        botLocation.querySelector('#locationBot' + (i+1).toString() + ' a').innerHTML = "( " + bot.x + ", " + bot.y + " )";
    }
}

function innitialize()
{   
    const botLocation = document.getElementById('botsLocation');

    for(let i = 0; i < 4; i++)
    {
        const bot = robots[i];
        botLocation.querySelector('#locationBot' + (i+1).toString() + ' b').style.color = bot.getBotColor();
    }

    board.drawMap();
    drawRobots();
}
async function getPositions()
{
    const response = await get("getBotPositions");
    const json = await response.json();
    return json;
}

function updateBots(botsData)
{
    robots.forEach((bot, index) =>
    {
        const botData = Object.values(botsData.botPositions)[index];
        bot.botNumber = botData.botID;
        
        bot.x = botData.currentPosition.x;
        bot.y = botData.currentPosition.y;
        
        bot.direction = botData.direction;
        
        const pickup = board.getPickupForBot(bot);
        const goal   = board.getGoalForBot(bot);
        const drop   = board.getDropForBot(bot);

        if(equalPosition(bot, goal))
            board.removeGoalForBot(bot);

        if(equalPosition(bot, pickup))
            board.removePickupForBot(bot);

        if(equalPosition(bot, drop))
            board.removeDropForBot(bot);
    });
} 

async function updateMoveBots() 
{
    const botsData = await getPositions();
    console.log(botsData);

    updateBots(botsData);

    board.drawMap();

    updateBotsInfo();
    drawRobots();

    displayQueues();
}

function toggleUpdate()
{
    const button = document.getElementById('start-button');
    if(!isStart) 
    {
        isStart = true;

        button.style.backgroundColor = "green";
        button.style.borderColor = "green";
        button.innerHTML = 'STOP';

        _ = post("toggleMoveBots", "{\"mode\": "+true+"}");
    }
    else
    {
        isStart = false;

        button.style.backgroundColor = "red";
        button.style.borderColor = "red";
        button.innerHTML = 'START';

        _ = post("toggleMoveBots", "{\"mode\": "+false+"}");
    }
}

async function displayQueues()
{
    const response = await get("getFirst5Queues");
    const responseJson = response.json();
    const queues = responseJson.queues;

    const botLocation = document.getElementById('pickUpQueue');
    if(queues[0] == undefined || queues[0] == null)
    {
        for(let i = 0; i < 4; i++)
        {
            botLocation.querySelector('#botQueue' + (i+1).toString() + ' a').innerHTML = "empty";
        }
    }

    for(let i = 0; i < queues.length; i++)
    {
        botLocation.querySelector('#botQueue' + (i+1).toString() + ' a').innerHTML = queues[i];
    }
}

function equalPosition(bot, position)
{
    const doesExist = (position !== null && position !== undefined);
    if(!doesExist)
        return false;

    return bot.x === position.x-1 && bot.y === position.y-1;
}

function setRoute(newRoute, index)
{
    routes[index] = newRoute;
}

function setRoutes(newRoutes)
{
    routes = newRoutes;
    console.log(routes);
}

function getBoard()
{
    return board;
}

function getRobots()
{
    return robots;
}

function getBot(index)
{
    return robots[index];
}

function getObsticalPostions()
{
    return obsticalPostions;
}

function getIsStart()
{
    return isStart;
}

setInterval(updateMoveBots, 200/*ms*/);
innitialize();