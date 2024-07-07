# Protobuf Schema Parser for C#

Protobuf Schema Parser for C# is an implementation of Protobuf Schema parser, ported from [proto-schema-parser](https://github.com/recap-build/proto-schema-parser).

# Installation

Copy the code into your project and install Antlr4.Runtime.Standard from [NuGet](https://www.nuget.org/packages/Antlr4.Runtime.Standard).

# Usage

Call the static method Parse under ProtoParser.

```csharp
using Parser = ProtoParser.ProtoParser;

var protoText = """
    syntax = "proto3";

    package com.book;

    message Book
    {
        int64 isbn = 1;
        string title = 2;
        string author = 3;
    }
    """;

var file = Parser.Parse(protoText);
```

The variable 'file' will contain various nodes in the AST. All nodes implement the 'IElement' interface, which is used to get the node type.

# License

This project is under the [MIT](LICENSE.txt) license.
