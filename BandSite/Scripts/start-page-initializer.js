$(function GenerateAdminToolMenuButton() {
    $(".admin-tools-button").wrap("<a href='/administrativetools/navigation'/>");
});

$(function GenerateRegisterButton() {
    $("#loginLink").empty();
    $("#loginLink").append("<div class='login-button'></div>");
});

$(function GenerateRegisterButton() {
    $("#registerLink").empty();
    $("#registerLink").append("<div class='register-button'></div>");
});

$(function GenerateRegisterButton() {
    $("#logoutLink").empty();
    $("#logoutLink").append("<div class='logout-button'></div>");
});

$(function DisplayUserName() {
    $(".username").empty();
    var username = $(".username").attr("username");
    $(".username").append("<div class='user-name-area'>" + username + "</div>");
});