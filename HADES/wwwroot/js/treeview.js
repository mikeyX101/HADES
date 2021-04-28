var selectedPath;

function showTreeView(userObj) {
    $(function () {
        var rootData = JSON.parse('[' + JSON.stringify(userObj) + ']');

        // options
        $('#mytreeview').treeview({
            color: "#428bca",
            expandIcon: 'fa fa-plus',
            collapseIcon: 'fa fa-minus',
            showBorder: false,
            highlightSelected: true,
            data: rootData
        });

        // expand root node 1 level deep
        $('#mytreeview').treeview('expandNode', [0, { levels: 1, silent: true }]);

        // select root node
        $('#mytreeview').treeview('selectNode', [0, { silent: true }]);

        // Action when node is selected
        $('#mytreeview').on('nodeSelected', function (event, data) {
            selectedPath = "";
            var node = data;
            selectedPath = "/" + node.text;
            while (typeof node.parentId !== 'undefined') {
                node = $('#mytreeview').treeview('getNode', node.parentId);
                selectedPath = "/" + node.text + selectedPath;
            } 
            $.ajax({
                url: '/Home/UpdateContent',
                type: "GET",
                data: { id: selectedPath },
                success: function (msg) {
                    $('#content').html(msg);
                }
            });
        });
    });
}

