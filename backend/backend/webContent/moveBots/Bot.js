const offset = 7;

const right = 0;
const up = 1;
const left = 2;
const down = 3;
const idle = 4;


const dotsInfo =
[
    {x: 0,       y: -offset, isLedOn: false}, //up
    {x: 0,       y: offset,  isLedOn: false}, //down
    
    {x: -offset, y: 0,       isLedOn: false}, //left
    {x: offset,  y: 0,       isLedOn: false}, //right
]
class Bot
{
    constructor(x, y, canvas, botNumber)
    {
        this.botNumber = botNumber;

        this.canvas = canvas;
        this.ctx = this.canvas.getContext("2d");

        this.x = x;
        this.y = y;

        this.dotsInfo = copyObjectArray(dotsInfo);

        this.direction = idle;
    }

    drawBot(checkerBoard)
    {
        const tileX = this.getCenterOfTileX(checkerBoard);
        const tileY = this.getCenterOfTileY(checkerBoard);
    
        const radius = 15;

        this.ctx.beginPath();
        this.ctx.arc(tileX, tileY, radius, 0, Math.PI * 2);
        this.ctx.fillStyle = this.getBotColor();
        this.ctx.fill();

        this.dotMoveLogic();
        for (let i = 0; i < 4; i++)
        {
            this.drawDot(this.dotsInfo[i], checkerBoard);
        }

    }

    drawDot(dotInfo, checkerBoard)
    {
        const tileX = this.getCenterOfTileX(checkerBoard);
        const tileY = this.getCenterOfTileY(checkerBoard);

        const radiusDot = 3;

        this.ctx.beginPath();
        this.ctx.arc(tileX+dotInfo.x, tileY+dotInfo.y, radiusDot, 0, Math.PI * 2);
        this.ctx.fillStyle = (dotInfo.isLedOn) ? this.getDotOnColor() : "white";
        this.ctx.fill();
    }

    dotMoveLogic()
    {
        switch(this.direction)
        {
            case idle:
                this.setDotsOff();
                break;

            case right:
                this.setDotOn(3/*right*/);
                break;

            case left:
                this.setDotOn(2/*left*/);
                break;

            case down:
                this.setDotOn(1/*down*/);
                break;

            case up:
                this.setDotOn(0/*up*/);
                break;

            default:
                console.error("route has nextposition.x > 1 | < -1 botNumber: " + this.botNumber);
        }
    }

    getCenterOfTileX(checkerBoard)
    {
        const tileSize = checkerBoard.tileSize;

        //checkerBoard is first index of 1 soo ++ to compensate
        const boardX = this.x + 1;

        return (boardX * tileSize) + (tileSize / 2);
    }

    getCenterOfTileY(checkerBoard)
    {
        const tileSize = checkerBoard.tileSize;

        //checkerBoard is first index of 1 soo ++ to compensate
        const boardY = this.y + 1;

        return (boardY * tileSize) + (tileSize / 2);
    }

    getBotColor()
    {
        switch(this.botNumber)
        {
            case 0:
                return "red";

            case 1:
                return "darkgreen";

            case 2:
                return "darkorange";  

            case 3:
                return "blue";
        }
    }

    getDotOnColor()
    {
        switch(this.botNumber)
        {
            case 0:
                return "yellow";

            case 1:
                return "yellow";

            case 2:
                return "red";  

            case 3:
                return "red";
        }
    }

    setDotsOff()
    {
        this.dotsInfo.forEach(dots =>
        {
            dots.isLedOn = false; 
        });
    }

    setDotOn(botNumber)
    {
        this.setDotsOff();
        this.dotsInfo[botNumber].isLedOn = true;
    }
}

function copyObjectArray(array)
{
    return array.map(Element => {return{...Element}});
}