using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Util.ModelAD;
using Newtonsoft.Json;

namespace HADES.Models
{
    public class MainViewViewModel
    {
        public TreeNode<string> ADRoot { get; set; }

        public string ADRootJson { get; set; }

        public TreeNode<string> SelectedNode { get; set; }

        public int SelectedNodeId { get; set; }

        public string ADConnectionError { get; set; }

    }
}
