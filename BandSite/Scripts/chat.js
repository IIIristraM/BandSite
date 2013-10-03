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
    this._chat = $.connection.chatHub;
    this._$sendBtn = undefined;
    this._$messageTb = undefined;
    this._currentContact = undefined;
    this._contacts = undefined;
    this._containerTemplate = "<div class='panel panel-default'>" +
                                 "<div class='panel-heading'>Contacts</div>" +
                                 "<div class='panel-body'>" +
                                   "<div class='contact-scroller'><div class='list-group contact-list'></div></div>" +
                                   "<div class='tab-content dialog-tab'></div>" +
                                   "<hr>" +
                                   "<div class='text-area'><div class='form-group'>" +
                                         "<textarea class='form-control' />" +
                                   "</div>" +
                                   "<button class='btn btn-primary'>Send</button></div>" +
                                 "</div>" +
                              "</div>";
    this._contactListItemTemplate = "<a class='list-group-item' data-toggle='tab'><i class='offline glyphicon glyphicon-user float-left'></i><span></span><span class='badge'></span></a>";
    this._contactDialogTabTemplate = "<div class='tab-pane fade in'></div>";

    this._generateChatMarkup();
    this._bindSendBtnClickHandler();
    this._addHubClientMethods();
}

Chat.prototype._generateChatMarkup = function() {
    $("#" + this.id).html(this._containerTemplate);
    this._$sendBtn = $("#" + this.id).find(".btn.btn-primary");
    this._$messageTb = $("#" + this.id).find("textarea");
    $("#" + this.id).find(".contact-list").sortable();
};

Chat.prototype._markAsOnline = function(contact) {
    $("a[data-contact=" + contact + "]").find("i").removeClass("offline");
};

Chat.prototype._markAsOffline = function (contact) {
    $("a[data-contact=" + contact + "]").find("i").addClass("offline");
};

Chat.prototype._setDefaultContact = function() {
    var $contact = $("#" + this.id).find(".contact-list a").first();
    $contact.addClass("active");
    this._currentContact = $contact.attr("data-contact");
    $dialog = $($contact.attr("href"));
    $dialog.addClass("active");
};

Chat.prototype._addContact = function (contact) {
    var self = this;
    $("#" + this.id).find(".contact-list").append(this._contactListItemTemplate);
    var $item = $("#" + this.id).find(".contact-list a").last();
    var guid = generateGUID();
    $item.attr("href", "#" + guid);
    $item.find("span").first().html(contact);
    $item.attr("data-contact", contact);
    $item.click(function () {
        self._currentContact = $(this).attr("data-contact");
        $(this).parent().find(".active").removeClass("active");
        $(this).addClass("active");
    });

    $("#" + this.id).find(".tab-content").prepend(this._contactDialogTabTemplate);
    $item = $("#" + this.id).find(".tab-content div").first();
    $item.attr("id", guid);
};

Chat.prototype.logout = function () {
    $.connection.hub.stop();
    $("#" + this.id).find(".contact-list").empty();
    $("#" + this.id).find(".tab-content").empty();
};

Chat.prototype.login = function() {
    $.connection.hub.start();
};

Chat.prototype.increaseUnreadMsgCount = function (tab) {
    var $badge = $("#" + this.id).find("a[data-contact=" + tab + "] .badge");
    var count = 0;
    if ($badge.html() !== "")
        count = parseInt($badge.html(), 10);
    $badge.html(count + 1);
};

Chat.prototype.decreaseUnreadMsgCount = function (tab) {
    var $badge = $("#" + this.id).find("a[data-contact=" + tab + "] .badge");
    var count = "";
    if ($badge.html() !== "")
        count = parseInt($badge.html(), 10) - 1;
    $badge.html(count);
};

Chat.prototype._addHubClientMethods = function () {
    var self = this;
    var methodCollection = this._chat.client;
    methodCollection.messageDelivered = function (guid) {
        $("#" + self.id).find("p[data-msg-guid=" + guid + "]").removeClass("undelivered");
    };
    methodCollection.addMessage = function (tab, contact, message) {
        var tabId = $("#" + self.id).find("a[data-contact=" + tab + "]").attr("href");
        var dialogTab = $(tabId);
        var $dialog = dialogTab.prepend("<p data-msg-guid='" + message.guid + "'><b>" + contact + " [" + message.date + "]:</b><br><span>" + message.text + "</span></p>");
        switch (message.status) {
            case "Undelivered":
                $dialog.find("p").first().addClass("undelivered");
                break;
            case "Unread":
                $dialog.find("p").first().addClass("unread");
                self.increaseUnreadMsgCount(tab);
                setTimeout(function () {
                    var $msg = $("#" + self.id).find("p[data-msg-guid=" + message.guid + "]");
                    var $list = $("#" + self.id).find(".dialog-tab");
                    if (($msg.offset().top < $list.offset().top + $list.height() - 10) && ($msg.offset().top > $list.offset().top)) {
                        $msg = $msg.removeClass("unread");
                        self.decreaseUnreadMsgCount(tab);
                    }
                }, 2500);
                break;
        }
    };
    methodCollection.login = function (contactsOnline) {
        $.ajax({
            url: "/Account/GetUserslist",
            type: "GET",
            cache: false
        }).done(function (allContacts) {
            $("#" + self.id).find(".contact-list").empty();
            $("#" + self.id).find(".tab-content").empty();
            for (var i = 0; i < allContacts.length; i++) {
                self._addContact(allContacts[i].name);
                self._chat.server.loadHistoryWith(allContacts[i].name);
            }
            var onlineCount = (contactsOnline !== undefined) ? contactsOnline.length : 0;
            for (i = 0; i < onlineCount; i++) {
                self._markAsOnline(contactsOnline[i]);
            }
            self._setDefaultContact();
        });
    };
    methodCollection.contactOnline = function (contact) {
        self._markAsOnline(contact);
    };
    methodCollection.contactOffline = function (contact) {
        self._markAsOffline(contact);
    };
    $.connection.hub.start();
};

Chat.prototype._bindSendBtnClickHandler = function () {
    var self = this;
    this._$sendBtn.click(function () {
        if (self._$messageTb.val() !== "") {
            self._chat.server.addMessage(self._currentContact, self._$messageTb.val());
            self._$messageTb.val("");
        }
    });
};

jQuery.prototype.chat = function () {
    var options = {};
    options.id = this.attr("id");
    var chat = new Chat(options);
    return chat;
};
