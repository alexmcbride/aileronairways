// Write your JavaScript code.
function disableFormElement(element, disabled) {
    if (disabled) {
        $(element).attr('disabled', 'disabled');
    }
    else {
        $(element).removeAttr("disabled");
    }
}