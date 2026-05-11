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
    // 5. Avant la soumission : on s'assure d'envoyer les cours !
    $(document).on("submit", "form", function () {
        // On sélectionne toutes les options de la liste "Cours actuels"
        // Le navigateur va envoyer automatiquement ceux qui sont "selected"
        $("#CurrentCourses option").prop("selected", true);
    });
    
});