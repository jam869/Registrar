// script pour l'interface de gestion de sélection avec deux <select...>
// auteur : Nicolas Chourot

$(document).ready(initUI);

function initUI() {
    sortAllSelect();
    $('.selectionsGrid').each(function() {
        deSelectAll($(this));
    });

    $(document).on('change', '.UnselectedItems', function (e) {
        let grid = $(this).closest('.selectionsGrid');
        if ($(this).find('option:selected').length > 0) {
            grid.find(".SelectedItems option:selected").prop("selected", false);
        }
        updateGridState(grid);
        e.preventDefault();
    });

    $(document).on('change', '.SelectedItems', function (e) {
        let grid = $(this).closest('.selectionsGrid');
        if ($(this).find('option:selected').length > 0) {
            grid.find(".UnselectedItems option:selected").prop("selected", false);
        }
        updateGridState(grid);
        e.preventDefault();
    });

    $(document).on('submit', 'form', function () {
        $('.SelectedItems option').prop('selected', true);
    });

    $(document).on('click', '.AddSelection', function () {
        let grid = $(this).closest('.selectionsGrid');
        grid.find('.UnselectedItems').find('option:selected').each(function () {
            $(this).remove();
            $(this).prop('selected', false);
            grid.find('.SelectedItems').append($(this));
            sortSelect(grid.find(".SelectedItems"));
            scrollTo(grid.find(".SelectedItems"), $(this).offset().top);
            grid.find(".SelectedItems").focus();
        });
        updateGridState(grid);
    });

    $(document).on('click', '.RemoveSelection', function () {
        let grid = $(this).closest('.selectionsGrid');
        grid.find('.SelectedItems').find('option:selected').each(function () {
            $(this).remove();
            $(this).prop('selected', false);
            grid.find('.UnselectedItems').append($(this));
            sortSelect(grid.find(".UnselectedItems"));
            scrollTo(grid.find(".UnselectedItems"), $(this).offset().top);
            grid.find(".UnselectedItems").focus();
        });
        updateGridState(grid);
    });

    $(document).on('click', '.UnselectAll', function () {
        let grid = $(this).closest('.selectionsGrid');
        deSelectAll(grid);
    });

    // Gestion du changement de layout basé sur l'état du premier <details>
    $(document).on('toggle', 'details', function () {
        updateLayout();
    });

    // Backup pour certains navigateurs où toggle ne suffit pas
    $(document).on('click', 'summary', function() {
        setTimeout(updateLayout, 10);
    });

    updateLayout();
}

function updateGridState(grid) {
    let hasSelected = grid.find('.SelectedItems option:selected').length > 0;
    let hasUnselected = grid.find('.UnselectedItems option:selected').length > 0;
    let isStacked = grid.hasClass('stacked');

    // Mise à jour des icônes selon le layout
    let addIcon = grid.find('.AddSelection');
    let removeIcon = grid.find('.RemoveSelection');

    if (isStacked) {
        addIcon.removeClass('fa-arrow-circle-left').addClass('fa-arrow-circle-up');
        removeIcon.removeClass('fa-arrow-circle-right').addClass('fa-arrow-circle-down');
    } else {
        addIcon.removeClass('fa-arrow-circle-up').addClass('fa-arrow-circle-left');
        removeIcon.removeClass('fa-arrow-circle-down').addClass('fa-arrow-circle-right');
    }

    if (hasUnselected) {
        addIcon.show();
        removeIcon.hide();
        grid.find('.UnselectAll').show();
    } else if (hasSelected) {
        addIcon.hide();
        removeIcon.show();
        grid.find('.UnselectAll').show();
    } else {
        addIcon.hide();
        removeIcon.hide();
        grid.find('.UnselectAll').hide();
    }
}

function updateLayout() {
    let infoSection = $('details').filter(function() {
        let text = $(this).find('summary').text().toLowerCase();
        return text.indexOf('information') > -1 || text.indexOf('identification') > -1;
    }).first();
    
    if (infoSection.length === 0) infoSection = $('details').first();

    let grids = $('.selectionsGrid');
    
    if (infoSection.length > 0) {
        if (!infoSection.prop('open')) {
            grids.addClass('stacked');
        } else {
            grids.removeClass('stacked');
        }
    }
    
    grids.each(function() {
        updateGridState($(this));
    });
}

function deSelectAll(grid) {
    grid.find('.SelectedItems option').prop('selected', false);
    grid.find('.UnselectedItems option').prop('selected', false);
    updateGridState(grid);
}

function normalize(str) {
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}
function sortSelect(select) {
    select.each(function () {
        let select = $(this);
        select.html(select.find('option').sort(function (option1, option2) {
            return $(option1).text() < $(option2).text() ? -1 : 1;
        }))
    });
}

function sortAllSelect() {
    $('select').each(function () {
        let select = $(this);
        select.html(select.find('option').sort(function (option1, option2) {
            return $(option1).text() < $(option2).text() ? -1 : 1;
        }))
    });
}

function scrollTo(selectObj, optionTop) {
    var selectTop = selectObj.offset().top;
    selectObj.scrollTop(selectObj.scrollTop() + (optionTop - selectTop));
}