const debug = document.getElementById("debug");

async function sendCommand()
{
    let botNumber = Number( document.getElementById("select-bot").value );

    let x = Number( document.getElementById("goalX").value );
    let y = Number( document.getElementById("goalY").value );

    if(!isPositionValid(x, y, "sendCommand"))
        return;
    
    const postJsonObj =
    {
        botID:  botNumber,
        currentPosition:
        {
            x:      getBot(botNumber).x, 
            y:      getBot(botNumber).y
        },

        targetPosition:
        {    
            x:     x,
            y:     y
        }
    }
    
    console.log( JSON.stringify(postJsonObj));

    debug.innerHTML = "";
    _ = await post(path = "setTarget", JSON.stringify(postJsonObj));

    getBoard().setGoalForBot(x, y, getBot(botNumber));
}

async function addToQueue()
{
    let pickupX = Number( document.getElementById("pickupX").value );
    let pickupY = Number( document.getElementById("pickupY").value );

    let dropX = Number( document.getElementById("dropX").value );
    let dropY = Number( document.getElementById("dropY").value );

    if(!isPositionValid(pickupX, pickupY, "addToQueue.pickup"))
        return;

    if(!isPositionValid(dropX, dropY, "addToQueue.drop"))
        return;

    const postJsonobj =
    {
        pickup: 
        {
            x: pickupX, 
            y: pickupY
        },
        
        drop:   
        {
            x: dropX, 
            y: dropY
        }
    }
    
    let response = await post(path = "addToQueue", JSON.stringify(postJsonobj));

    let responseJson = await response.json();
    console.log(JSON.stringify(responseJson));

    const botNumber = Number( responseJson.botID );
    console.log(botNumber);

    const queues = responseJson.botQueues;
    const isInQueue = responseJson.isInQueue;
    displayQueues(queues);

    if(isInQueue)
        return;

    const bot = getBot(botNumber);

    getBoard().setPickupForBot(pickupX, pickupY, bot);
    getBoard().setDropForBot(dropX, dropY, bot);
}

function isPositionValid(x, y, label) 
{
    let isValidPostition = true;

    getObsticalPostions().forEach(obstical =>
    {
        if(obstical.x === (x+1) && obstical.y === (y+1))
        {
            isValidPostition = false;
            return;
        }
    });

    if(!isValidPostition)
    {
        debug.innerHTML = label + ": (" + x.toString() + ", " + y.toString() + ") is an invalid position";
        return false;
    }
    return true;
}

function checkIfStart()
{
    if(!getIsStart())
    {
        debug.innerHTML = "press start first";
        return true;
    }

    return false;
}

function showFetchError(label, message)
{
    debug.innerHTML = "fetch error[" + label + "]: " + message;
}

async function getRoutes()
{
    let response = await get("getBotRoutes");
    response = await response.json();

    setRoutes(response.listOfBotRoutes);
}

async function get (path)
{
    const rawResponse = await fetch
        (
            "/api/" + path,
            {
                method: "GET",
                headers:
                {
                    'Content-Type': "application/json"
                },
            }
        )
        .catch(err => showFetchError("get", err));
    return rawResponse;
}

async function post (path, jsonMessage)
{
    const rawResponse = await fetch
        (
            "/api/" + path,
            {
                method: "POST",
                headers:
                {
                    'Content-Type': "application/json"
                },
                body: jsonMessage,
            }
        )
        .catch(err => showFetchError("post", err));
    return rawResponse;
}