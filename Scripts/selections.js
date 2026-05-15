$(document).ready(initUI);
function initUI() {
    sortAllSelect();
    $('.selectionsGrid').each(function() {
        deSelectAll($(this));
    });
    $(document).on('change', '.UnselectedItems', function (e) {
        let grid = $(this).closest('.selectionsGrid');
        grid.find('.SelectedItems option:selected').prop("selected", false);
        updateGridState(grid);
        e.preventDefault();
    });
    $(document).on('change', '.SelectedItems', function (e) {
        let grid = $(this).closest('.selectionsGrid');
        grid.find('.UnselectedItems option:selected').prop("selected", false);
        updateGridState(grid);
        e.preventDefault();
    });
    $(document).on('submit', 'form', function () {
        $('.SelectedItems option').prop('selected', true);
    });
    $(document).on('click', '.SelectionIcon', function () {
        let grid = $(this).closest('.selectionsGrid');
        let unselected = grid.find('.UnselectedItems').find('option:selected');
        let selected = grid.find('.SelectedItems').find('option:selected');
        if (unselected.length > 0) {
            unselected.each(function () {
                $(this).remove();
                $(this).prop('selected', false);
                grid.find('.SelectedItems').append($(this));
            });
            sortSelect(grid.find(".SelectedItems"));
            grid.find(".SelectedItems").focus();
        } else if (selected.length > 0) {
            selected.each(function () {
                $(this).remove();
                $(this).prop('selected', false);
                grid.find('.UnselectedItems').append($(this));
            });
            sortSelect(grid.find(".UnselectedItems"));
            grid.find(".UnselectedItems").focus();
        }
        deSelectAll(grid);
    });
    $(document).on('click', '.UnselectAll', function () {
        let grid = $(this).closest('.selectionsGrid');
        deSelectAll(grid);
    });
    $(document).on('toggle', 'details', function () {
        updateLayout();
    });
    $(document).on('click', 'summary', function() {
        setTimeout(updateLayout, 10);
    });
    updateLayout();
}
function updateGridState(grid) {
    let hasSelected = grid.find('.SelectedItems option:selected').length > 0;
    let hasUnselected = grid.find('.UnselectedItems option:selected').length > 0;
    let isStacked = grid.hasClass('stacked');
    let icon = grid.find('.SelectionIcon');
    let xIcon = grid.find('.UnselectAll');
    icon.removeClass('fa-arrow-circle-left fa-arrow-circle-right fa-arrow-circle-up fa-arrow-circle-down');
    if (hasUnselected) {
        icon.addClass(isStacked ? 'fa-arrow-circle-up' : 'fa-arrow-circle-left').show();
        xIcon.show();
    } else if (hasSelected) {
        icon.addClass(isStacked ? 'fa-arrow-circle-down' : 'fa-arrow-circle-right').show();
        xIcon.show();
    } else {
        icon.hide();
        xIcon.hide();
    }
}
function updateLayout() {
    let infoSection = $('#info_section, #identification_section, details').first();
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