var player;

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
});

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
    });
}

function AutocompleteConfig() {
    $("input[data-autocomplete-source]").each(function () {
        var target = $(this);
        var entity = target.attr("data-entity-type");
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
            open: function (event, ui) {
                wasSelected = false;
            }
        });
    });
}



