// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showLabel(button) {
    // Prevent the form from submitting immediately to allow the label to be shown
    event.preventDefault();

    // Find the label inside the clicked button and remove the hidden class
    const label = button.querySelector('.button-label');
    label.classList.remove('hidden-label');

    // Optionally submit the form after revealing the label
    // button.closest("form").submit();
}