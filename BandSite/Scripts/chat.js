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
    this._contactListItemTemplate = "<a class='list-group-item' data-toggle='tab'><i class='offline glyphicon glyphicon-user float-left'></i><span></span></a>";
    this._contactDialogTabTemplate = "<div class='tab-pane fade in'></div>";

    this._generateChatMarkup();
    this._bindSendBtnClickHandler();
    this._addHubClientMethods();
}

Chat.prototype._generateChatMarkup = function() {
    $("#" + this.id).html(this._containerTemplate);
    this._$sendBtn = $("#" + this.id).find(".btn.btn-primary");
    this._$messageTb = $("#" + this.id).find("textarea");
    $("#" + this.id).find(".panel-body").css("padding", "0 0 5px 0");
    $("#" + this.id).find("hr").css("margin-top", "5px");
    $("#" + this.id).find("hr").css("margin-bottom", "5px");
    $("#" + this.id).find(".panel-heading").css("text-align", "center");
    $("#" + this.id).find(".contact-list").sortable();
};

Chat.prototype._markAsOnline = function(contact) {
    $("a[data-contact=" + contact + "]").find("i").removeClass("offline");
};

Chat.prototype._markAsOffline = function (contact) {
    $("a[data-contact=" + contact + "]").find("i").addClass("offline");
};

Chat.prototype._setDefaultContact = function() {
    var $item = $("#" + this.id).find(".contact-list a").first();
    $item.addClass("active");
    this._currentContact = $item.attr("data-contact");
    $item = $($item.attr("href"));
    $item.addClass("active");    
};

Chat.prototype._addContact = function (contact) {
    var self = this;
    $("#" + this.id).find(".contact-list").append(this._contactListItemTemplate);
    var $item = $("#" + this.id).find(".contact-list a").last();
    var guid = generateGUID();
    $item.attr("href", "#" + guid);
    $item.find("span").html(contact);
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

Chat.prototype.login = function() {
    this._chat.client.login();
};

Chat.prototype._addHubClientMethods = function () {
    var self = this;
    var methodCollection = this._chat.client;
    methodCollection.addMessage = function (tab, contact, message, data) {
        var tabId = $("#" + self.id).find("a[data-contact=" + tab + "]").attr("href");
        var dialogTab = $(tabId);
        dialogTab.prepend("<p><b>" + contact + " [" + data + "]:</b><br><span>" + message + "</span></p>");
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
            for (i = 0; i < (contactsOnline !== undefined) ? contactsOnline.length : 0; i++) {
                self._markAsOnline(contactsOnline[i]);
            }
            self._setDefaultContact();
        });
    };
    methodCollection.logout = function () {
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
    this._$sendBtn.click(function() {
        self._chat.server.addMessage(self._currentContact, self._$messageTb.val());
    });
};

jQuery.prototype.chat = function () {
    var options = {};
    options.id = this.attr("id");
    var chat = new Chat(options);
    return chat;
};
