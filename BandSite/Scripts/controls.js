var player;

function autocompleteConfig() {
    $("input[data-autocomplete-source]").each(function () {
        var target = $(this);
        var relatedEntity = target.attr("data-related-entity-type");
        var wasSelected = false;
        target.unbind("focusout");
        target.focusout(function () {
            if (!wasSelected) {
                target.parent().find("#add_" + relatedEntity + "_title").val("");
                target.parent().find("#add_" + relatedEntity + "_id").val("");
            }
        });
        target.unbind("autocomplete");
        target.autocomplete({
            source: target.attr("data-autocomplete-source"),
            appendTo: "#searchResults",
            open: function () {
                wasSelected = false;
            }
        });
        target.on("autocompleteselect", function (event, ui) {
            wasSelected = true;
            target.parent().find("#add_" + relatedEntity + "_title").val(ui.item.label);
            target.parent().find("#add_" + relatedEntity + "_id").val(ui.item.value);
            return false;
        });
    });
}

function preventDropdownClose(e) {
    e.stopPropagation();
}

function configNavbar() {
    $(".nav.navbar-nav li").click(function () {
        $(".nav.navbar-nav").find(".active").removeClass("active");
        $(this).addClass("active");
    });
    $.ajax({        
        url: "/home/playlist",
        type: "GET",
        cache: false
    }).done(function (content) {
        
        $("#playlist-tab").attr("title", content);
        $('#playlist-tab').tooltip();
        $('#playlist-tab').tooltip("show");
        $('#playlist-tab').parent().find(".tooltip").css("display", "none");
        generatePlayer();
        autocompleteConfig();
        
        $('#playlist-tab').click(function () {
            if ($(this).parent().find(".tooltip").css("display") != "none") {
                $(this).parent().find(".tooltip").css("display", "none");
            } else {
                $(this).parent().find(".tooltip").css("display", "block");
            }
        });
    });
}

function generatePlayer() {
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
                url: "/Song/RemoveFromUsersPlaylist?userId=-1&songId=" + id,
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

        var playerpanel = $("#playlist-tab").parent();
        
        playerpanel.find(".listen-btn").click(function () {
            if (player !== undefined) {
                var id = playerpanel.find("#add_song_id").val();
                if (id !== "") {
                    var url = "/Song/GetStream/" + id;
                    var title = playerpanel.find("#add_song_title").val();
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

        playerpanel.find(".populate-btn").click(function () {
            var id = playerpanel.find("#add_song_id").val();
            if ((id !== "") && (player !== undefined)) {
                var title = playerpanel.find("#add_song_title").val();
                var url = "/Song/GetStream/" + id;
                var guid;
                if (player.getAudio().attr("data-song-guid") !== undefined) guid = player.getAudio().attr("data-song-guid");
                player.addPlaylistItem(title, url, guid);
                player.bindPlayList();
                $.ajax({
                    url: "/Song/PopulateUsersPlaylist?userId=-1&songId=" + id,
                    cache: false,
                    type: "POST"
                });
            }
        });
    });
}

$(function() {
    configNavbar();
});

