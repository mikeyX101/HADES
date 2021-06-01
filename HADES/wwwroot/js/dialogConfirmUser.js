
/**
 * Show treeview
 * @param {any} userObj
 * @param {any} nodeName
 */

/**
 * Setup dialog settings after document is ready
 */
$(function () {
    $("#dialog-confirmation-remove").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        buttons: {
            OK: function () {
                $(this).dialog("close");
                $(this).data('form').submit();
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
    });

    $("#dialog-confirmation-delete").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        buttons: {
            OK: function () {
                $(this).dialog("close");
                $(this).data('form').submit();
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

});

/**
 * delete User
 * @param {any} form
 */
function deleteUser(form) {

    // confirmation dialog{
    $("#dialog-confirmation-delete").data('form', form).dialog("open");


    return false; // pour ne pas faire de submit avant d'avoir eu la réponse de la boite de dialog
}

/**
 * remove User
 * @param {any} form
 */
function removeUser(form) {

    // confirmation dialog{
    $("#dialog-confirmation-remove").data('form', form).dialog("open");


    return false; // pour ne pas faire de submit avant d'avoir eu la réponse de la boite de dialog
}

