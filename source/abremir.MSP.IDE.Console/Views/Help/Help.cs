using System.Collections.Generic;
using System.Linq;
using Terminal.Gui.Trees;

namespace abremir.MSP.IDE.Console.Views
{
    internal class Help(IReadOnlyCollection<HelpItem> children) : TreeNode
    {
        private readonly IList<ITreeNode> _children = children.Cast<ITreeNode>().ToList();

        public override IList<ITreeNode> Children => _children;
        public override string Text => "Instruction set, Errors and Warnings";
    }
}
