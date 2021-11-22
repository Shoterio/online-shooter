if (typeof require !== "undefined") {
    PhysicsEngine = require('../Physics/PhysicsEngine.js');
}
let drawExecutionNumber = 0;
let extrapolationTime = 0;
let drawTime = 0;

let drawTimes = [];
let extrapolationTimes = [];
let extrapolationVectorSimilarities = [];

function WorldController() {

    const that = this;

    that.mapObjects = Array();
    that.players = Array();
    that.bullets = Array();
    that.myPlayer = null;

    that.PlayerId = -1;
    that.physicsEngine = new PhysicsEngine(that);

    that.OnMapStateReceived = function (mapstate) {

        for (obj of mapstate.mapObjects) {

            let mapObject;

            if (obj.type === "box") {
                mapObject = new MapRect(obj.position.x, obj.position.y, obj.width, obj.height);
                mapObject.SetTextureId(obj.texture);
            }
            else {
                console.error(`Unknown map object type ${obj.type}`);
            }

            that.mapObjects[obj.id] = mapObject;
        }

        console.log("Loaded map objects " + mapstate.mapObjects.length);
    };

    // Update function is called when gamestate was received from server
    that.OnGameStateReceived = function (gamestate) {

        that.physicsEngine.LastTickDate = new Date();

        let players = Array();
        for (player of gamestate.players) {

            const playerObject = new Player(player.id);
            playerObject.SetPosition(new Vector2(player.position.x, player.position.y));
            playerObject.SetSpeed(new Vector2(player.speed.x, player.speed.y));
            playerObject.SetRadius(player.radius);
            playerObject.SetAngle(player.angle);
            playerObject.SetHealth(player.health);
            playerObject.SetMaxHealth(player.maxHealth);
            playerObject.SetAlive(player.isAlive);

            const foundPlayer = that.players.find(x => x.GetId() === playerObject.GetId());
            if (typeof foundPlayer !== "undefined") {

                const speedVector = playerObject.GetSpeed();

                const previousPosition = Vector2.Subtract(playerObject.GetPosition(), speedVector);
                const extrapolationVector = Vector2.Subtract(foundPlayer.GetPosition(), previousPosition);

                const similarity = Vector2.Similarity(extrapolationVector, speedVector);
                extrapolationVectorSimilarities.push(similarity);
            }

            if (playerObject.GetId() === that.PlayerId) {
                if (!playerObject.IsAlive()) {
                    $('#killScreen').addClass('overlay');
                    $('#killScreen').removeClass('hidden');

                    $('#resurectionTime').html(Math.round(player.milisecondsToResurect / 1000) + " seconds");
                } else {
                    $('#killScreen').removeClass('overlay');
                    $('#killScreen').addClass('hidden');
                    drawingController.SetMyPlayer(playerObject);
                    $('#healthLabel').html(`${player.health}/${player.maxHealth}`);
                }
            }

            players.push(playerObject);
        }
        that.players = players;

        that.bullets = Array();
        for (bullet of gamestate.bullets) {

            const bulletObject = new Bullet(bullet.playerId);
            bulletObject.SetPosition(new Vector2(bullet.position.x, bullet.position.y));
            bulletObject.SetSpeed(new Vector2(bullet.speed.x, bullet.speed.y));
            bulletObject.SetAngle(bullet.angle);
            bulletObject.SetRadius(bullet.radius);

            that.bullets.push(bulletObject);
        }
    };

    that.Draw = function () {

        const drawStart = new Date();
        drawExecutionNumber += 1;

        // Start a new drawing state
        push();

        if (config.extrapolation) {
            const start = new Date();

            that.physicsEngine.ExtrapolatePhysics();

            extrapolationTime += (new Date() - start);
        }

        drawingController.Draw(that.mapObjects, that.players, that.bullets);

        // Restore original state
        pop();

        drawTime += new Date() - drawStart;
        if (drawExecutionNumber === 1000) {
            drawExecutionNumber = 0;

            drawTimes.push(drawTime);
            extrapolationTimes.push(extrapolationTime);

            extrapolationTime = 0;
            drawTime = 0;
        }
    };
}

const worldController = new WorldController();

// Export module
if (typeof module !== 'undefined' && module.hasOwnProperty('exports')) {
    module.exports = worldController;
}