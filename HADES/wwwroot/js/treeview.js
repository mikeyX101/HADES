var selectedPath;
var selectedPathForContent;
var selectedNode;
var selectedDepth;
var selectedContentName;
var isValid = false;

/**
 * Show treeview
 * @param {any} userObj
 * @param {any} nodeName
 */
function showTreeView(userObj, nodeName) {
    $(function () {
        // conversion Json en array Json
        var rootData = JSON.parse('[' + JSON.stringify(userObj) + ']');

        // configure treeview options
        $('#mytreeview').treeview({
            color: "#428bca",
            expandIcon: 'fa fa-plus',
            collapseIcon: 'fa fa-minus',
            emptyIcon: 'fa',
            showBorder: false,
            highlightSelected: true,
            highlightSearchResults: false,
            data: rootData
        });

        // search node in treeview
        var foundNodes = $('#mytreeview').treeview('search', [nodeName, {
            ignoreCase: false,     // case insensitive
            exactMatch: true,    // like or equals
            revealResults: true,  // reveal matching nodes
        }]);

        // expand node 1 level deep
        $('#mytreeview').treeview('expandNode', [foundNodes[0].nodeId, { levels: 1, silent: true }]);

        // set icons
        setIcons();

        // select node
        $('#mytreeview').treeview('selectNode', [foundNodes[0].nodeId, { silent: true }]);

        // Action when node is selected
        $('#mytreeview').on('nodeSelected', function (event, data) {
            // Get selectedPath and depth of selected node
            selectedPath = "";
            selectedNode = data;
            selectedPath = "/" + selectedNode.text;
            selectedDepth = 1;
            while (typeof selectedNode.parentId !== 'undefined') {
                selectedNode = $('#mytreeview').treeview('getNode', selectedNode.parentId);
                selectedPath = "/" + selectedNode.text + selectedPath;
                selectedDepth++;
            } 

            // backup selectedNode
            selectedNode = data;
            selectedPathForContent = selectedPath;

            // If Group is selected set selectedPathForContent to his parent ou
            if (selectedDepth > 2) {
                // Group is selected so set selectedPathForContent to his parent ou
                var splitSelectedPath = selectedPath.split("/");
                selectedPathForContent = "";
                for (var i = 1; i <= splitSelectedPath.length - 2; i++) {
                    selectedPathForContent += "/" + splitSelectedPath[i];
                }
            }

            // update selected node content to display
            $.ajax({
                url: '/Home/UpdateContent',
                type: "GET",
                data: { selectedPathForContent: selectedPathForContent },
                success: function (msg) {
                    $('#main').html(msg);
                }
            });

            // If Group is selected select his parent ou
            if (selectedDepth > 2) {
                $('#mytreeview').treeview('selectNode', [selectedNode.parentId, { silent: true }]);
            }
            
        });
        
    });
    
}

/**
 * Set icons for home, ou and group
 * */
function setIcons() {
    var node = $('#mytreeview').treeview('getNode', 0);
    // set root icon
    node.icon = 'fa fa-home';
    // set folder and group icons
    var folders = node.nodes;
    var groups;
    for (var i = 0; i < folders.length; i++) {
        $('#mytreeview').treeview('getNode', folders[i].nodeId).icon = 'fa fa-folder';
        groups = folders[i].nodes;
        if (typeof groups !== 'undefined') {
            for (var j = 0; j < groups.length; j++) {
                $('#mytreeview').treeview('getNode', groups[j].nodeId).icon = 'fa fa-users';
            }
        }
    }
}

/**
 * Setup dialog settings after document is ready
 */
$(function () {
    $("#dialog-error-delete").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        buttons: {
            OK: function () { $(this).dialog("close"); }
        },
    });

    $("#dialog-confirmation-delete").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        buttons: {
            OK: function () {
                $(this).dialog("close");
                $(this).data('form');
                $.ajax({
                    url: '/Home/Delete',
                    type: "POST",
                    data: {
                        SelectedPath: $(this).data('form')[1].value,
                        SelectedContentName: $(this).data('form')[2].value
                    },
                    success: function (msg) {
                        $('#main').html(msg);
                    }
                });
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    // focus on modal input field after dialog shows up
    $('#createOuModal').on('shown.bs.modal', function () {
        $('#NewName').focus();
    })  
});

/**
 * delete OU
 * @param {any} form
 */
function deleteOU(form) {

    // validation here
    selectedPath = form[1].value;
    selectedContentName = form[2].value;

    // search for selectedContentName
    var foundNodes = $('#mytreeview').treeview('search', [selectedContentName, {
        ignoreCase: false,      // case insensitive
        exactMatch: true,       // like or equals
        revealResults: true,    // reveal matching nodes
    }]);

    // OU is valid if it does not contain Groups(il a un parent qui est root et n'a pas d'enfant)
    isValid = foundNodes[0] && foundNodes[0].parentId == 0 && typeof foundNodes[0].nodes === 'undefined';

    if (isValid) {
        // confirmation dialog
        $("#dialog-confirmation-delete").data('form', form).dialog("open");
    }
    else {
        // error dialog
        $("#dialog-error-delete").dialog("open");
    }
    
    return false; // pour ne pas faire de submit avant d'avoir eu la réponse de la boite de dialog
}

function deleteGroup(form) {

    selectedContentName = form[2].value;

    // search for selectedContentName
    var foundNodes = $('#mytreeview').treeview('search', [selectedContentName, {
        ignoreCase: false,      // case insensitive
        exactMatch: true,       // like or equals
        revealResults: true,    // reveal matching nodes
    }]);

    // Group is valid if 
    isValid = foundNodes[0] && foundNodes[0].parentId !== 0 && typeof foundNodes[0].nodes === 'undefined';

    if (isValid) {
        // confirmation dialog
        $("#dialog-confirmation-delete").data('form', form).dialog("open");
    }
    else {
        // error dialog
        $("#dialog-error-delete").dialog("open");
    }

    return false; // pour ne pas faire de submit avant d'avoir eu la réponse de la boite de dialog

}

/**
 * Submit the form for deleting OU or Group
 * @param {any} form
 */
function formSubmit(form) {
    if (isOU(form[1].value)) {
        deleteOU(form)
    } 
    /* TODO : implémenter la fonctionnalité deleteGroup */
    if (isGroup(form[1].value)) {
        deleteGroup(form)
        //alert("Implémenter la fonctionalité de suppression d'un groupe")
    }
    return false; // pour ne pas faire de submit avant d'avoir eu la réponse de la boite de dialog
}

function isGroup(path) {
    var splitPath = path.split('/');
    return splitPath.length == 3;
}

function isOU(path) {
    var splitPath = path.split('/');
    return splitPath.length < 3;
}
