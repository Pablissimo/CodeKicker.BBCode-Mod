using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests2;

namespace CodeKicker.BBCode.Tests
{
    [TestClass]
    [PexClass(typeof(SyntaxTreeVisitor), MaxRuns = 1000000000, MaxRunsWithoutNewTests = 1000000000, Timeout = 1000000000, MaxExecutionTreeNodes = 1000000000, MaxBranches = 1000000000, MaxWorkingSet = 1000000000, MaxConstraintSolverMemory = 1000000000, MaxStack = 1000000000, MaxConditions = 1000000000)]
    public partial class SyntaxTreeVisitorTest
    {
        [PexMethod]
        public void DefaultVisitorModifiesNothing()
        {
            var tree = BBCodeTestUtil.GetAnyTree();
            var tree2 = new SyntaxTreeVisitor().Visit(tree);
            Assert.IsTrue(ReferenceEquals(tree, tree2));
        }

        [PexMethod]
        public void IdentityModifiedTreesAreEqual()
        {
            var tree = BBCodeTestUtil.GetAnyTree();
            var tree2 = new IdentitiyModificationSyntaxTreeVisitor().Visit(tree);
            Assert.IsTrue(tree == tree2);
        }

        [PexMethod]
        public void TextModifiedTreesAreNotEqual()
        {
            var tree = BBCodeTestUtil.GetAnyTree();
            var tree2 = new TextModificationSyntaxTreeVisitor().Visit(tree);
            Assert.IsTrue(tree != tree2);
        }

        class IdentitiyModificationSyntaxTreeVisitor : SyntaxTreeVisitor
        {
            protected internal override SyntaxTreeNode Visit(TextNode node)
            {
                if (!PexChoose.Value<bool>("x")) return base.Visit(node);

                return new TextNode(node.Text, node.HtmlTemplate);
            }
            protected internal override SyntaxTreeNode Visit(SequenceNode node)
            {
                var baseResult = base.Visit(node);
                if (!PexChoose.Value<bool>("x")) return baseResult;
                return baseResult.SetSubNodes(baseResult.SubNodes.ToList());
            }
            protected internal override SyntaxTreeNode Visit(TagNode node)
            {
                var baseResult = base.Visit(node);
                if (!PexChoose.Value<bool>("x")) return baseResult;
                return baseResult.SetSubNodes(baseResult.SubNodes.ToList());
            }
        }

        class TextModificationSyntaxTreeVisitor : SyntaxTreeVisitor
        {
            protected internal override SyntaxTreeNode Visit(TextNode node)
            {
                return new TextNode(node.Text + "x", node.HtmlTemplate);
            }
            protected internal override SyntaxTreeNode Visit(SequenceNode node)
            {
                var baseResult = base.Visit(node);
                return baseResult.SetSubNodes(baseResult.SubNodes.Concat(new[] { new TextNode("y") }));
            }
            protected internal override SyntaxTreeNode Visit(TagNode node)
            {
                var baseResult = base.Visit(node);
                return baseResult.SetSubNodes(baseResult.SubNodes.Concat(new[] { new TextNode("z") }));
            }
        }
    }
}
