using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests2;

namespace CodeKicker.BBCode.Tests
{
    [TestClass]
    [PexClass(MaxRuns = 1000000000, MaxRunsWithoutNewTests = 1000000000, Timeout = 1000000000, MaxExecutionTreeNodes = 1000000000, MaxBranches = 1000000000, MaxWorkingSet = 1000000000, MaxConstraintSolverMemory = 1000000000, MaxStack = 1000000000, MaxConditions = 1000000000)]
    public partial class SyntaxTreeNodeTest
    {
        [PexMethod]
        public void EqualTreesHaveEqualBBCode(out string bbCode1, out string bbCode2)
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCodeTestUtil.GetAnyTree();
            bbCode1 = tree1.ToBBCode();
            bbCode2 = tree2.ToBBCode();
            Assert.AreEqual(tree1 == tree2, bbCode1 == bbCode2);
        }
        [PexMethod]
        public void UnequalTexthasUnequalTrees(out string text1, out string text2)
        {
            var tree1 = BBCodeTestUtil.GetAnyTree();
            var tree2 = BBCodeTestUtil.GetAnyTree();
            text1 = tree1.ToText();
            text2 = tree2.ToText();
            if (text1 != text2) Assert.IsTrue(tree1 != tree2);
        }
    }
}
