var selectedPath;
var selectedPathForContent;
var selectedNode;
var depth;

function showTreeView(userObj) {
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
            data: rootData
        });

        // expand root node only 1 level deep
        $('#mytreeview').treeview('expandNode', [0, { levels: 1, silent: true }]);

        // select root node
        $('#mytreeview').treeview('selectNode', [0, { silent: true }]);

        // Action when node is selected
        $('#mytreeview').on('nodeSelected', function (event, data) {
            // Get selectedPath and depth of selected node
            selectedPath = "";
            selectedNode = data;
            selectedPath = "/" + selectedNode.text;
            depth = 1;
            while (typeof selectedNode.parentId !== 'undefined') {
                selectedNode = $('#mytreeview').treeview('getNode', selectedNode.parentId);
                selectedPath = "/" + selectedNode.text + selectedPath;
                depth++;
            } 

            // backup selectedNode
            selectedNode = data;
            selectedPathForContent = selectedPath;

            // If Group is selected set selectedPathForContent to his parent ou
            if (depth > 2) {
                // Group is selected so set selectedPathForContent to his parent ou
                var splitSelectedPath = selectedPath.split("/");
                selectedPathForContent = "";
                for (var i = 1; i <= splitSelectedPath.length - 2; i++) {
                    selectedPathForContent += "/" + splitSelectedPath[i];
                }
            }

            // update content of selected node to display
            $.ajax({
                url: '/Home/UpdateContent',
                type: "GET",
                data: { id: selectedPathForContent },
                success: function (msg) {
                    $('#content').html(msg);
                }
            });

            // If Group is selected select his parent ou
            if (depth > 2) {
                $('#mytreeview').treeview('selectNode', [selectedNode.parentId, { silent: true }]);
            }
        });

    });
}

