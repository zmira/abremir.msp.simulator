using System.Collections.Generic;
using System.Linq;
using Terminal.Gui.Trees;

namespace abremir.MSP.IDE.Console.Views
{
    internal class Help : TreeNode
    {
        private readonly IList<ITreeNode> _children;

        public Help(IReadOnlyCollection<HelpItem> children)
        {
            _children = children.Cast<ITreeNode>().ToList();
        }

        public override IList<ITreeNode> Children => _children;
        public override string Text => "Instruction set, Errors and Warnings";
    }
}
