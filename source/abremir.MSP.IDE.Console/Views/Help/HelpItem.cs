using System.Collections.Generic;
using System.Linq;
using Terminal.Gui.Trees;

namespace abremir.MSP.IDE.Console.Views
{
    internal class HelpItem : TreeNode
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Syntax { get; set; }
        public IEnumerable<HelpItem>? Items { get; set; }
        public int? Code { get; set; }

        public override string Text => Title ?? string.Empty;
        public override IList<ITreeNode> Children => Items?.Cast<ITreeNode>().ToList() ?? [];
    }
}
