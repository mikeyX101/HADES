using HADES.Util;
using HADES.Util.ModelAD;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HADES.Models
{
    public class MainViewViewModel
    {
        public List<RootDataInformation> ADRoot { get; set; }

        public TreeNode<string> ADRootTreeNode { get; set; }

        public GroupAD GroupAD { get; set; }

        public string ADRootTreeNodeJson { get; set; }

        public string SelectedPath { get; set; }

        public string ADConnectionError { get; set; }

        public string CreateButtonLabel { get; set; }

        public string EditLinkLabel { get; set; }

        public string DataTarget { get; set; }

        public string SelectedNodeName { get; set; }

        public string SelectedContentName { get; set; }

        public string ExpandedNodesId { get; set; }

        public string BeforeEditMembers { get; set; }

        public string SelectedMembers { get; set; }

        public string OuGroup { get; set; }
        public ADManager ADManager { get; set; }
        public string UsersAD { get; set; }


        [Required]
        [OuName]
        public string NewName { get; set; }

    }
}
