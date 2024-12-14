$(function () {
    console.log("Page is ready");

    // Attach an event listener to the Save button
    $("#save-game").on("click", function () {
        fetch('/Game/SaveGame', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // CSRF Token
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert(data.message); // Show success message
                } else {
                    alert(data.message); // Show error message
                }
            })
            .catch(error => console.error('Error:', error));
    });

    // Attach an event listener to the entire document for the 'mousedown' event on elements with the class 'game-button'
    $(document).on("mousedown", ".game-button", function (event) {
        var cellNumber = $(this).val();

        switch (event.which) {
            case 1: // Left Mouse Button
                console.log("Cell number " + cellNumber + " was left clicked.");
                doCellUpdate(cellNumber, 'Game/LeftClickShowOneButton');
                break;
            case 2: // Middle Mouse Button
                alert("Middle mouse button clicked");
                break;
            case 3: // Right Mouse Button
                console.log("Cell number " + cellNumber + " was RIGHT clicked.");
                doCellUpdate(cellNumber, 'Game/RightClickShowOneButton');
                break;
            default:
                alert("Nothing was clicked.");
                break;
        }
    });

    // Handle "Load Saved Game" button click
    $(document).on("click", ".load-saved-game", function () {
        var gameId = $(this).data("id"); // Get the game ID from the button's data-id attribute

        $.ajax({
            url: '/Game/LoadSavedGame', // URL of the LoadSavedGame action
            method: 'POST',
            data: { id: gameId }, // Send the game ID to the server
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // CSRF token
            },
            success: function (response) {
                // Replace the game zone with the new gameboard partial view
                $("#game-zone").html(response);
                alert("Game loaded successfully!");
            },
            error: function (xhr, status, error) {
                console.error("Error loading game:", error);
                alert("An unexpected error occurred. Please try again.");
            }
        });
    });

    // Handle Load Saved Games button click using event delegation
    $(document).on("click", "#show-saved-game", function () {
        fetch('/Game/GetSavedGames')
            .then(response => response.text())
            .then(data => {
                $("#saved-games-zone").html(data); // Update the saved games table
            })
            .catch(error => console.error('Error loading saved games:', error));
    });

    // Handle Delete button click
    $(document).on("click", ".delete-game", function () {
        var gameId = $(this).data("id"); // Get the game ID from the button's data-id attribute

        if (confirm("Are you sure you want to delete this game?")) {
            $.ajax({
                url: '/Game/DeleteGameById', // URL of the Delete action
                method: 'POST',
                data: { id: gameId }, // Send the game ID to the server
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // CSRF token
                },
                success: function (response) {
                    if (response.success) {
                        // Remove the deleted game's row from the table
                        $("#game-row-" + gameId).remove();
                        alert(response.message); // Show success message
                    } else {
                        alert(response.message); // Show failure message
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error deleting game:", error);
                    alert("An unexpected error occurred. Please try again.");
                }
            });
        }
    });

    // Prevent the context menu from showing up on right-click
    $(document).bind("contextmenu", function (event) {
        event.preventDefault();
        console.log("Right Click. Prevented context menu");
    });

    // Define the function to update a cell
    function doCellUpdate(cellNumber, urlString) {
        $.ajax({
            datatype: "html", // Expect HTML or JSON response from the server
            method: "POST",   // Use POST method
            url: urlString,   // Endpoint URL
            data: { "cellNumber": cellNumber }, // Send cellNumber to the server
            success: function (data) {
                if (data.redirectUrl) {
                    window.location.href = data.redirectUrl;
                } else if (data.includes("id=\"game-zone\"")) {
                    $("#game-zone").html(data); // Update the entire game board
                } else {
                    $("#" + cellNumber).html(data); // Update a specific cell
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX request failed:", error);
            }
        });
    }
}); // End Main Function
