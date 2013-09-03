var player;
var chat;

$(function () {
    $("#settings-tabs").tabs();
    AutocompleteConfig();
    GeneratePlayer();

    $("#playlist-tab").find(".listen-btn").click(function () {
        if (player !== undefined) {
            var id = $("#playlist-tab").find("#add_song_id").val();
            if (id !== "") {
                var url = "/AdministrativeTools/Song/GetStream/" + id;
                var title = $("#playlist-tab").find("#add_song_title").val();
                var $player = $("#" + player.id);
                var $audio = $player.find("audio");
                $audio.attr("src", url);
                $player.find(".song-control-panel").attr("data-song-guid", generateGUID());
                $player.find(".song-title").empty();
                $player.find(".song-title").html(title);
                if ($player.find(".play-btn").attr("data-state") === "play") {
                    player.playSong($player.find(".play-btn").get(0));
                }
                else {
                    $audio.get(0).play();
                }
            }
        }
    });

    $("#playlist-tab").find(".populate-btn").click(function () {
        var id = $("#playlist-tab").find("#add_song_id").val();        
        if ((id !== "") && (player !== undefined)) {
            var title = $("#playlist-tab").find("#add_song_title").val();
            var url = "/AdministrativeTools/Song/GetStream/" + id;
            var guid;
            if (player.getAudio().attr("data-song-guid") !== undefined) guid = player.getAudio().attr("data-song-guid");
            player.addPlaylistItem(title, url, guid);
            player.bindPlayList();
            $.ajax({
                url: "/AdministrativeTools/Song/PopulateUsersPlaylist?userId=-1&songId=" + id,
                cache: false,
                type: "POST"
            });
        }
    });    

    GenerateChat();
});

function ReplaceSimbols(string) {
    string = string.replace(/'/, "_");
    string = string.replace(/\./, "_");
    string = string.replace(/\s/, "_");
    string = string.replace(/@/, "_");
    return string;
}

function GenerateChat() {
    $.ajax({
        url: "/Account/GetUserslist",
        type: "GET",
        cache: false
    }).done(function (usersList) {
        var html = "";
        for (var i = 0; i < usersList.length; i++) {
            html = html +
                   "<li class='user-list-item ui-state-default' data-user-name='" + ReplaceSimbols(usersList[i].name) + "'>" +
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
                $(".user-list").find(".user-list-item[data-user-name=" + ReplaceSimbols(usernames[i]) + "]").find(".indicator").addClass("user-list-item-online");
            }
        };

        chat.client.onOffline = function (username) {
            alert(username);
            $(".user-list").find(".user-list-item[data-user-name=" + ReplaceSimbols(username) + "]").find(".indicator").removeClass("user-list-item-online");
        };

        $.connection.hub.start().done(function () {
            chat.server.register();
        });

        $("#send-btn").click(function () {
            var message = $("#message-txt").val();
            var user = $("#user-name").val();
            $("#msg-list").append("<li class='msg-list-item'><span><b>Me :</b><br>" + message + "</span></li>");
            chat.server.send(user, message);
            $("#message-txt").val("");
        });
        
        window.onbeforeunload = function () {
            chat.server.logout();
            //return "offline!";
        };
    });
}

function GeneratePlayer() {
    $.ajax({
        url: "/Account/GetPlaylist/",
        cache: false,
        type: "POST"
    }).done(function (playlist) {
        var options = {
            playlist: playlist
        };
        player = $("#player").audioPlayer(options);
   
        player.removePlaylistItemExtend = function (guid) {
            var url = $("#player").find(".play-list").find(".play-list-item[data-song-guid=" + guid + "]").attr("data-song-url");
            var id = url.substr(url.lastIndexOf("/") + 1);
            $.ajax({
                url: "/AdministrativeTools/Song/RemoveFromUsersPlaylist?userId=-1&songId=" + id,
                cache: false,
                type: "POST"
            });
        };

        player.updatePlaylistOrder = function (event, ui) {
            var url = ui.item.attr("data-song-url");
            var id = url.substr(url.lastIndexOf("/") + 1);
            var $player = $("#" + this.id);
            var $palylist = $player.find(".play-list");
            var order = $palylist.find(".play-list-item").index($(".play-list-item[data-song-guid=" + ui.item.attr("data-song-guid") + "]").get(0)) + 1;
            $.ajax({
                url: "/Account/ReorderPlaylist?songId=" + id + "&order=" + order,
                type: "POST",
                cache: false
            });
        };
    });
}

function AutocompleteConfig() {
    $("input[data-autocomplete-source]").each(function () {
        var target = $(this);
        var relatedEntity = target.attr("data-related-entity-type");
        var wasSelected = false;
        target.unbind("focusout");
        target.focusout(function () {
            if (!wasSelected) {
                $("#add_" + relatedEntity + "_title").val("");
                $("#add_" + relatedEntity + "_id").val("");
            }
        });
        target.unbind("autocomplete");
        target.autocomplete({
            source: target.attr("data-autocomplete-source"),
            select: function (event, ui) {
                wasSelected = true;
                $("#add_" + relatedEntity + "_title").val(ui.item.label);
                $("#add_" + relatedEntity + "_id").val(ui.item.value);
                return false;
            },
            open: function () {
                wasSelected = false;
            }
        });
    });
}



