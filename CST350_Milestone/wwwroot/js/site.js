// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    console.log("Page is ready");

    // JavaScript
    // Attach an event listener to the entire document for the 'mousedown' event
    // on elements with the class 'game-button'
    $(document).on("mousedown", ".game-button", function (event) {

        // Get the value of the clicked button
        var cellNumber = $(this).val();

        // Use a switch statement to check with mouse button was clicked
        switch (event.which) {
            // Left Mouse Button
            case 1:
                // Log the button number to the console
                console.log("Cell number " + cellNumber + "  was left clicked.");
                // Call the function 'doButtonUpdate' with the button number and a specific endpoint
                doCellUpdate(cellNumber, 'Game/ShowOneButton');
                break;
            // Middle Mouse Button
            case 2:
                alert("Middle mouse button clicked");
                break;
            // Right Mouse Button
            case 3:
                // Log the button number to the console
                console.log("Cell number " + cellNumber + "  was RIGHT clicked.");
                // Call the function 'doButtonUpdate' with the button number and a specific endpoint
                doCellUpdate(cellNumber, 'Game/RightClickShowOneButton');
                break;
            default:
                alert("Nothing was clicked.");
                break;
        }
    });

    // Bind a new event to the context menu to prevent the right-click menu
    // from appearing
    // Whenever the context menu shows up call this fuction
    $(document).bind("contextmenu", function (event) {
        // Stop the right click menuu from showing up
        event.preventDefault();
        console.log("Right Click. Prevented context menu");
    });

    // Define the function 'doButtonUpdate' that takes two parameters: 'buttonNumber' and 'urlString'
    function doCellUpdate(cellNumber, urlString) {
        // Make an AJAX request using jQuery
        $.ajax({
            // Set the expeected data type to 'json' for the responese
            datatype: "json",
            // Use the 'POST'method for the request
            method: "POST",
            // Set the url for the request using the value passed in 
            url: urlString,
            // Send data to the server, specificly the "buttonNumber as a key-value pair"
            data: { "cellNumber": cellNumber },
            // Define a callback function to handle a successful response
            success: function (data) {
                // Log the response to the console
                console.log(data);
                // Update the HTML content of the element with ID
                $("#" + cellNumber).html(data);
            }
        });
    }
}); //End Main Function

// Save Game function
function saveGame() {
    const gameData = JSON.stringify(getGameState());
    fetch('/Game/SaveGame', {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ gameData: gameData })
    }).then(response => response.json())
        .then(data => response.json())
        .then(data => {
            if (data.success) {
                alert('Game saved successfully!')
            }
        });
}

// Load Game function
function loadGame() {
    fetch('Game/LoadGame')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                setGameState(JSON.parse(data.data));
            }
            else {
                alert('No saved game found.');
            }
        });
}
