$(function () {
    console.log("Page is ready");

    // Attach an event listener to the Save button
    $("#save-game").on("click", function () {
        // Send a POST request to save the game
        fetch('/Game/SaveGame', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Specify JSON data format
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Include CSRF Token for security
            }
        })
            .then(response => response.json())
            .then(data => {
                // Display appropriate alert message based on success or failure
                if (data.success) {
                    alert(data.message);
                } else {
                    alert(data.message);
                }
            })
            .catch(error => console.error('Error:', error)); // Log any errors
    });

    // Attach an event listener to the document for mousedown on game buttons
    $(document).on("mousedown", ".game-button", function (event) {
        var cellNumber = $(this).val(); // Get the cell number from the button value

        switch (event.which) {
            case 1: // Left Mouse Button
                console.log("Cell number " + cellNumber + " was left clicked.");
                doCellUpdate(cellNumber, 'Game/LeftClickShowOneButton'); // Handle left click
                break;
            case 2: // Middle Mouse Button
                alert("Middle mouse button clicked"); // Handle middle click
                break;
            case 3: // Right Mouse Button
                console.log("Cell number " + cellNumber + " was RIGHT clicked.");
                doCellUpdate(cellNumber, 'Game/RightClickShowOneButton'); // Handle right click
                break;
            default:
                alert("Nothing was clicked."); // Handle other clicks
                break;
        }
    });

    // Handle "Load Saved Game" button click
    $(document).on("click", ".load-saved-game", function () {
        var gameId = $(this).data("id"); // Get game ID from button's data attribute

        $.ajax({
            url: '/Game/LoadSavedGame', // Endpoint to load saved game
            method: 'POST',
            data: { id: gameId }, // Send game ID to server
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // CSRF Token for security
            },
            success: function (response) {
                // Update the game zone with the loaded game
                $("#game-zone").html(response);
                alert("Game loaded successfully!");
            },
            error: function (xhr, status, error) {
                console.error("Error loading game:", error); // Log error
                alert("An unexpected error occurred. Please try again.");
            }
        });
    });

    // Handle "Show Saved Games" button click
    $(document).on("click", "#show-saved-game", function () {
        // Fetch saved games and update the saved games zone
        fetch('/Game/GetSavedGames')
            .then(response => response.text())
            .then(data => {
                $("#saved-games-zone").html(data); // Display saved games
            })
            .catch(error => console.error('Error loading saved games:', error)); // Log errors
    });

    // Handle "Delete Game" button click
    $(document).on("click", ".delete-game", function () {
        var gameId = $(this).data("id"); // Get game ID from button's data attribute

        // Confirm deletion before proceeding
        if (confirm("Are you sure you want to delete this game?")) {
            $.ajax({
                url: '/Game/DeleteGameById', // Endpoint to delete game
                method: 'POST',
                data: { id: gameId }, // Send game ID to server
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // CSRF Token
                },
                success: function (response) {
                    if (response.success) {
                        // Remove the game's row from the table and show success message
                        $("#game-row-" + gameId).remove();
                        alert(response.message);
                    } else {
                        alert(response.message); // Show failure message
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error deleting game:", error); // Log error
                    alert("An unexpected error occurred. Please try again.");
                }
            });
        }
    });

    // Prevent the default context menu from appearing on right-click
    $(document).bind("contextmenu", function (event) {
        event.preventDefault(); // Disable right-click context menu
        console.log("Right Click. Prevented context menu");
    });

    // Define the function to update a cell
    function doCellUpdate(cellNumber, urlString) {
        $.ajax({
            datatype: "html", // Specify expected response type
            method: "POST",   // Use POST method for the request
            url: urlString,   // Specify the endpoint URL
            data: { "cellNumber": cellNumber }, // Send the cell number to server
            success: function (data) {
                if (data.redirectUrl) {
                    // Redirect if a URL is provided in the response
                    window.location.href = data.redirectUrl;
                } else if (data.includes("id=\"game-zone\"")) {
                    // Update the entire game zone if relevant data is returned
                    $("#game-zone").html(data);
                } else {
                    // Update the specific cell with returned data
                    $("#" + cellNumber).html(data);
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX request failed:", error); // Log errors
            }
        });
    }
}); // End Main Function
