var selectedPath;
var selectedPathForContent;
var selectedNode;
var selectedDepth;

function showTreeView(userObj, nodeName) {
    $(function () {
        // conversion Json en array Json
        var rootData = JSON.parse('[' + JSON.stringify(userObj) + ']');

        // configure treeview options
        $('#mytreeview').treeview({
            color: "#428bca",
            expandIcon: 'fa fa-plus',
            collapseIcon: 'fa fa-minus',
            showBorder: false,
            highlightSelected: true,
            highlightSearchResults: false,
            data: rootData
        });

        // search
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
                    $('#content').html(msg);
                }
            });

            // If Group is selected select his parent ou
            if (selectedDepth > 2) {
                $('#mytreeview').treeview('selectNode', [selectedNode.parentId, { silent: true }]);
            }
            
        });
        
    });
    
}

// Set icons for home, ou and group
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
        for (var j = 0; j < groups.length; j++) {
            $('#mytreeview').treeview('getNode', groups[j].nodeId).icon = 'fa fa-users';
        }
    }
}
