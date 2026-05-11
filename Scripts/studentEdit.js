$(document).ready(function () {
    // 1. Gestion du layout (côte à côte vs vertical)
    function updateLayout() {
        let isInfoOpen = $("#info_section").prop("open");
        if (isInfoOpen) {
            $("#dual-list-container").removeClass("vertical").addClass("side-by-side");
            $("#moveToAvailable").text("▶");
            $("#moveToCurrent").text("◀");
        } else {
            $("#dual-list-container").removeClass("side-by-side").addClass("vertical");
            $("#moveToAvailable").text("▼");
            $("#moveToCurrent").text("▲");
        }
    }

    $(document).on("toggle", "#info_section", updateLayout);
    updateLayout();

    // 2. Transfert vers "Disponibles" (Utilisation de .on pour AJAX)
    $(document).on("click", "#moveToAvailable", function () {
        $("#CurrentCourses option:selected").detach().appendTo("#AvailableCourses");
    });

    // 3. Transfert vers "Actuels"
    $(document).on("click", "#moveToCurrent", function () {
        $("#AvailableCourses option:selected").detach().appendTo("#CurrentCourses");
    });

    // 4. Bouton X
    $(document).on("click", "#deselectAll", function () {
        $("#CurrentCourses option, #AvailableCourses option").prop("selected", false);
    });

    // 5. Avant la soumission : on prépare les données pour le C#
    $(document).on("submit", "form", function () {
        $("#hiddenSelections").empty();
        $("#CurrentCourses option").each(function () {
            // Correction de la syntaxe de l'input caché
            $("#hiddenSelections").append('<input type="hidden" name="SelectedCourses" value="' + $(this).val() + '" />');
        });
    });
});