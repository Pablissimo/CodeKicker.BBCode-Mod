using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode;
using CodeKicker.BBCode.SyntaxTree;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests2
{
    [TestClass]
    [PexClass(MaxRuns = 1000000000, MaxRunsWithoutNewTests = 1000000000, Timeout = 1000000000, MaxExecutionTreeNodes = 1000000000, MaxBranches = 1000000000, MaxWorkingSet = 1000000000, MaxConstraintSolverMemory = 1000000000, MaxStack = 1000000000, MaxConditions = 1000000000)]
    public partial class BBCodeParserTest
    {
        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual("", BBEncodeForTest("", ErrorMode.Strict));
        }

        [TestMethod]
        public void Test2()
        {
            Assert.AreEqual("a", BBEncodeForTest("a", ErrorMode.Strict));
            Assert.AreEqual(" a b c ", BBEncodeForTest(" a b c ", ErrorMode.Strict));
        }

        [TestMethod]
        public void Test3()
        {
            Assert.AreEqual("<b></b>", BBEncodeForTest("[b][/b]", ErrorMode.Strict));
        }

        [TestMethod]
        public void Test4()
        {
            Assert.AreEqual("text<b>text</b>text", BBEncodeForTest("text[b]text[/b]text", ErrorMode.Strict));
        }

        [TestMethod]
        public void Test5()
        {
            Assert.AreEqual("<a href=\"http://example.org/path?name=value\">text</a>", BBEncodeForTest("[url=http://example.org/path?name=value]text[/url]", ErrorMode.Strict));
        }

        [TestMethod]
        public void LeafElementWithoutContent()
        {
            Assert.AreEqual("xxxnameyyy", BBEncodeForTest("[placeholder=name]", ErrorMode.Strict));
            Assert.AreEqual("xxxyyy", BBEncodeForTest("[placeholder=]", ErrorMode.Strict));
            Assert.AreEqual("xxxyyy", BBEncodeForTest("[placeholder]", ErrorMode.Strict));
            Assert.AreEqual("axxxyyyb", BBEncodeForTest("a[placeholder]b", ErrorMode.Strict));
            Assert.AreEqual("<b>a</b>xxxyyy<b>b</b>", BBEncodeForTest("[b]a[/b][placeholder][b]b[/b]", ErrorMode.Strict));

            try
            {
                BBEncodeForTest("[placeholder][/placeholder]", ErrorMode.Strict);
                Assert.Fail();
            }
            catch (BBCodeParsingException)
            {
            }

            try
            {
                BBEncodeForTest("[placeholder/]", ErrorMode.Strict);
                Assert.Fail();
            }
            catch (BBCodeParsingException)
            {
            }
        }

        [TestMethod]
        public void ImgTagHasNoContent()
        {
            Assert.AreEqual("<img src=\"url\" />", BBEncodeForTest("[img]url[/img]", ErrorMode.Strict));
        }

        [TestMethod]
        public void ListItemIsAutoClosed()
        {
            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<ul><li>item</li></ul>", BBEncodeForTest("[list][*]item[/list]", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item[/*]", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<li><li>item</li></li>", BBEncodeForTest("[*][*]item", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, false));
            Assert.AreEqual("<li>1<li>2</li></li>", BBEncodeForTest("[*]1[*]2", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, false));

            Assert.AreEqual("<li></li>item", BBEncodeForTest("[*]item", ErrorMode.Strict, BBTagClosingStyle.LeafElementWithoutContent, false));
            Assert.AreEqual("<ul><li></li>item</ul>", BBEncodeForTest("[list][*]item[/list]", ErrorMode.Strict, BBTagClosingStyle.LeafElementWithoutContent, false));
            Assert.AreEqual("<li></li><li></li>item", BBEncodeForTest("[*][*]item", ErrorMode.Strict, BBTagClosingStyle.LeafElementWithoutContent, false));
            Assert.AreEqual("<li></li>1<li></li>2", BBEncodeForTest("[*]1[*]2", ErrorMode.Strict, BBTagClosingStyle.LeafElementWithoutContent, false));

            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<ul><li>item</li></ul>", BBEncodeForTest("[list][*]item[/list]", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>item</li>", BBEncodeForTest("[*]item[/*]", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li></li><li>item</li>", BBEncodeForTest("[*][*]item", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>1</li><li>2</li>", BBEncodeForTest("[*]1[*]2", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>1<b>a</b></li><li>2</li>", BBEncodeForTest("[*]1[b]a[/b][*]2", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true));
            Assert.AreEqual("<li>1<b>a</b></li><li>2</li>", BBEncodeForTest("[*]1[b]a[*]2", ErrorMode.ErrorFree, BBTagClosingStyle.AutoCloseElement, true));

            try
            {
                BBEncodeForTest("[*]1[b]a[*]2", ErrorMode.Strict, BBTagClosingStyle.AutoCloseElement, true);
                Assert.Fail();
            }
            catch (BBCodeParsingException)
            {
            }
        }

        [TestMethod]
        public void TagContentTransformer()
        {
            var parser = new BBCodeParser(new[]
                {
                    new BBTag("b", "<b>", "</b>", true, true, content => content.Trim()), 
                });

            Assert.AreEqual("<b>abc</b>", parser.ToHtml("[b] abc [/b]"));
        }

        [TestMethod]
        public void AttributeValueTransformer()
        {
            var parser = new BBCodeParser(ErrorMode.Strict, null, new[]
                {
                    new BBTag("font", "<span style=\"${color}${font}\">", "</span>", true, true,
                        new BBAttribute("color", "color", attributeRenderingContext => string.IsNullOrEmpty(attributeRenderingContext.AttributeValue) ? "" : "color:" + attributeRenderingContext.AttributeValue + ";"),
                        new BBAttribute("font", "font", attributeRenderingContext => string.IsNullOrEmpty(attributeRenderingContext.AttributeValue) ? "" : "font-family:" + attributeRenderingContext.AttributeValue + ";")),
                });

            Assert.AreEqual("<span style=\"color:red;font-family:Arial;\">abc</span>", parser.ToHtml("[font color=red font=Arial]abc[/font]"));
            Assert.AreEqual("<span style=\"color:red;\">abc</span>", parser.ToHtml("[font color=red]abc[/font]"));
        }

        //the parser may never ever throw an exception other that BBCodeParsingException for any non-null input
        [PexMethod]
        public void NoCrash(ErrorMode errorMode, [PexAssumeNotNull] string input, BBTagClosingStyle listItemBbTagClosingStyle, out string output)
        {
            PexAssume.EnumIsDefined(errorMode);
            PexAssume.EnumIsDefined(listItemBbTagClosingStyle);
            try
            {
                output = BBEncodeForTest(input, errorMode, listItemBbTagClosingStyle, false);
                Assert.IsNotNull(output);
            }
            catch (BBCodeParsingException)
            {
                Assert.AreNotEqual(ErrorMode.ErrorFree, errorMode);
                output = null;
            }
        }

        [PexMethod]
        public void ErrorFreeModeAlwaysSucceeds([PexAssumeNotNull] string input, out string output)
        {
            output = BBEncodeForTest(input, ErrorMode.ErrorFree);
        }

        //no script-tags may be contained in the output under any circumstances
        [PexMethod]
        public void NoScript_AnyInput(ErrorMode errorMode, [PexAssumeNotNull] string input)
        {
            PexAssume.EnumIsDefined(errorMode);
            try
            {
                var output = BBEncodeForTest(input, errorMode);
                PexAssert.IsTrue(!output.Contains("<script"));
            }
            catch (BBCodeParsingException)
            {
                PexAssume.Fail();
            }
        }

        //no script-tags may be contained in the output under any circumstances
        [PexMethod]
        public void NoScript_AnyInput_Tree()
        {
            var parser = BBCodeTestUtil.GetParserForTest(ErrorMode.ErrorFree, true, BBTagClosingStyle.AutoCloseElement, false);
            var tree = BBCodeTestUtil.CreateRootNode(parser.Tags.ToArray());
            var output = tree.ToHtml();
            PexAssert.IsTrue(!output.Contains("<script"));
        }

        //no html-chars may be contained in the output under any circumstances
        [PexMethod]
        public void NoHtmlChars_AnyInput(ErrorMode errorMode, [PexAssumeNotNull] string input)
        {
            PexAssume.EnumIsDefined(errorMode);
            try
            {
                var output = BBCodeTestUtil.SimpleBBEncodeForTest(input, errorMode);
                PexObserve.ValueForViewing("output", output);
                PexAssert.IsTrue(output.IndexOf('<') == -1);
                PexAssert.IsTrue(output.IndexOf('>') == -1);
            }
            catch (BBCodeParsingException)
            {
                PexAssume.Fail();
            }
        }

        [PexMethod]
        public void NoScript_FixedInput(ErrorMode errorMode)
        {
            PexAssume.EnumIsDefined(errorMode);
            Assert.IsFalse(BBEncodeForTest("<script>", errorMode).Contains("<script"));
        }

        [PexMethod]
        public void NoScriptInAttributeValue(ErrorMode errorMode)
        {
            PexAssume.EnumIsDefined(errorMode);
            var encoded = BBEncodeForTest("[url=<script>][/url]", errorMode);
            Assert.IsFalse(encoded.Contains("<script"));
        }

        //1. given a syntax tree, encode it in BBCode, parse it back into a second syntax tree and ensure that both are exactly equal
        //2. given any syntax tree, the BBCode it represents must be parsable without error
        [PexMethod]
        public void Roundtrip(ErrorMode errorMode, out string bbcode, out string output)
        {
            PexAssume.EnumIsDefined(errorMode);

            var parser = BBCodeTestUtil.GetParserForTest(errorMode, false, BBTagClosingStyle.AutoCloseElement, false);
            var tree = BBCodeTestUtil.CreateRootNode(parser.Tags.ToArray());
            bbcode = tree.ToBBCode();
            var tree2 = parser.ParseSyntaxTree(bbcode);
            output = tree2.ToHtml();
            Assert.IsTrue(tree == tree2);
        }

        //given a BBCode-string, parse it into a syntax tree, encode the tree in BBCode, parse it back into a second sytax tree and ensure that both are exactly equal
        [PexMethod]
        public void Roundtrip2(ErrorMode errorMode, [PexAssumeNotNull] string input, out string bbcode, out string output)
        {
            PexAssume.EnumIsDefined(errorMode);

            var parser = BBCodeTestUtil.GetParserForTest(errorMode, false, BBTagClosingStyle.AutoCloseElement, false);
            SequenceNode tree;
            try
            {
                tree = parser.ParseSyntaxTree(input);
            }
#pragma warning disable 168
            catch (BBCodeParsingException e)
#pragma warning restore 168
            {
                PexAssume.Fail();
                tree = null;
            }

            bbcode = tree.ToBBCode();
            var tree2 = parser.ParseSyntaxTree(bbcode);
            output = tree2.ToHtml();
            Assert.IsTrue(tree == tree2);
        }

        [PexMethod]
        public void TextNodesCannotBeSplit(ErrorMode errorMode, [PexAssumeNotNull] string input)
        {
            PexAssume.EnumIsDefined(errorMode);

            var parser = BBCodeTestUtil.GetParserForTest(errorMode, true, BBTagClosingStyle.AutoCloseElement, false);
            SequenceNode tree;
            try
            {
                tree = parser.ParseSyntaxTree(input);
            }
#pragma warning disable 168
            catch (BBCodeParsingException e)
#pragma warning restore 168
            {
                PexAssume.Fail();
                return;
            }

            AssertTextNodesNotSplit(tree);
        }

        static void AssertTextNodesNotSplit(SyntaxTreeNode node)
        {
            if (node.SubNodes != null)
            {
                SyntaxTreeNode lastNode = null;
                for (int i = 0; i < node.SubNodes.Count; i++)
                {
                    AssertTextNodesNotSplit(node.SubNodes[i]);
                    if (lastNode != null)
                        Assert.IsFalse(lastNode is TextNode && node.SubNodes[i] is TextNode);
                    lastNode = node.SubNodes[i];
                }
            }
        }

        public static string BBEncodeForTest(string bbCode, ErrorMode errorMode)
        {
            return BBEncodeForTest(bbCode, errorMode, BBTagClosingStyle.AutoCloseElement, false);
        }
        public static string BBEncodeForTest(string bbCode, ErrorMode errorMode, BBTagClosingStyle listItemBbTagClosingStyle, bool enableIterationElementBehavior)
        {
            return BBCodeTestUtil.GetParserForTest(errorMode, true, listItemBbTagClosingStyle, enableIterationElementBehavior).ToHtml(bbCode).Replace("\r", "").Replace("\n", "<br/>");
        }

        [PexMethod]
        public void ToTextDoesNotCrash([PexAssumeNotNull] string input, out string text)
        {
            var parser = BBCodeTestUtil.GetParserForTest(ErrorMode.ErrorFree, true, BBTagClosingStyle.AutoCloseElement, false);
            text = parser.ParseSyntaxTree(input).ToText();
            Assert.IsTrue(text.Length <= input.Length);
        }

        [TestMethod]
        public void StrictErrorMode()
        {
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"", ErrorMode.Strict));
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"[b]abc[/b]", ErrorMode.Strict));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"[b]abc", ErrorMode.Strict));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"abc[0]def", ErrorMode.Strict));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"\", ErrorMode.Strict));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"\x", ErrorMode.Strict));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"[", ErrorMode.Strict));
            Assert.IsFalse(BBCodeTestUtil.IsValid(@"]", ErrorMode.Strict));
        }

        [TestMethod]
        public void CorrectingErrorMode()
        {
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"", ErrorMode.TryErrorCorrection));
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"[b]abc[/b]", ErrorMode.TryErrorCorrection));
            Assert.IsTrue(BBCodeTestUtil.IsValid(@"[b]abc", ErrorMode.TryErrorCorrection));

            Assert.AreEqual(@"\", BBEncodeForTest(@"\", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"\x", BBEncodeForTest(@"\x", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"\", BBEncodeForTest(@"\\", ErrorMode.TryErrorCorrection));
        }

        [TestMethod]
        public void CorrectingErrorMode_EscapeCharsIgnored()
        {
            Assert.AreEqual(@"\\", BBEncodeForTest(@"\\\\", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"\", BBEncodeForTest(@"\", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"\x", BBEncodeForTest(@"\x", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"\", BBEncodeForTest(@"\\", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"[", BBEncodeForTest(@"\[", ErrorMode.TryErrorCorrection));
            Assert.AreEqual(@"]", BBEncodeForTest(@"\]", ErrorMode.TryErrorCorrection));
        }

        [TestMethod]
        public void TextNodeHtmlTemplate()
        {
            var parserNull = new BBCodeParser(ErrorMode.Strict, null, new[]
                {
                    new BBTag("b", "<b>", "</b>"), 
                });
            var parserEmpty = new BBCodeParser(ErrorMode.Strict, "", new[]
                {
                    new BBTag("b", "<b>", "</b>"), 
                });
            var parserDiv = new BBCodeParser(ErrorMode.Strict, "<div>${content}</div>", new[]
                {
                    new BBTag("b", "<b>", "</b>"), 
                });

            Assert.AreEqual(@"", parserNull.ToHtml(@""));
            Assert.AreEqual(@"abc", parserNull.ToHtml(@"abc"));
            Assert.AreEqual(@"abc<b>def</b>", parserNull.ToHtml(@"abc[b]def[/b]"));

            Assert.AreEqual(@"", parserEmpty.ToHtml(@""));
            Assert.AreEqual(@"", parserEmpty.ToHtml(@"abc"));
            Assert.AreEqual(@"<b></b>", parserEmpty.ToHtml(@"abc[b]def[/b]"));

            Assert.AreEqual(@"", parserDiv.ToHtml(@""));
            Assert.AreEqual(@"<div>abc</div>", parserDiv.ToHtml(@"abc"));
            Assert.AreEqual(@"<div>abc</div><b><div>def</div></b>", parserDiv.ToHtml(@"abc[b]def[/b]"));
        }

        [TestMethod]
        public void ContentTransformer_EmptyAttribute_CanChooseValueFromAttributeRenderingContext()
        {
            var parser = BBCodeTestUtil.GetParserForTest(ErrorMode.Strict, true, BBTagClosingStyle.AutoCloseElement, false);

            Assert.AreEqual(@"<a href=""http://codekicker.de"">http://codekicker.de</a>", parser.ToHtml(@"[url2]http://codekicker.de[/url2]"));
            Assert.AreEqual(@"<a href=""http://codekicker.de"">http://codekicker.de</a>", parser.ToHtml(@"[url2=http://codekicker.de]http://codekicker.de[/url2]"));
        }

        [TestMethod]
        public void StopProcessingDirective_StopsParserProcessingTagLikeText_UntilClosingTag()
        {
            var parser = new BBCodeParser(ErrorMode.ErrorFree, null, new[] { new BBTag("code", "<pre>", "</pre>") { StopProcessing = true } });

            var input = "[code][i]This should [u]be a[/u] text literal[/i][/code]";
            var expected = "<pre>[i]This should [u]be a[/u] text literal[/i]</pre>";

            Assert.AreEqual(expected, parser.ToHtml(input));
        }

        [TestMethod]
        public void GreedyAttributeProcessing_ConsumesAllTokensForAttributeValue()
        {
            var parser = new BBCodeParser(ErrorMode.ErrorFree, null, new[] { new BBTag("quote", "<div><span>Posted by ${name}</span>", "</div>", new BBAttribute("name", "")) { GreedyAttributeProcessing = true } });

            var input = "[quote=Test User With Spaces]Here is my comment[/quote]";
            var expected = "<div><span>Posted by Test User With Spaces</span>Here is my comment</div>";

            Assert.AreEqual(expected, parser.ToHtml(input));
        }

        [TestMethod]
        public void NewlineTrailingOpeningTagIsIgnored()
        {
            var parser = new BBCodeParser(ErrorMode.ErrorFree, null, new[] { new BBTag("code", "<pre>", "</pre>") });

            var input = "[code]\nHere is some code[/code]";
            var expected = "<pre>Here is some code</pre>"; // No newline after the opening PRE

            Assert.AreEqual(expected, parser.ToHtml(input));
        }

        [TestMethod]
        public void SuppressFirstNewlineAfter_StopsFirstNewlineAfterClosingTag()
        {
            var parser = new BBCodeParser(ErrorMode.ErrorFree, null, new[] { new BBTag("code", "<pre>", "</pre>"){ SuppressFirstNewlineAfter = true } });

            var input = "[code]Here is some code[/code]\nMore text!";
            var expected = "<pre>Here is some code</pre>More text!"; // No newline after the closing PRE

            Assert.AreEqual(expected, parser.ToHtml(input));
        }
    }
}
