const connector = {

    initialize: function (name) {

        // Start the connection.
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl('/game')
            .build();

        this.connection.onclose(function (e) {
            vex.dialog.alert({
                message: 'Connection with server lost!',
                callback: function () {
                    location.reload();
                }
            });
        });

        this.connection.on('newPlayerConnected', function (name) {

            notificationController.PlayerJoinedNotification(name);

        });

        this.connection.on('playerKilled',function(killerAndvictim) {
            notificationController.PlayerKilledNotification(killerAndvictim[0], killerAndvictim[1]);
        });

        this.connection.on('playerDisconnected', function (name) {

            notificationController.PlayerLeftNotification(name);

        });

        this.connection.on('updateLatency', function (sendTime) {

            var d = new Date();
            var n = d.getTime();
            measurementController.UpdatePing(n - sendTime);
        });

        // Create a function that the hub can call to broadcast messages.
        this.connection.on('updateGameState', function (gameState) {
            worldController.OnGameStateReceived(gameState);
        });

        this.connection.on('connectConfirmation', function (response) {

            config = { ...config, ...response.config };

            worldController.OnMapStateReceived(response.mapState);
            weaponService.onWeaponsReceived(response.weapons);

            console.log(`Loaded server configuration (${Object.keys(response.config).length} variables)`);

            worldController.PlayerId = response.playerId;
        });

        this.connection.start()
            .then(function () {
                console.log('connection started');
                connector.onOpen(name);
                // Set up interval (sending player state to server)
                setInterval(connector.connectionInterval, 50);
            });
    },

    onOpen: function (name) {

        connector.connection.invoke('onOpen', name);
    },

    connectionInterval: function () {

        const playerStateString = JSON.stringify({
            Type: "playerstate",
            Keys: keyboardController.GetKeysState(),
            Angle: mouseController.getCurrentAngle(),
            MouseClicked: mouseController.IsMouseClicked(),
            PingStart: new Date().getTime()
        });

        connector.connection.invoke('clientStateUpdate', playerStateString);


        var d = new Date();
        connector.connection.invoke('measureLatency', d.getTime());
    }
};