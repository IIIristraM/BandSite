//do not used
//depends on controls.js

function replaceSimbols(string) {
    string = string.replace(/'/, "_");
    string = string.replace(/\./, "_");
    string = string.replace(/\s/, "_");
    string = string.replace(/@/, "_");
    return string;
}





