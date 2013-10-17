function generateGUID() {
    var d = new Date().getTime();
    var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c === 'x' ? r : (r & 0x7 | 0x8)).toString(16);
    });
    return guid;
}

function Chat(options) {
    this.id = options.id;
    this._unreadConversations = 0;
    this._chat = $.connection.chatHub;
    this._$sendBtn = undefined;
    this._$messageTb = undefined;
    this._currentContact = undefined;
    this._conferences = [];
    this._unreadMsgDelay = 1000;
    this._containerTemplate = "<div class='panel panel-default'>" +
                                 "<div class='panel-heading'>" +
                                         "<span>Contacts<span>" +
                                         "<span class='badge conversations-badge'></span>" +
                                         "<a><i class='minimize-btn glyphicon glyphicon-arrow-left'></i></a>" +
                                 "</div>" +
                                 "<div class='panel-body'>" +
                                   "<div class='contact-scroller'>" +
                                       "<div class='list-group contact-list'>" +
                                       "</div>" +
                                   "</div>" +
                                   "<hr><hr>" +
                                   "<div class='tab-content dialog-tab'></div>" +
                                   "<hr><hr>" +
                                   "<div class='text-area'><div class='form-group'>" +
                                         "<textarea class='form-control' />" +
                                   "</div>" +
                                   "<button class='btn btn-primary'>Send</button></div>" +
                                 "</div>" +
                              "</div>";
    this._contactListItemTemplate = "<a class='list-group-item' data-toggle='tab'>" +
                                         "<i class='offline glyphicon glyphicon-user'></i>" +
                                         "<span></span>" +
                                         "<i class='glyphicon glyphicon-remove-circle float-right'></i>" +
                                         //"<i class='glyphicon glyphicon-edit float-right'></i>" +
                                         "<span class='badge'></span>" +
                                    "</a>";
    this._contactDialogTabTemplate = "<div class='tab-pane fade in list-group'></div>";

    this._generateChatMarkup();
    this._bindSendBtnClickHandler();
    this._addHubClientMethods();
}

Chat.prototype._generateChatMarkup = function () {
    var self = this;
    $("#" + this.id).html(this._containerTemplate);
    this._$sendBtn = $("#" + this.id).find(".btn.btn-primary");
    this._$messageTb = $("#" + this.id).find("textarea");
    $("#" + this.id).find(".contact-list").sortable();

    $("#" + this.id).find(".dialog-tab").scroll($.debounce(self._unreadMsgDelay / 2, function () {
        self._checkUnreadMessages(self._currentContact, self._unreadMsgDelay / 2);
    }));

    $("#" + this.id).find(".minimize-btn").click(function () {
        self._resize();
    });
    $(window).resize(function () {
       if (($(this).width() >= 1200) && self._isCompact) self._resize();
    });
};

Chat.prototype._resize = function () {
    if (this._isCompact !== true) {
        $("#" + this.id).find(".panel-body").css("display", "none");
        $("#" + this.id).find(".panel").addClass("compact");
        $("#" + this.id).find(".minimize-btn").removeClass("glyphicon-arrow-left");
        $("#" + this.id).find(".minimize-btn").addClass("glyphicon-arrow-right");
        this._isCompact = true;
    }
    else {
        $("#" + this.id).find(".panel-body").css("display", "block");
        $("#" + this.id).find(".panel").removeClass("compact");
        $("#" + this.id).find(".minimize-btn").addClass("glyphicon-arrow-left");
        $("#" + this.id).find(".minimize-btn").removeClass("glyphicon-arrow-right");
        this._isCompact = false;
    }
};

Chat.prototype._markAsOnline = function (users) {
    for (var conf in this._conferences) {
        var online = 0;
        for (var user in this._conferences[conf].online) {
            for (var j = 0; j < users.length; j++) {
                if ((users[j] === user) && (this._conferences[conf].online[user] === 0)) {
                    this._conferences[conf].online[user]++;
                }
            }
            online += this._conferences[conf].online[user];
        }
        if (online > 0) {
            $("a[data-conference=" + conf + "]").find("i").removeClass("offline");
        }
    }
};

Chat.prototype._markAsOffline = function (users) {
    for (var conf in this._conferences) {
        var online = 0;
        for (var user in this._conferences[conf].online) {
            for (var j = 0; j < users.length; j++) {
                if ((users[j] === user) && (this._conferences[conf].online[user] === 1)) {
                    this._conferences[conf].online[user]--;
                }
            }
            online += this._conferences[conf].online[user];
        }
        if (online === 0) {
            $("a[data-conference=" + conf + "]").find("i.glyphicon-user").addClass("offline");
        }
    }
};

Chat.prototype.checkScrollOnBottom = function () {
    var scrollheight = $(".dialog-tab").get(0).scrollHeight;
    var scrollTop = $(".dialog-tab").scrollTop();
    var dialogTabHeight = $(".dialog-tab").outerHeight();
    return (scrollheight === scrollTop + dialogTabHeight);
};

Chat.prototype.moveScrollDown = function () {
    var height = $(".dialog-tab").get(0).scrollHeight;
    $(".dialog-tab").scrollTop(height);
};

Chat.prototype._createAddConfButton = function () {
    var self = this;
    $("#" + this.id).find(".contact-list").append("<a class='add-conference-btn list-group-item'><i class='glyphicon glyphicon-plus'></i><span>New conversation</span></a>");
    $(".add-conference-btn").click(function () {
        if ($("body").find(".add-conference-dialog").length === 0) {
            $("body").append("<div class='add-conference-dialog modal fade' tabindex='-1' role='dialog'>" +
                          "<div class='modal-dialog'>" +
                              "<div class='modal-content'>" +
                                  "<div class='modal-header'>" +
                                      "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>×</button>" +
                                      "<div class='modal-title'>New conversation</div>" +
                                  "</div>" +
                                  "<div class='modal-body'>" +
                                      "<div class='error-area'></div>" + 
                                      "<div class='form-group'><label>Title</label><input class='form-control contact-title' type='text'/></div>" +
                                       "<label>Add users to conversation</label>" +
                                      "<div class='users-list list-group'></div>" +
                                      "<div class='btn btn-primary'>Create</div>" +
                                  "</div>" +
                              "</div>" +
                          "</div>" +
                      "</div>");
        }
        var $confDialog = $("body").find(".add-conference-dialog");
        $.ajax({
            url: "/Account/GetUsersList",
            type: "GET",
            cache: false
        }).done(function (users) {
            var invite = {};
            $confDialog.find(".users-list").empty();
            for (var i = 0; i < users.length; i++) {
                $confDialog.find(".users-list").append("<a class='list-group-item'><i class='glyphicon glyphicon-user'></i><span>" + users[i] + "</span></a>");
            }
            $confDialog.find(".list-group-item").click(function () {
                if ($(this).hasClass("active")) {
                    $(this).removeClass("active");
                    var user = $(this).find("span").first().html();
                    invite[user] = 0;
                } else {
                    $(this).addClass("active");
                    var user = $(this).find("span").first().html();
                    invite[user] = 1;
                }
            });
            $confDialog.find(".btn").click(function () {
                var title = $confDialog.find(".contact-title").val();
                var usersArray = [];
                var i = 0;
                for (var name in invite) {
                    if (invite[name] === 1)
                    {
                        usersArray[i] = name;
                        i++;
                    }
                }
                if ((title !== "") && (usersArray.length > 0)) {
                    usersArray[usersArray.length] = self._currentUser;
                    self._chat.server.createConference(title, usersArray);
                    $confDialog.modal("hide");
                }
                else {
                    $confDialog.find(".error-area").html("<div class='alert alert-block alert-danger fade in'></div>");
                    if (title === "") {
                        $confDialog.find(".alert").append("<p>You need to enter a title</p>");
                    }
                    if (usersArray.length === 0) {
                        $confDialog.find(".alert").append("<p>You need to select at least one user</p>");
                    }
                    $confDialog.find(".alert").alert();
                }
            });
            $confDialog.on('hidden.bs.modal', function () {
                $confDialog.remove();
            });
            $confDialog.modal({ show: true });
        });
    });
};

Chat.prototype._addContact = function (conference) {
    var self = this;
    this._conferences[conference.guid] = {
        unreadMsgCount: 0,
        title: conference.title,
        online: {}
    };
    for (var i = 0; i < conference.users.length; i++) {
        this._conferences[conference.guid].online[conference.users[i]] = 0;
    }
    $("#" + this.id).find(".contact-list").prepend(this._contactListItemTemplate);
    var $item = $("#" + this.id).find(".contact-list a").first();
    var guid = generateGUID();
    $item.attr("href", "#" + guid);
    $item.find("span").first().html(conference.title);
    $item.attr("data-conference", conference.guid);
    $item.click(function () {
        self._currentContact = $(this).attr("data-conference");
        $(this).parent().find(".active").removeClass("active");
        $(this).addClass("active");
        self._checkUnreadMessages(self._currentContact, self._unreadMsgDelay);
    });

    $item.find(".glyphicon-remove-circle").click(function () {
        var guid = $(this).parent().attr("data-conference");
        self._chat.server.removeUserFromConference(guid, self._currentUser);
    });

    $("#" + this.id).find(".tab-content").prepend(this._contactDialogTabTemplate);
    $item = $("#" + this.id).find(".tab-content div").first();
    $item.attr("id", guid);
    $("#" + this.id).find(".contact-list a").first().on('shown.bs.tab', function (e) {
        self.moveScrollDown();
    });
};

Chat.prototype.logout = function () {
    $.connection.hub.stop();
    $("#" + this.id).find(".contact-list").empty();
    $("#" + this.id).find(".tab-content").empty();
};

Chat.prototype.login = function() {
    $.connection.hub.start();
};

Chat.prototype.decreaseUnreadConversations = function () {
    if (this._unreadConversations > 0) {
        this._unreadConversations--;
        $(".conversations-badge").html((this._unreadConversations > 0) ? this._unreadConversations : "");
    }
};

Chat.prototype.increaseUnreadMsgCount = function (guid) {
    var $badge = $("#" + this.id).find("a[data-conference=" + guid + "] .badge");
    this._conferences[guid].unreadMsgCount = this._conferences[guid].unreadMsgCount + 1;
    if (this._conferences[guid].unreadMsgCount === 0) {
        this._unreadConversations++;
        $(".conversations-badge").html(this._unreadConversations);
    }
    $badge.html(this._conferences[guid].unreadMsgCount);
};

Chat.prototype.decreaseUnreadMsgCount = function (guid) {
    var $badge = $("#" + this.id).find("a[data-conference=" + guid + "] .badge");
    this._conferences[guid].unreadMsgCount = this._conferences[guid].unreadMsgCount - 1;
    if (this._conferences[guid].unreadMsgCount !== 0) {
        $badge.html(this._conferences[guid].unreadMsgCount);
    }
    else {
        this.decreaseUnreadConversations();
        $badge.html("");
    }
};

Chat.prototype._addHubClientMethods = function () {
    var self = this;
    var methodCollection = this._chat.client;
    methodCollection.messagesDelivered = function (guid) {
        var tabId = $("#" + self.id).find("a[data-conference=" + guid + "]").attr("href");
        var dialogTab = $(tabId);
        dialogTab.find(".undelivered").removeClass("undelivered");
    };
    methodCollection.addMessage = function (guid, message) {
        var tabId = $("#" + self.id).find("a[data-conference=" + guid + "]").attr("href");
        var dialogTab = $(tabId);
        var isScrollOnBottom = self.checkScrollOnBottom();
        var $dialog = dialogTab.append("<a href='" + location.hash + "' class='list-group-item' data-msg-guid='" + message.guid + "'>" +
                                           "<b class='list-group-item-heading " + ((message.sender === self._currentUser) ? "my-msg" : "") + "'>" +
                                                message.sender + " [" + message.date + "]:" +
                                                "<i class='glyphicon glyphicon-refresh float-right'></i>" +
                                                "<i class='glyphicon glyphicon-exclamation-sign float-right'></i>" +
                                           "</b>" +
                                           "<p class='list-group-item-text'>" +
                                              "<span>" + message.text + "</span>" +
                                           "</p>" +
                                        "</a>");
        if (isScrollOnBottom) {
            self.moveScrollDown();
        }
        switch (message.status) {
            case "Undelivered":
                $dialog.find("a").last().addClass("undelivered");
                break;
            case "Unread":
                $dialog.find("a").last().addClass("unread");
                self.increaseUnreadMsgCount(guid);
                self._checkUnreadMessages(self._currentContact, self._unreadMsgDelay);
                break;
        }
    };
    methodCollection.login = function (me, contactsOnline) {
        $.ajax({
            url: "/Account/GetConferenceList",
            type: "GET",
            cache: false
        }).done(function (conferences) {
            self._currentUser = me;
            for (var i = 0; i < conferences.length; i++) {
                self._addContact(conferences[i]);
                self._chat.server.loadHistory(conferences[i].guid);
            }
            self._markAsOnline(contactsOnline);
            self._createAddConfButton();
        });
    };
    methodCollection.contactOnline = function (users) {
        self._markAsOnline(users);
    };
    methodCollection.contactOffline = function (users) {
        self._markAsOffline(users);
    };
    methodCollection.disconnect = function () {
        $("body").append("<div class='disconnect-msg modal fade' tabindex='-1' role='dialog'>" +
                             "<div class='modal-dialog'>" +
                                 "<div class='modal-content'>" +
                                     "<div class='modal-header'>" +
                                         "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>×</button>" +
                                         "<div class='modal-title'>Alert (connections number is over limit)</div>" +
                                     "</div>" +
                                     "<div class='modal-body'>This tab has been disconnected from chat</div>" +
                                 "</div>" +
                             "</div>" +
                         "</div>");
        $("body").find(".disconnect-msg").modal({ show: true });
        self.logout();
    };
    methodCollection.markReadMessage = function (guid) {
        var $msg = $("#" + self.id).find("a[data-msg-guid=" + guid + "]");
        $msg.removeClass("unread");
        var tabId = $msg.parents(".tab-pane").attr("id");
        var confGuid = $("#" + self.id).find("a[href=#" + tabId + "]").attr("data-conference");
        self.decreaseUnreadMsgCount(confGuid);
    };
    methodCollection.createConference = function (conference, online) {
        self._addContact(conference);
        self._markAsOnline(online);
    };
    methodCollection.removeUserFromConference = function (guid, user) {
        var online = 0;
        for (var name in self._conferences[guid].online) {
            if (name === user) {
                delete self._conferences[guid].online[user];
            } else {
                online += self._conferences[guid].online[name];
            }
        }
        if (online === 0) {
            $("a[data-conference=" + guid + "]").find("i.glyphicon-user").addClass("offline");
        }
    };
    methodCollection.removeConference = function (guid) {
        if (self._conferences[guid].unreadMsgCount > 0) {
            self.decreaseUnreadConversations();
        }
        delete self._conferences[guid];
        var tabId = $("a[data-conference=" + guid + "]").attr("href");
        $("a[data-conference=" + guid + "]").remove();
        $(tabId).remove();
    };
};

Chat.prototype.send = function () {
    if (this._$messageTb.val() !== "") {
        this._chat.server.addMessage(this._currentContact, this._$messageTb.val());
        this._$messageTb.val("");
    }
};

Chat.prototype._bindSendBtnClickHandler = function () {
    var self = this;
    this._$sendBtn.click(function () {
        self.send();
    });
    this._$messageTb.keydown(function (e) {
        if ((e.keyCode === 13) && (!self._shiftMode)) {
            self.send();
            e.preventDefault();
        } else if (e.keyCode === 16) {
            self._shiftMode = true;
        }
    });
    this._$messageTb.keyup(function (e) {
        if (e.keyCode === 16) {
            self._shiftMode = false;
        }
    });
};

Chat.prototype._checkUnreadMessages = function (guid, delay) {
    var self = this;
    var tabId = $("#" + self.id).find("a[data-conference=" + guid + "]").attr("href");
    var msgArray = [];
    var arrInd = 0;
    clearTimeout(self._unreadMsgTimeout);
    self._unreadMsgTimeout = setTimeout(function () {
            $(tabId).find("a.unread").each(function () {
                var $msg = $(this);
                var $list = $("#" + self.id).find(".dialog-tab");
                if (($msg.offset().top < $list.offset().top + $list.height() - 10) && ($msg.offset().top > $list.offset().top)) {
                    msgArray[arrInd] = $msg.attr("data-msg-guid");
                    arrInd++;
                }
            });
            self._chat.server.markReadMessages(msgArray).done(function () { self._locked = false; });
        }, delay);
};

jQuery.prototype.chat = function () {
    var options = {};
    options.id = this.attr("id");
    var chat = new Chat(options);
    return chat;
};
