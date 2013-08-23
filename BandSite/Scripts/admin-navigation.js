var _entity = "song";
var _action = "index";
var uploader;

function GenerateCreateButtons() {
    $("[data-nav-action-type=create]").unbind("click");
    $("[data-nav-action-type=create]").click(function () {
        _entity = $(this).attr("data-entity-type");
        uploader.server.createAnchor();
        location.hash = "entity=" + _entity + "&action=create";
    });
}

function GenerateEditButtons() {
    $("[data-nav-action-type=edit]").unbind("click");
    $("[data-nav-action-type=edit]").click(function () {
        var id = $(this).attr("data-item-id");
        _entity = $(this).attr("data-entity-type");
        location.hash = "entity=" + _entity + "&action=edit&id=" + id;
    });
}

function GenerateDeleteButtons() {
    $("[data-nav-action-type=delete]").unbind("click");
    $("[data-nav-action-type=delete]").click(function () {
        var id = $(this).attr("data-item-id");
        _entity = $(this).attr("data-entity-type");
        location.hash = "entity=" + _entity + "&action=delete&id=" + id;
    });
}

function GenerateDetailsButtons() {
    $("[data-nav-action-type=details]").unbind("click");
    $("[data-nav-action-type=details]").click(function () {
        var id = $(this).attr("data-item-id");
        _entity = $(this).attr("data-entity-type");
        location.hash = "entity=" + _entity + "&action=details&id=" + id;
    });
}

function GenerateListButtons() {
    $("[data-nav-action-type=list]").unbind("click");
    $("[data-nav-action-type=list]").click(function () {
        _entity = $(this).attr("data-entity-type");
        location.hash = "entity=" + _entity + "&action=index";
    });
}

function GenerateAddAreas()
{
    $("[data-nav-action-type=add]").each(function () {
        var _THIS = $(this);
        var id =_THIS.attr("data-item-id");
        _entity = _THIS.attr("data-entity-type");
        var relatedEntity = _THIS.attr("data-related-entity-type");
        $.ajax({
            url: "/AdministrativeTools/" + _entity.toLowerCase() + "/add" + relatedEntity.toLowerCase() + "?" + _entity.toLowerCase() + "Id=" + id,
            cache: false
        }).done(function (html) {
            _THIS.empty();
            _THIS.append(html);
            SetCustomControls();
        });
    });
}

function GenerateShowAreas() {
    $("[data-nav-action-type=show]").each(function () {
        var _THIS = $(this);
        var id = $(this).attr("data-item-id");
        _entity = $(this).attr("data-entity-type");
        var relatedEntity = $(this).attr("data-related-entity-type");
        $.ajax({
            url: "/AdministrativeTools/" + _entity.toLowerCase() + "/show" + relatedEntity.toLowerCase() + "?" + _entity.toLowerCase() + "Id=" + id,
            cache: false
        }).done(function (html) {
            _THIS.empty();
            _THIS.append(html);
            if (_THIS.hasClass("editable"))
            {
                AddEditControls(_THIS);
            }
            SetCustomControls();
        });
    });
}

function AddEditControls($area) {
    var rows = $area.find("tr");
    var id = $area.attr("data-item-id");
    var entity = $area.attr("data-entity-type");
    var relatedEntity = $area.attr("data-related-entity-type");
    rows.each(function () {
        if ($(this).find("td").length > 0) {
            $(this).find("td").first().find("div").first().wrap("<form method='post' action='/AdministrativeTools/" + entity + "/delete" + relatedEntity + "?albumId=" + id + "'/>");
            $(this).find("td").find(".buttons").append("<input type='submit' data-sumbit-action-type='delete' value=''>");
        }
    });
}

function RenderContent() {
    var params = GetParams();
    if (params.entity === undefined) {
        params.entity = "song";
    }
    if (params.action === undefined) {
        params.action = "index";
    }
    if (params.id === undefined) {
        params.id = "";
    }
    if (params.loader === undefined) {
        params.loader = 1;
    }
    $("#content").empty();
    $("#action-loader").show().parent("div").each(function () {
        var t = 500;
        if (params.loader === 0) {
            t = 0;
            $("#action-loader").hide();
        }
        setTimeout(function () {
            $.ajax({
                url: "/AdministrativeTools/" + params.entity + "/" + params.action + "/" + params.id,
                cache: false,
                complete: function () {
                    $("#action-loader").hide();
                }
            }).done(function (html) {
                $("#content").append(html);
                SetCustomControls();
                GenerateAreas();
            });
        }, t);
    });
}

$(function(){
    RenderContent();
    uploader = $.connection.uploader;
    $.connection.hub.start();
    uploader.client.showProgress = function (percentage) {
        $(".ajax-loader").find(".progress").empty();
        $(".ajax-loader").find(".progress").append(percentage + " %");
    }

    window.onhashchange = function () {
        RenderContent();
    };
});

function RewriteSubmit() {
    $("#content").find("form").unbind("submit");
    $("#content").find("form").submit(function (event) {
        event.preventDefault();
        var $form = $(this);
        var formData;
        var options = {
            url: $form.attr("action"),
            type: $form.attr("method"),
            cache: false,
            beforeSend: function () {
                $("#action-loader").show();
                $("#content").empty();
            },
            complete: function () {
                $("#action-loader").hide();
            }
        };
        formData = $form.serialize();
        if ($form.attr("enctype") === "multipart/form-data") {
            formData = new FormData($form.get(0));
            options.contentType = false;
            options.processData = false;
        }
        options.data = formData;
        $.ajax(options).done(function (response) {
            location.hash = response.hash;
        });
    });
}

function SetCssStyle() {
    $("input.text-box").addClass("admin-fields");
    $("textarea.text-box").addClass("admin-fields");
    $(".display-label").addClass("admin-labels");
}

function GenerateCRUDButtons() {
    GenerateCreateButtons();
    GenerateDeleteButtons();
    GenerateDetailsButtons();
    GenerateEditButtons();
    GenerateListButtons();
}

function GenerateAreas()
{
    GenerateAddAreas();
    GenerateShowAreas();
}

function SetCustomControls()
{
    GenerateCRUDButtons();
    SetCssStyle();
    RewriteSubmit();
    AutocompleteConfig();
}

function GetParams()
{
    var strParams = location.hash.substr(1).split('&');
    var params = {};
    for (i = 0; i < strParams.length; i++) {
        var key = (strParams[i].split('='))[0];
        params[key] = (strParams[i].split('='))[1];
    }
    return params;
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