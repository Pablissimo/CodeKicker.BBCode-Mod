CodeKicker.BBCode-Mod
=====================

Modifications to CodeKicker.BBCode at http://bbcode.codeplex.com/

## First things first
CodeKicker.BBCode is a project hosted on [CodePlex](http://bbcode.codeplex.com/) that offers BBCode parsing and rendering functionality. I've got no involvement in that project at all, but TFS projects on CodePlex are pretty horrific to do anything with so I'm publishing a couple of fixes I made for a recent project on GitHub.

## A disclaimer
These fixes were put in at short notice for a project, so there might be better ways to do them. However, it does at least show how flexible a project CodeKicker's BBCode parser is.

## What fixes?
There're a bunch of BBCode parsers out there for .NET but none of them seem to be perfect. The CodeKicker implementation's better than most, though still had a few issues I needed to overcome.


### [code] tags no longer parse their contents as BBCode
This stops odd behaviour if the thing in your [code] tags happens to contain valid BBCode. For example:

    [code]
    int myVal = arr[i]; // The [i] is plain-text, not an italics directive!
    [/code]

### Better whitespace handling
The first newline after a tag is now suppressed. Optionally, if you set 'SuppressFirstNewlineAfter' = true on a BBTag it'll consume the whitespace up to and including the first newline after the closing tag. This helps in two situations:

It stops a spurious newline appearing before the first item in a list (or first line of a code snippet) if you write your BBCode as the following, more readable form:

    [list]
    [*]A list item
    [/list]

instead of the following which you currently need to do to get the formatting right:

    [list][*]A list item[/list]

Setting SuppressFirstNewlineAfter to true means that the first newline after the BBTag is consumed, so the following will render properly:

    [list]
    [*]Item one
    [*]Item two
    [/list]
    This will appear immediately below the list, no newline above

### Support for spaces in tag attribute values
If you have a tag with an attribute whose value needs to have spaces, BBTag now has a 'GreedyAttributeProcessing' property that when set to true will consume all characters up to the opening tag's closing square bracket as the attribute value, instead of stopping at the first space character. For example, now:

    [quote=Test user name]

Will now parse correctly, instead of setting the attribute's value to 'Test' where really you wanted 'Test user name'.

## Licence
The original CodeKicker.BBCode implementation is licensed under the MIT Licence. My modifications to the original implementation are released under the same terms.

Copyright (c) 2009 http://codekicker.de

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

