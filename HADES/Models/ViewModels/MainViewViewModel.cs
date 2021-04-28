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
        public List<RootDataInformation> ADRoot { get; set; }

        public TreeNode<string> ADRootTreeNode { get; set; }
        
        public string ADRootTreeNodeJson { get; set; }

        public string SelectedPath { get; set; }

        public string ADConnectionError { get; set; }
    }
}
