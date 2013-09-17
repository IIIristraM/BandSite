function autocompleteConfig() {
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

function configNavbar() {
    $(".nav.navbar-nav li").click(function () {
        $(".nav.navbar-nav").find(".active").removeClass("active");
        $(this).addClass("active");
    });
}

$(function() {
    configNavbar();
});

