document.addEventListener("DOMContentLoaded", function () {
    
    function bindConnectionMessage(connection) {
        var powerStatusChangedCallback = function (sender, args) {
            if (args.IsOn) {
                $(document.getElementById("light-bulb")).addClass("is-on").removeClass("is-off");
            } else {
                $(document.getElementById("light-bulb")).addClass("is-off").removeClass("is-on");
            }

            var logArea = document.getElementById("log-area");
            logArea.value += "Light " + (args.IsOn ? "ON" : "OFF") + ": " + args.Notes + "\r\n";
            logArea.scrollTop = logArea.scrollHeight;
        };

        // Create a function that the hub can call to broadcast messages.
        connection.on("powerStatusChanged", powerStatusChangedCallback);

        connection.onclose(onConnectionError);
    }

    function onConnected(connection) {
        console.log("connection started");

        document.getElementById("toggle-light-bulb-with-signalr").addEventListener("click", function (event) {
            connection
                .invoke("toggleLightBulb")
                .catch(e => alert(e.message));

            event.preventDefault();
        });

        document.getElementById("toggle-light-bulb-with-api").addEventListener("click", function (event) {

            $.post("api/toggle-light-bulb");

            event.preventDefault();
        });
    }

    function onConnectionError(error) {
        if (error && error.message) {
            console.error(error.message);
        }
        var modal = document.getElementById("myModal");
        modal.classList.add("in");
        modal.style = "display: block;";
    }

    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/lightBulb')
        .build();

    bindConnectionMessage(connection);
    connection.start()
        .then(function () {
            onConnected(connection);
        })
        .catch(function (error) {
            if (error) {
                if (error.message) {
                    console.error(error.message);
                }
            }
        });
});