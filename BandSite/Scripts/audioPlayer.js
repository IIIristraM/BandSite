function getTime(seconds) {
    var min = Math.floor(seconds / 60);
    var sec = Math.floor(seconds - min * 60);
    var normalizedSec;
    (sec < 10) ? normalizedSec = "0" + sec : normalizedSec = sec;
    if ((min + ":" + normalizedSec).indexOf("NaN") !== -1) return "0:00";
    return min + ":" + normalizedSec;
}

function generateGUID() {
    var d = new Date().getTime();
    var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x7 | 0x8)).toString(16);
    });
    return guid;
}

function AudioPlayer(options) {
    this.id = options.id;
    this.playlist = options.playlist;
    for (var i = 0; i < this.playlist.length; i++) this.playlist[i].url = "/AdministrativeTools/Song/GetStream/" + this.playlist[i].id;
    this.defaultGUID = generateGUID();
    this.currentVolume = 1.0;
}

AudioPlayer.prototype.removePlaylistItemExtend = function (guid) { 
};

AudioPlayer.prototype.removePlaylistItem = function (guid) {
    this.removePlaylistItemExtend(guid);
    var $player = $("#" + this.id);
    var $palylist = $player.find(".play-list");
    if (guid === $player.find(".song-control-panel").attr("data-song-guid")) {
        this.goNext();
    }
    $player.find(".play-list").find(".play-list-item[data-song-guid=" + guid + "]").remove();
    for (var i = 0; i < this.playlist.length; i++) {
        if (this.playlist[i].guid === guid) {
            this.playlist.splice(i, 1);
        }
    }
    if (this.playlist.length === 0) {
        this.getAudio().attr("src", "");
        $player.find(".song-title").empty();
        $player.find(".song-title").html("No songs in list");
    }
};

AudioPlayer.prototype.addPlaylistItem = function (title, url, guid) {
    if ($(".play-list-item[data-song-url='" + url + "']").length === 0) {
        var $player = $("#" + this.id);
        title = title.replace(/'/, "&#8217;");
        if (guid === undefined) {
            if (this.playlist.length !== 0) {
                guid = generateGUID();
            }
            else {
                guid = this.defaultGUID;
            }
        }
        this.playlist[this.playlist.length] = {
            url: url,
            title: title,
            guid: guid
        };
        var $palylist = $player.find(".play-list").append(
            "<li class='play-list-item ui-state-default' data-song-url='" + url + "' data-song-guid='" + guid + "' data-song-title='" + title + "'>" +
                 "<span>" +
                      title +
                 "</span>" +
                 "<div class='remove-item'></div>" +
            "</li>"
        );
    }
};

AudioPlayer.prototype.generatePlaylistMarkup = function () {
    var guid = this.defaultGUID;
    var audiolist = "";
    for (var i = 0; i < this.playlist.length; i++) {
        this.playlist[i].title = this.playlist[i].title.replace(/'/, "&#8217;");
        if (i > 0) guid = generateGUID();
        this.playlist[i].guid = guid;
        audiolist = audiolist +
                    "<li class='play-list-item ui-state-default' data-song-url='" + this.playlist[i].url + "' data-song-guid='" + guid + "' data-song-title='" + this.playlist[i].title + "'>" +
                        "<span>" +
                            this.playlist[i].title +
                        "</span>" +
                        "<div class='remove-item'></div>" +
                    "</li>";
    }
    return audiolist;
};

AudioPlayer.prototype.generateMarkup = function () {
    var _this = this;
    $("#" + this.id).empty();
    var audiolist = this.generatePlaylistMarkup();
    var title = "No songs in list";
    var id = "";
    if (this.playlist.length > 0) {
        id = this.playlist[0].id;
        title = this.playlist[0].title;
    }
    $("#" + this.id).html(
       "<audio src='/AdministrativeTools/Song/GetStream/" + id + "'></audio>" +
       "<div class='song-control-panel' data-song-guid='" + this.defaultGUID + "'>" +
           "<hr>" +
           "<div class='song-title'>" + title + "</div>" +
           "<div class='time-line'>" +
                "<div class='time-slider'></div>" +
                "<div class='time-label'></div>" +
           "</div>" +
           "<div class='back-btn'></div>" +
           "<div class='play-btn' data-state='play'></div>" +
           "<div class='forward-btn'></div>" +
           "<div class='loop-btn' data-state='once'></div>" +
           "<div class='volume-btn' data-state='normal'></div>" +
           "<div class='volume-slider'></div>" +
           "<div style='clear:both; visibility:hidden;'></div>" +
           "<hr>" +
       "</div>" +
       "<br><br>" +
       "<ul class='play-list'>" +
           audiolist +
       "</ul>"
    );
    $("#" + this.id).find(".play-list").sortable({
        update: function (event, ui) {
            _this.defaultGUID = $("#" + _this.id).find(".play-list-item").first().attr("data-song-guid");
            _this.updatePlaylistOrder(event, ui);
        }
    });
};

AudioPlayer.prototype.updatePlaylistOrder = function () {
};

AudioPlayer.prototype.getAudio = function () {
    var $player = $("#" + this.id);
    return $player.find("audio");
};

AudioPlayer.prototype.setPrevSong = function () {
    var $player = $("#" + this.id);
    var $palylist = $player.find(".play-list");
    var prevSongGuid = $palylist.find(".play-list-item[data-song-guid=" + $player.find(".song-control-panel").attr("data-song-guid") + "]").prev(".play-list-item").attr("data-song-guid");
    if (prevSongGuid === undefined) {
        return false;
    }
    else {
        var prevsongurl = $palylist.find(".play-list-item[data-song-guid=" + $player.find(".song-control-panel").attr("data-song-guid") + "]").prev(".play-list-item").attr("data-song-url");
        var prevsongtitle = $palylist.find(".play-list-item[data-song-guid=" + $player.find(".song-control-panel").attr("data-song-guid") + "]").prev(".play-list-item").attr("data-song-title");
        this.getAudio().attr("src", prevsongurl);
        $player.find(".song-control-panel").attr("data-song-guid", prevSongGuid);
        $player.find(".song-title").empty();
        $player.find(".song-title").html(prevsongtitle);
        return true;
    }
};

AudioPlayer.prototype.setNextSong = function () {
    var $player = $("#" + this.id);
    var $palylist = $player.find(".play-list");
    var nextSongGuid = $palylist.find(".play-list-item[data-song-guid=" + $player.find(".song-control-panel").attr("data-song-guid") + "]").next(".play-list-item").attr("data-song-guid");
    if (nextSongGuid === undefined) {
        this.resetPlaylist();
        return false;
    }
    else {
        var nextsongurl = $palylist.find(".play-list-item[data-song-guid=" + $player.find(".song-control-panel").attr("data-song-guid") + "]").next(".play-list-item").attr("data-song-url");
        var nextsongtitle = $palylist.find(".play-list-item[data-song-guid=" + $player.find(".song-control-panel").attr("data-song-guid") + "]").next(".play-list-item").attr("data-song-title");
        this.getAudio().attr("src", nextsongurl);
        $player.find(".song-control-panel").attr("data-song-guid", nextSongGuid);
        $player.find(".song-title").empty();
        $player.find(".song-title").html(nextsongtitle);
        return true;
    }
};

AudioPlayer.prototype.resetPlaylist = function () {
    var $audio = this.getAudio();
    var $player = $("#" + this.id);
    $audio.attr("src", $player.find(".play-list-item[data-song-guid=" + this.defaultGUID + "]").attr("data-song-url"));
    $player.find(".song-control-panel").attr("data-song-guid", this.defaultGUID);
    $player.find(".song-title").empty();
    $player.find(".song-title").html($player.find(".play-list-item[data-song-guid=" + this.defaultGUID + "]").attr("data-song-title"));
    $player.find(".time-slider").slider("value", 0);
};

AudioPlayer.prototype.playSong = function (btn) {
        var $btn = $(btn);
        var $audio = this.getAudio();
        if ($btn.attr("data-state") === "play") {
            if (this.getAudio().get(0).readyState !== 0) {
                $audio.get(0).play();
                $btn.attr("data-state", "pause");
            }
        }
        else if ($btn.attr("data-state") === "pause") {
            $audio.get(0).pause();
            $btn.attr("data-state", "play");
        }
};

AudioPlayer.prototype.checkLoop = function () {
    var $audio = this.getAudio();
    var $player = $("#" + this.id);
    var $loopbtn = $player.find(".loop-btn");
    var $playbtn = $player.find(".play-btn");
    if ($loopbtn.attr("data-state") === "once") {
        if (this.setNextSong()) {
            if ($player.find(".play-btn").attr("data-state") === "pause") {
                $audio.get(0).play();
            }
            else {
                this.playSong($playbtn.get(0));
            }
        }
        else {
            this.resetPlaylist();
            this.playSong($playbtn.get(0));
        }
    }
    else if ($loopbtn.attr("data-state") === "songloop") {
        if ($player.find(".play-btn").attr("data-state") === "pause") {
            $audio.get(0).play();
        }
        else {
            this.playSong($playbtn.get(0));
        }
    }
    else if ($loopbtn.attr("data-state") === "listloop") {
        if (!this.setNextSong()) {
            this.resetPlaylist();
        }
        if ($player.find(".play-btn").attr("data-state") === "pause") {
            $audio.get(0).play();
        }
        else {
            this.playSong($playbtn.get(0));
        }
    }
};

AudioPlayer.prototype.changeLoopState = function (btn) {
    var $btn = $(btn);
    if ($btn.attr("data-state") === "once") {
        $btn.attr("data-state", "songloop");
    }
    else if ($btn.attr("data-state") === "songloop") {
        if (this.playlist.length > 1) {
            $btn.attr("data-state", "listloop");
        }
        else {
            $btn.attr("data-state", "once");
        }
    }
    else if ($btn.attr("data-state") === "listloop") {
        $btn.attr("data-state", "once");
    }
};

AudioPlayer.prototype.renderProgress = function () {
    var $player = $("#" + this.id);
    var $audio = this.getAudio();
    var $slider = $player.find(".time-slider");
    if (($slider.attr("data-enabled") === undefined) || ($slider.attr("data-enabled") === "true")) {
        var $time = $player.find(".time-label");
        var currTime = $audio.get(0).currentTime;
        var duration = $audio.get(0).duration;
        if (!isNaN(duration)) {
            $slider.slider("value", currTime / duration * 100);
        }
        $time.empty();
        $time.append(getTime($audio.get(0).duration - $audio.get(0).currentTime));
    }
};

AudioPlayer.prototype.checkMute = function (btn) {
    var $btn = $(btn);
    var $player = $("#" + this.id);
    if ($btn.attr("data-state") === "normal") {
        $btn.attr("data-state", "mute");
        this.getAudio().get(0).volume = 0;
        $player.find(".volume-slider").slider("value", 0);
    }
    else if ($btn.attr("data-state") === "mute") {
        $btn.attr("data-state", "normal");
        this.getAudio().get(0).volume = this.currentVolume;
        $player.find(".volume-slider").slider("value", this.currentVolume * 100);
    }
};

AudioPlayer.prototype.goNext = function () {
    var $player = $("#" + this.id);
    if (this.playlist.length > 0) {
        if (this.setNextSong()) {
            if ($player.find(".play-btn").attr("data-state") === "pause") this.getAudio().get(0).play();
        }
        else {
            if ($player.find(".play-btn").attr("data-state") === "pause") this.playSong($player.find(".play-btn").get(0));
        }
    }
};

AudioPlayer.prototype.goPrev = function () {
    var $player = $("#" + this.id);
    if (this.playlist.length > 0) {
        if (this.setPrevSong()) {
            if ($player.find(".play-btn").attr("data-state") === "pause") this.getAudio().get(0).play();
        }
    }
};

AudioPlayer.prototype.bindPlayList = function () {
    var $player = $("#" + this.id);
    var _this = this;
    $player.find(".play-list-item").unbind("click");
    $player.find(".play-list-item").click(function () {
        var $audio = _this.getAudio();
        var src = $(this).attr("data-song-url");
        var guid = $(this).attr("data-song-guid");
        var title = $(this).attr("data-song-title");
        $audio.attr("src", src);
        $player.find(".song-control-panel").attr("data-song-guid", guid);
        $player.find(".song-title").empty();
        $player.find(".song-title").html(title);
        if ($player.find(".play-btn").attr("data-state") === "play") {
            _this.playSong($player.find(".play-btn").get(0));
        }
        else {
            $audio.get(0).play();
        }
    });
    $player.find(".remove-item").unbind("click");
    $player.find(".remove-item").click(function () {
        var guid = $(this).parent().attr("data-song-guid");
        _this.removePlaylistItem(guid);
    });
};

AudioPlayer.prototype.bindEvents = function () {
    var $player = $("#" + this.id);
    var _this = this;

    $player.find(".time-slider").slider({
        range: "min",
        start: function () {
            $(this).attr("data-enabled", "false");
        },
        stop: function () {
            if (_this.getAudio().get(0).readyState !== 0) {
                var $audio = _this.getAudio();
                var time = $audio.get(0).duration * $(this).slider("value") / 100;
                var inRnge = false;
                for (var i = 0; i < $audio.get(0).buffered.length; i++) {
                    if ((time > $audio.get(0).buffered.start(i)) && (time < $audio.get(0).buffered.end(i))) {
                        inRnge = true;
                    }
                }
                if (inRnge) {
                    $audio.get(0).currentTime = $audio.get(0).duration * $(this).slider("value") / 100;
                }
            }
            $(this).attr("data-enabled", "true");
        }
    });

    $player.find(".play-btn").click(function () {
        _this.playSong(this);
    });

    $player.find(".loop-btn").click(function () {
        _this.changeLoopState(this);
    });

    $player.find(".back-btn").click(function () {
        _this.goPrev();
    });

    $player.find(".forward-btn").click(function () {
        _this.goNext();
    });

    $player.find(".volume-btn").click(function () {
        _this.checkMute(this);
    });

    this.getAudio().get(0).addEventListener("timeupdate", function () { _this.renderProgress(); });
    this.getAudio().get(0).addEventListener("ended", function () { _this.checkLoop(); });

    this.bindPlayList();

    $player.find(".volume-slider").slider({
        range: "min",
        value: 100 * _this.currentVolume,
        stop: function () {
            _this.getAudio().get(0).volume = $(this).slider("value") / 100;
            _this.currentVolume = _this.getAudio().get(0).volume;
            if ((_this.currentVolume > 0) && ($player.find(".volume-btn").attr("data-state") === "mute")) {
                _this.checkMute($player.find(".volume-btn").get(0));
            }
            else if ((_this.currentVolume === 0) && ($player.find(".volume-btn").attr("data-state") === "normal")) {
                _this.checkMute($player.find(".volume-btn").get(0));
            }
        }
    });
};

jQuery.prototype.audioPlayer = function (options) {
    options.id = this.attr("id");
    var player = new AudioPlayer(options);
    player.generateMarkup();
    player.bindEvents();
    return player;
};