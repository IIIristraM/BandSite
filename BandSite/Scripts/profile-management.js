//depends on controls.js

var player;

$(function () {
    generateChat();
});

function replaceSimbols(string) {
    string = string.replace(/'/, "_");
    string = string.replace(/\./, "_");
    string = string.replace(/\s/, "_");
    string = string.replace(/@/, "_");
    return string;
}

function generateChat() {
    $.ajax({
        url: "/Account/GetUserslist",
        type: "GET",
        cache: false
    }).done(function (usersList) {
        var html = "";
        for (var i = 0; i < usersList.length; i++) {
            html = html +
                   "<li class='user-list-item ui-state-default' data-user-name='" + replaceSimbols(usersList[i].name) + "'>" +
                        "<div class='float-left'>" + 
                             "<span>" +
                                   usersList[i].name +
                             "</span>" +
                        "</div>" +
                        "<div class='indicator'></div>" +
                        "<div class='clear-fix'></div>" +
                    "</li>";
        }
        $(".user-list").html(html);
        if (usersList.length > 0) {
            $("#user-name").val(usersList[0].name + "#");
            $(".user-list").find(".user-list-item").first().addClass("user-list-item-highlight");
        }
        $(".user-list-item").click(function () {
            var username = $(this).find("span").html();
            if (!$(this).hasClass("user-list-item-highlight")) {
                var names = $("#user-name").val();
                $("#user-name").val(names + username + "#");
                $(this).addClass("user-list-item-highlight");
            }
            else {
                var names = $("#user-name").val();
                names = names.replace(username + "#", "");
                $("#user-name").val(names);
                $(this).removeClass("user-list-item-highlight");
            }
        });
        
        chat = $.connection.chatHub;

        chat.client.addMessage = function (user, message) {
            $("#msg-list").append("<li class='msg-list-item'><span><b>" + user + " :</b><br>" + message + "</span></li>");
        };

        chat.client.onOnline = function (usernames) {
            for (var i = 0; i < usernames.length; i++) {
                $(".user-list").find(".user-list-item[data-user-name=" + replaceSimbols(usernames[i]) + "]").find(".indicator").addClass("user-list-item-online");
            }
        };

        chat.client.onOffline = function (username) {
            $(".user-list").find(".user-list-item[data-user-name=" + replaceSimbols(username) + "]").find(".indicator").removeClass("user-list-item-online");
        };

        $.connection.hub.start();

        $("#send-btn").click(function () {
            var message = $("#message-txt").val();
            var user = $("#user-name").val();
            $("#msg-list").append("<li class='msg-list-item'><span><b>Me :</b><br>" + message + "</span></li>");
            chat.server.send(user, message);
            $("#message-txt").val("");
        });
    });
}




