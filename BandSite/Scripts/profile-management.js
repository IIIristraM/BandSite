//depends on controls.js

var chat;

$(function () {
    $("#chat").chat();
});

function replaceSimbols(string) {
    string = string.replace(/'/, "_");
    string = string.replace(/\./, "_");
    string = string.replace(/\s/, "_");
    string = string.replace(/@/, "_");
    return string;
}





