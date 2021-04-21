$(function () {

    // Data for treeview
    var defaultData = [
        {
            text: 'Dossier 1',
            nodes: [
                {
                    text: 'Groupe 1',
                    nodes: [
                        {
                            text: 'Membre 1',
                        },
                        {
                            text: 'Membre 2',
                        }
                    ]
                },
                {
                    text: 'Groupe 2',
                }
            ]
        },
        {
            text: 'Dossier 2',
        },
        {
            text: 'Dossier 3',
        }
    ];

    // options
    $('#mytreeview').treeview({
        color: "#428bca",
        expandIcon: 'fa fa-plus',
        collapseIcon: 'fa fa-minus',
        showBorder: false,
        highlightSelected: true,
        data: defaultData
    });

    // collapses all nodes
    $('#mytreeview').treeview('collapseAll', { silent: true });

    // Action when node is selected
    $('#mytreeview').on('nodeSelected', function (event, data) {
        // TODO: show node content
        window.alert("You have selected: " + data.text + "\n");
        console.log(data);
    });

});