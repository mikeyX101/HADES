$(function () {

    // Data for treeview
    var defaultData = [
        {
            text: 'Dossier 1',
            href: '#dossier1',
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

    // customization options
    $('#treeview1').treeview({
        color: "#428bca",
        expandIcon: 'fa fa-plus',
        collapseIcon: 'fa fa-minus',
        showBorder: false,
        highlightSelected: true,
        data: defaultData
    });

    // Action when node is selected
    $('#treeview1').on('nodeSelected', function (event, data) {
        window.alert("You have selected: " + data.text + "\n");
        console.log(data);
    });

});