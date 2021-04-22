function userDataDependent(userObj) {
    $(function () {

        // Data for treeview
        //var defaultData1 = [
        //    {
        //        text: 'Dossier 1',
        //        nodes: [
        //            {
        //                text: 'Groupe 1',
        //                nodes: [
        //                    {
        //                        text: 'Membre 1',
        //                    },
        //                    {
        //                        text: 'Membre 2',
        //                    }
        //                ]
        //            },
        //            {
        //                text: 'Groupe 2',
        //            }
        //        ]
        //    },
        //    {
        //        text: 'Dossier 2',
        //    },
        //    {
        //        text: 'Dossier 3',
        //    }
        //];

        //var defaultData2 = [
        //    {
        //        "text": "root",
        //        "nodes": [
        //            {
        //                "text": "node0",
        //                "nodes": []
        //            },
        //            {
        //                "text": "node1",
        //                "nodes": []
        //            },
        //            {
        //                "text": "node2",
        //                "nodes": [
        //                    {
        //                        "nodes": []
        //                    },
        //                    {
        //                        "text": "node21",
        //                        "nodes": [
        //                            {
        //                                "text": "node210",
        //                                "nodes": []
        //                            },
        //                            {
        //                                "text": "node211",
        //                                "nodes": []
        //                            }
        //                        ]
        //                    }
        //                ]
        //            },
        //            {
        //                "text": "node3",
        //                "nodes": [
        //                    {
        //                        "text": "node30",
        //                        "nodes": []
        //                    }
        //                ]
        //            }
        //        ]
        //    }
        //];

        var defaultData3 = JSON.parse('[' + JSON.stringify(userObj) + ']');

        // options
        $('#mytreeview').treeview({
            color: "#428bca",
            expandIcon: 'fa fa-plus',
            collapseIcon: 'fa fa-minus',
            showBorder: false,
            highlightSelected: true,
            data: defaultData3
        });

        // collapses all nodes
        $('#mytreeview').treeview('collapseAll', { silent: true });

        // Action when node is selected
        $('#mytreeview').on('nodeSelected', function (event, data) {
            // TODO: show node content
            window.alert("You have selected: " + data.text + "\n");
        });

    });
}
