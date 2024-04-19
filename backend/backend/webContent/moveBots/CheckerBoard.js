class Obsticals
{
    constructor(color, positions/*array of {x: 0, y: 0}*/)
    {
        this.color = color;
        this.positions = positions;
    }
}

class CheckerBoard
{
    constructor(obsticals, canvas)
    {
        this.canvas = canvas;
        this.ctx = this.canvas.getContext("2d");

        this.mapSize = 12;
        this.tileSize = this.canvas.width / this.mapSize;

        this.obsticals = obsticals;

        this.goals   = [null, null, null, null];
        
        this.pickups = [null, null, null, null];
        this.drops   = [null, null, null, null];
    }

    fillTile(x, y, tileColor, text)
    {
        const tileSize = this.tileSize;

        this.ctx.fillStyle = tileColor;
        this.ctx.fillRect(x * tileSize, y * tileSize, tileSize, tileSize);
        this.ctx.strokeStyle = "darkgrey";
        this.ctx.strokeRect(x * tileSize, y * tileSize, tileSize, tileSize);
        
        if(text !== undefined)
        {
            this.setTextInTile(x, y, text);
        }
    }

    setTextInTile(x, y, text)
    {
        const tileSize = this.tileSize;

        this.ctx.fillStyle = "black";
        this.ctx.font = "bold 10px Arial";
        this.ctx.textAlign = "center";
        this.ctx.textBaseline = "middle";
        this.ctx.fillText(text, x * tileSize + tileSize / 2, y * tileSize + tileSize / 2);
    }

    addGridAndGridNumber(x, y)
    {
        let tileColor = "lightgray";
        let textInBlock = undefined;

        if(x === 0 && y === 0)
        {
            textInBlock = "#";
        }
        else if(x === 0 && y < 11)
        {
            textInBlock = (y-1).toString();
        }
        else if(y === 0 && x < 11)
        {
            textInBlock = (x-1).toString();
        }
        else if(x < 11 && y < 11)
        {
            tileColor = this.getTileColor(x, y);
        }

        this.fillTile(x, y, tileColor, textInBlock);
    }

    drawMap() 
    {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

        for (let x = 0; x < this.mapSize; x++) 
        {
            for (let y = 0; y < this.mapSize; y++) 
            {
                this.addGridAndGridNumber(x, y);
            }
        }
    
        const obsicalPositions = this.obsticals.positions;
        const obsticalColor = this.obsticals.color;

        this.fillSpectialTiles(this.goals,   "Here");

        this.fillSpectialTiles(this.drops,   "Drop");
        this.fillSpectialTiles(this.pickups, "Pickup");
    
        obsicalPositions.forEach(obstical => 
        {
            this.fillTile(obstical.x, obstical.y, obsticalColor);
        });

    }

    fillSpectialTiles(TileTypeList, text)
    {
        TileTypeList.forEach(TileType =>
        {
            if(TileType !== null)
                this.fillTile(TileType.x, TileType.y, TileType.color, text);
        });
    }

    getCenterOfTileX(tileX)
    {
        const tileSize = this.tileSize;

        return (tileX * tileSize) + (tileSize / 2);
    }

    getCenterOfTileY(tileY)
    {
        const tileSize = this.tileSize;

        return (tileY * tileSize) + (tileSize / 2);
    }

    setGoalForBot(x, y, bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        this.goals[bot.botNumber] = {x: ++x, y: ++y, color: bot.getBotColor()}; 
    }

    setPickupForBot(x, y, bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        this.pickups[bot.botNumber] = {x: ++x, y: ++y, color: bot.getBotColor()}; 
    }

    setDropForBot(x, y, bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        this.drops[bot.botNumber] = {x: ++x, y: ++y, color: bot.getBotColor()}; 
    }

    getGoalForBot(bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        return this.goals[bot.botNumber]; 
    }

    getPickupForBot(bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        return this.pickups[bot.botNumber]; 
    }

    getDropForBot(bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        return this.drops[bot.botNumber]; 
    }

    removeGoalForBot(bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        this.goals[bot.botNumber] = null; 
    }

    removePickupForBot(bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        this.pickups[bot.botNumber] = null; 
    }

    removeDropForBot(bot)
    {
        //checkerBoard is first index of 1 soo ++ to compensate
        this.drops[bot.botNumber] = null; 
    }

    getTileColor(x, y)
    {
        return (isEven(x + y)) ? "gray" : "lightyellow";
    }
}

function isEven(number)
{
    return (number % 2) === 0;
}