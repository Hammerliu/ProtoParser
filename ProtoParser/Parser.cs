using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ProtoParser
{
    public class ProtoParser
    {
        public static IElement Parse(string protoText)
        {
            AntlrInputStream inputStream = new AntlrInputStream(protoText);
            var protobufLexer = new ProtobufLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(protobufLexer);
            var protobufParser = new ProtobufParser(commonTokenStream);
            var visitor = new ASTConstructor();
            return visitor.Visit(protobufParser.file());
        }
    }

    class ASTConstructor : ProtobufParserBaseVisitor<IElement>
    {
        public override IElement VisitFile([NotNull] ProtobufParser.FileContext context)
        {
            var syntax = context.syntaxDecl() == null ? string.Empty : GetText(context.syntaxDecl().syntaxLevel());
            var file = new File(syntax);
            file.Elements.AddRange(context.commentDecl().Select(Visit));
            file.Elements.AddRange(context.fileElement().Select(Visit));
            return file;
        }

        public override IElement VisitCommentDecl([NotNull] ProtobufParser.CommentDeclContext context)
        {
            return new Comment(GetText(context));
        }

        public override IElement VisitPackageDecl([NotNull] ProtobufParser.PackageDeclContext context)
        {
            var name = GetText(context.packageName());
            return new Package(name);
        }

        public override IElement VisitImportDecl([NotNull] ProtobufParser.ImportDeclContext context)
        {
            var name = GetText(context.importedFileName());
            var weak = context.WEAK() != null;
            var public_ = context.PUBLIC() != null;
            return new Import(name, weak, public_);
        }

        public override IElement VisitOptionDecl([NotNull] ProtobufParser.OptionDeclContext context)
        {
            var name = GetText(context.optionName());
            var value = GetText(context.optionValue());
            return new Option(name, value);
            
        }

        public override IElement VisitMessageLiteralField([NotNull] ProtobufParser.MessageLiteralFieldContext context)
        {
            var name = GetText(context.messageLiteralFieldName());
            var value = GetText(context.value());
            return new MessageLiteralField(name, value);
        }

        public override IElement VisitMessageLiteralWithBraces([NotNull] ProtobufParser.MessageLiteralWithBracesContext context)
        {
            return Visit(context.messageTextFormat());
        }

        public override IElement VisitMessageTextFormat([NotNull] ProtobufParser.MessageTextFormatContext context)
        {
            var fields = context.messageLiteralField() == null ? Array.Empty<IElement>() : context.messageLiteralField().Select(Visit).ToArray();
            return new ElementArray(fields);
        }

        public override IElement VisitMessageDecl([NotNull] ProtobufParser.MessageDeclContext context)
        {
            var name = GetText(context.messageName());
            var element = context.messageElement().Select(Visit).ToArray();
            return new Message(name, element);
        }

        public override IElement VisitMessageFieldDecl([NotNull] ProtobufParser.MessageFieldDeclContext context)
        {
            var fieldWithCardinality = context.fieldDeclWithCardinality();
            if (fieldWithCardinality != null)
                return Visit(fieldWithCardinality);
            else
            {
                var name = GetText(context.fieldName());
                var number = Convert.ToInt32(GetText(context.fieldNumber()));
                var type = GetText(context.messageFieldDeclTypeName());
                var options = context.compactOptions() == null ? new ElementArray(Array.Empty<IElement>()) : (ElementArray)Visit(context.compactOptions()) ;
                return new Field(name, number, type, options);
            }

        }

        public override IElement VisitFieldDeclWithCardinality([NotNull] ProtobufParser.FieldDeclWithCardinalityContext context)
        {
            var name = GetText(context.fieldName());
            var number = Convert.ToInt32(GetText(context.fieldNumber()));
            var cardinality = FieldCardinality.NONE;
            if (context.fieldCardinality().OPTIONAL() != null)
                cardinality = FieldCardinality.OPTIONAL;
            else if (context.fieldCardinality().REQUIRED() != null)
                cardinality = FieldCardinality.REQUIRED;
            else if (context.fieldCardinality().REPEATED() != null)
                cardinality = FieldCardinality.REPEATED;
            var type = GetText(context.fieldDeclTypeName());
            var options = context.compactOptions() == null ? new ElementArray(Array.Empty<IElement>()) : (ElementArray)Visit(context.compactOptions());
            return new Field(name, number, type, cardinality, options);
        }

        public override IElement VisitCompactOption([NotNull] ProtobufParser.CompactOptionContext context)
        {
            var name = GetText(context.optionName());
            var value = context.optionValue();

            
            if (value.messageLiteralWithBraces() != null)
            {
                var messageValue = Visit(value.messageLiteralWithBraces());
                return new MessageLiteralWithBraces(name, (ElementArray)messageValue);
            }
            else
            {
                var scalarValue = GetText(context.optionValue());
                return new Option(name, scalarValue);
            }
        }

        public override IElement VisitCompactOptions([NotNull] ProtobufParser.CompactOptionsContext context)
        {
            return new ElementArray(context.compactOption().Select(Visit).ToArray());
        }

        public override IElement VisitOneofFieldDecl([NotNull] ProtobufParser.OneofFieldDeclContext context)
        {
            var name = GetText(context.fieldName());
            var number = Convert.ToInt32(GetText(context.fieldNumber()));
            var type = GetText(context.oneofFieldDeclTypeName());
            var options = context.compactOptions() == null ? new ElementArray(Array.Empty<IElement>()) : (ElementArray)Visit(context.compactOptions());
            return new Field(name, number, type, options);
        }

        public override IElement VisitOneofGroupDecl([NotNull] ProtobufParser.OneofGroupDeclContext context)
        {
            var name = GetText(context.fieldName());
            var number = Convert.ToInt32(GetText(context.fieldNumber()));
            var elements = new ElementArray(context.messageElement().Select(Visit).ToArray());
            return new Group(name, number, elements);
        }

        public override IElement VisitMapFieldDecl(ProtobufParser.MapFieldDeclContext context)
        {
            var name = GetText(context.fieldName());
            var number = Convert.ToInt32(GetText(context.fieldNumber()));
            var keyType = GetText(context.mapType().mapKeyType());
            var valueType = GetText(context.mapType().typeName());
            var options = context.compactOptions() == null ? new ElementArray(Array.Empty<IElement>()): (ElementArray)Visit(context.compactOptions());
            return new MapField(name, number, keyType, valueType, options);
        }
        

        public override IElement VisitGroupDecl([NotNull] ProtobufParser.GroupDeclContext context)
        {
            var name = GetText(context.fieldName());
            var number = Convert.ToInt32(GetText(context.fieldNumber()));
            var fieldCardinality = context.fieldCardinality();
            var cardinality = FieldCardinality.NONE;
            if (fieldCardinality != null)
            {
                if (fieldCardinality.OPTIONAL() != null)
                    cardinality = FieldCardinality.OPTIONAL;
                else if (fieldCardinality.REQUIRED() != null)
                    cardinality = FieldCardinality.REQUIRED;
                else if (fieldCardinality.REPEATED() != null)
                    cardinality = FieldCardinality.REPEATED;
            }

            var elements = new ElementArray(context.messageElement().Select(Visit).ToArray());
            return new Group(name, number, cardinality, elements);
        }

        public override IElement VisitOneofDecl([NotNull] ProtobufParser.OneofDeclContext context)
        {
            var name = GetText(context.oneofName());
            var elements = new ElementArray(context.oneofElement().Select(Visit).ToArray());
            return new OneOf(name, elements);
        }

        public override IElement VisitExtensionRangeDecl([NotNull] ProtobufParser.ExtensionRangeDeclContext context)
        {
            var ranges = context.tagRanges().tagRange().Select(x => GetText(x)).ToArray();
            var options = context.compactOptions() == null ? new ElementArray(Array.Empty<IElement>()) : (ElementArray)Visit(context.compactOptions());
            return new ExtensionRange(ranges, options);
        }

        public override IElement VisitMessageReservedDecl([NotNull] ProtobufParser.MessageReservedDeclContext context)
        {
            var ranges = context.tagRanges() == null ? Array.Empty<string>() : context.tagRanges().tagRange().Select(x =>GetText(x)).ToArray();
            var names = context.names() == null ? Array.Empty<string>() : context.names().stringLiteral().Select(x => GetText(x)).ToArray();

            return new Reserved(ranges, names);
        }

        public override IElement VisitEnumDecl([NotNull] ProtobufParser.EnumDeclContext context)
        {
            var name = GetText(context.enumName());
            var elements = new ElementArray(context.enumElement().Select(Visit).ToArray());
            return new Enum(name, elements);
        }

        public override IElement VisitEnumValueDecl([NotNull] ProtobufParser.EnumValueDeclContext context)
        {
            var name = GetText(context.enumValueName());
            var number = Convert.ToInt32(GetText(context.enumValueNumber()));
            var options = context.compactOptions() == null ? new ElementArray(Array.Empty<IElement>()) : (ElementArray)Visit(context.compactOptions());

            return new EnumValue(name, number, options);
        }

        public override IElement VisitEnumReservedDecl([NotNull] ProtobufParser.EnumReservedDeclContext context)
        {
            var ranges = context.enumValueRanges() == null ? Array.Empty<string>() : context.enumValueRanges().enumValueRange().Select(x => GetText(x)).ToArray();
            var names = context.names() == null ? Array.Empty<string>() : context.names().stringLiteral().Select(x => GetText(x)).ToArray();

            return new EnumReserved(ranges, names);
        }

        public override IElement VisitExtensionDecl(ProtobufParser.ExtensionDeclContext context)
        {
            var typeName = GetText(context.extendedMessage());
            var elements = new ElementArray(context.extensionElement().Select(Visit).ToArray());
            return new Extension(typeName, elements);
        }
        

        public override IElement VisitServiceDecl(ProtobufParser.ServiceDeclContext context)
        {
            var name = GetText(context.serviceName());
            var elements = new ElementArray(context.serviceElement().Select(Visit).ToArray());
            return new Service(name, elements);
        }

        public override IElement VisitServiceElement(ProtobufParser.ServiceElementContext context)
        {
            var methodDecl = context.methodDecl();
            var optionDecl = context.optionDecl();
            var commentDecl = context.commentDecl();

            if (methodDecl != null)
                return Visit(methodDecl);
            else if (optionDecl != null)
                return Visit(optionDecl);
            else if (commentDecl != null)
                return Visit(commentDecl);
            else
                throw new ArgumentException("invalid service element");
        }
       

        public override IElement VisitMethodDecl(ProtobufParser.MethodDeclContext context)
        {
            var name = GetText(context.methodName());
            var input_type = (MessageType)Visit(context.inputType());
            var output_type = (MessageType)Visit(context.outputType());
            var elements = new ElementArray(context.methodElement().Select(Visit).ToArray());
            return new Method(name, input_type, output_type, elements);
        }

        public override IElement VisitInputType(ProtobufParser.InputTypeContext context)
        {
            return Visit(context.messageType());
        }
        

        public override IElement VisitOutputType(ProtobufParser.OutputTypeContext context)
        {
            return Visit(context.messageType());
        }
            
        public override IElement VisitMessageType(ProtobufParser.MessageTypeContext context)
        {
            var name = GetText(context.methodDeclTypeName());
            var stream = context.STREAM() != null;
            return new MessageType(name, stream);
        }

        public override IElement  VisitMethodElement(ProtobufParser.MethodElementContext context)
        {
            var optionDecl = context.optionDecl();
            var commentDecl = context.commentDecl();
            if (optionDecl != null)
                return Visit(optionDecl);
            else if (commentDecl != null)
                return Visit(commentDecl);
            else
                throw new ArgumentException("invalid method element");
        }
        

        private string GetText(ParserRuleContext context, bool trimQuotes = true)
        {
            var tokenSource = context.Start.TokenSource;
            var inputStream = tokenSource.InputStream;
            var text = inputStream.GetText(new Interval(context.Start.StartIndex, context.Stop.StopIndex));
            return trimQuotes ? text.Trim() : text;
        }
    }
}
   