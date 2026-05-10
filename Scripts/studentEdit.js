$(document).ready(function() {
    // Gérer l'affichage côte à côte vs vertical
    function updateLayout()
    {
        let isInfoOpen = $("#info_section").prop("open");
        if (isInfoOpen)
        {
            $("#dual-list-container").removeClass("vertical").addClass("side-by-side");
            $("#moveToAvailable").text("▶");
            $("#moveToCurrent").text("◀");
        }
        else
        {
            $("#dual-list-container").removeClass("side-by-side").addClass("vertical");
            $("#moveToAvailable").text("▼");
            $("#moveToCurrent").text("▲");
        }
    }

    $("#info_section").on("toggle", updateLayout);
    updateLayout();

    // Transfert vers "Disponibles"
    $("#moveToAvailable").click(function() {
        $("#CurrentCourses option:selected").detach().appendTo("#AvailableCourses");
    });

    // Transfert vers "Actuels"
    $("#moveToCurrent").click(function() {
        $("#AvailableCourses option:selected").detach().appendTo("#CurrentCourses");
    });

    // Désélectionner tout (le fameux bouton X)
    $("#deselectAll").click(function() {
        $("#CurrentCourses option, #AvailableCourses option").prop("selected", false);
    });

    // Avant de soumettre le formulaire, on crée des inputs cachés pour les cours dans "Actuels"
    $("form").submit(function() {
        $("#hiddenSelections").empty();
        $("#CurrentCourses option").each(function() {
            $("#hiddenSelections").append(`< input type = "hidden" name = "SelectedCourses" value = "${$(this).val()}" />`);
        });
    });
});