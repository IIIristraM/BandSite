$(function () {
    window.addEventListener("hashchange", function () {
        if (location.hash.indexOf("/") !== -1) {
            loadContent();
        }
    });
    (location.hash === "") ? location.hash = "#home/index" : loadContent();
});

function loadContent() {
    var params = getParams();
    $.ajax({
        // ../controller/action/id[optional]
        url: "/" + params[0] +
             "/" + params[1] +
             (params[2] !== undefined ? "/" + params[2] : ""),
        cache: false
    }).done(function (content) {
        renderContent(content);
    });
}

function renderContent(content) {
    $.ajax({
        url: "/account/index",
        cache: false
    }).done(function (loginBar) {
        $("#login-area").empty();
        $("#login-area").html(loginBar);
        $("#content").empty();
        $("#content").append(content);
        rewriteSubmit();
    });
}

function getParams()
{
    var params = location.hash.substr(1).split('/');
    return params;
}

function rewriteSubmit() {
    $("form:not([data-sealed])").each(function() {
        $(this).unbind("submit");
        $(this).submit(function (event) {
            event.preventDefault();
            var $form = $(this);
            var formData;
            var options = {
                url: $form.attr("action"),
                type: $form.attr("method"),
                cache: false
            };
            formData = $form.serialize();
            if ($form.attr("enctype") === "multipart/form-data") {
                formData = new FormData($form.get(0));
                options.contentType = false;
                options.processData = false;
            }
            options.data = formData;
            $.ajax(options).done(function (response) {
                if (response.hash !== undefined) {
                    (location.hash !== response.hash) ? location.hash = response.hash : loadContent();
                } else if ((typeof response) === "string") {
                    renderContent(response);
                } else {
                    alert("error: url " + url + " returns unexpected result");
                    location.href = location.href.substr(0, location.href.indexOf("#"));
                }
            });
        });
    });
}

//-------------------------------------------------------------------------------------------------

