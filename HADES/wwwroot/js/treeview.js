function userDataDependent(userObj) {
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

        $('#mytreeview').treeview('selectNode', [0, { silent: true }]);

        // Action when node is selected
        $('#mytreeview').on('nodeSelected', function (event, data) {
            // TODO: show node content
            window.alert("You have selected: " + data.nodes + "\n");
        });

    });
}
