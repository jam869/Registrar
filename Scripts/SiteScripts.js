$(document).ready(function () {
    $('.phone').mask('(999) 999-9999');
    $('.phoneExt').mask('(999) 999-9999 poste 99999');
    $('.zipcode').mask('a9a 9a9');
    $(".datepicker").datepicker({
        dateFormat: 'yy-mm-dd',
        changeMonth: true,
        changeYear: true,
        //yearRange: "-100:-15",
        dayNamesMin: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"],
        monthNamesShort: ["Janv.", "Févr.", "Mars", "Avril", "Mai", "Juin", "Juil.", "Août", "Sept.", "Oct.", "Nov.", "Déc."]
    });

    /*Filter unicode hack */
    $(":input").change(function () {
        try {
            let r = $(this).val().replace(/[^\x00-\xFF]/g, "");
            $(this).val(r);
        } catch (e) { }
    });
    $("textarea").change(function () {
        try {
            let r = $(this).val().replace(/[^\x00-\xFF]/g, "");
            $(this).val(r);
        } catch (e) { }
    });

    $(".countrySelect").change((e) => {
        $(e.target).next().attr("src", "/Images_Data/Loading_icon.gif")
        $.ajax({
            url: "/CountryFlag/get?countryCode=" + $(e.target).val(),
            datatype: "application/json",
            success: json => { $(e.target).next().attr("src", json); }
        });

    })
    SummaryHandling();
})


function SummaryHandling() {

    $('summary').attr('title', 'Utilisez ctrl-clic pour développer/réduire');
    $('summary').off();
    // Toggle collapse uncollapse details
    $('summary').on('click', function (e) {
        if (e.ctrlKey) {
            if ($(this).parent().attr('open') != undefined) {
                $('details').removeAttr('open');
                e.preventDefault();
            }
            else {
                $('details').prop('open', true);
                e.preventDefault();
            }
        }
    })
}
function RestoreDetailsState() {
    // Désattache les anciens événements pour éviter les doublons
    $("details").off('toggle');

    // Attache l'événement de bascule
    $("details").on('toggle', function () {
        let details_dom = $(this)[0];
        if (details_dom != undefined) {
            // Sauvegarde l'état dans la mémoire du navigateur
            localStorage.setItem(details_dom.id, details_dom.open);
        }
    });

    // Restaure l'état de chaque balise details
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key.indexOf("details") > -1) {
            let details_dom = $("#" + key)[0];
            if (details_dom != undefined) {
                details_dom.open = (localStorage.getItem(key) == "true");
            }
        }
    }
}

$(".submitCmd").click(function () {
    $("form").submit();
})
function InstallAutoComplete(targetId, words) {

    function split(val) {
        return val.split(/ \s*/);
    }

    function RemoveExtra(str, extra) {
        var extraLength = extra.length;
        var lastExtraIndex = str.lastIndexOf(extra);
        if ((lastExtraIndex + extraLength) == str.length)
            str = str.substring(0, str.length - extraLength);
        return str;
    }

    function extractLast(term) {
        return split(term).pop();
    }

    $("#" + targetId)
        // don't navigate away from the field on tab when selecting an item
        .bind("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB && $(this).data("ui-autocomplete").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            minLength: 1,
            source: function (request, response) {
                // delegate back to autocomplete, but extract the last term
                response($.ui.autocomplete.filter(words, extractLast(request.term)));
            },
            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = RemoveExtra(terms, ",").join(" ");
                return false;
            }
        });
}

function ajaxActionCall(actionLink) {
    // Ajax Action Call to actionLink
    $.ajax({
        url: actionLink,
        method: 'GET',
        success: (data) => {
            console.log("Result: " + data);
        }
    });
}

let minKeywordLenth = 1;
function highlight(text, elem) {
    text = text.trim();
    if (text.length >= minKeywordLenth) {
        var innerHTML = elem.innerHTML;
        let startIndex = 0;

        while (startIndex < innerHTML.length) {
            var normalizedHtml = innerHTML.toLocaleLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
            var index = normalizedHtml.indexOf(text, startIndex);
            let highLightedText = "";
            if (index >= startIndex) {
                highLightedText = "<span class='highlight'>" + innerHTML.substring(index, index + text.length) + "</span>";
                innerHTML = innerHTML.substring(0, index) + highLightedText + innerHTML.substring(index + text.length);
                startIndex = index + highLightedText.length + 1;
            } else
                startIndex = innerHTML.length + 1;
        }
        elem.innerHTML = innerHTML;
    }
}

$(document).ready(function () {
    // Restaurer l'état au chargement
    // Restaurer l'état au chargement
    $("details").each(function () {
        if (this.id) {
            let savedState = localStorage.getItem(this.id);
            if (savedState === "true") $(this).attr("open", "");
            if (savedState === "false") $(this).removeAttr("open");
        }
    });

    // Sauvegarder lors du clic (toggle)
    $("details").on("toggle", function () {
        if (this.id) {
            localStorage.setItem(this.id, this.open);
        }
    });

    $("#menu-toggle").click(function () {
        $("#side-menu").addClass("open");
    });

    $("#menu-close").click(function () {
        $("#side-menu").removeClass("open");
    });

    // Fermer si on clique à l'extérieur
    $(document).click(function (e) {
        if (!$(e.target).closest('#side-menu, #menu-toggle').length) {
            $("#side-menu").removeClass("open");
        }
    });

    // Gérer le Ctrl+Clic pour tout ouvrir/fermer
    $("summary").click(function (e) {
        if (e.ctrlKey) {
            e.preventDefault();
            let parentDetails = $(this).parent()[0];
            let state = !parentDetails.open;

            $("details").each(function () {
                if (state) {
                    $(this).attr("open", "");
                } else {
                    $(this).removeAttr("open");
                }
                $(this).trigger("toggle");
            });
        }
    });

});


function bootboxConfirmDelete(url, controller) {
    bootbox.confirm({
        closeButton: false, // Enlève le X en haut à droite
        message: "Effacer ?", // JUSTE le message
        buttons: {
            cancel: { label: 'Annuler', className: 'btn-secondary' },
            confirm: { label: "D'accord", className: 'btn-primary' }
        },
        callback: function (result) {
            if (result) {
                window.location.href = url;
            }
        }
    });
}