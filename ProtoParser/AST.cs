using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoParser
{
    public enum ElementType
    {
        Comment,
        Package,
        Import,
        Option,
        Message,
        Field,
        Group,
        File,
        ElementArray,
        OneOf,
        ExtensionRange,
        Reserved,
        Enum,
        EnumValue,
        EnumReserved,
        Extension,
        Service,
        Method,
        MessageType,
        MapField,
        MessageLiteralField,
        MessageLiteralWithBraces
    }

    public enum FieldCardinality
    {
        REQUIRED,
        OPTIONAL,
        REPEATED,
        NONE
    }


    public interface IElement
    {
        ElementType ElementType { get; }
    }

    public class Comment : IElement
    {
        public string Text { get; }
        public ElementType ElementType => ElementType.Comment;

        public Comment(string text)
        {
            Text = text;
        }
    }

    public class Package : IElement
    {
        public string Name { get; }
        public ElementType ElementType => ElementType.Package;
        public Package(string name)
        {
            Name = name;
        }
    }

    public class Import : IElement
    {
        public string Name { get; }
        public bool Weak { get; }
        public bool Public { get; }
        public ElementType ElementType => ElementType.Import;
        public Import(string name, bool weak, bool public_)
        {
            Name = name;
            Weak = weak;
            Public = public_;
        }
    }

    public class Option : IElement
    {
        public string Name { get; }
        public string Value { get; }
        public ElementType ElementType => ElementType.Option;
        public Option(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }


    public class Message : IElement
    {
        public string Name { get; }
        public IElement[] Elements { get; }
        public ElementType ElementType => ElementType.Message;
        public Message(string name, IElement[] elements)
        {
            Name = name;
            Elements = elements;
        }
    }

    public class Field : IElement
    {
        public string Name { get; }
        public int Number { get; }
        public string Type { get; }
        public FieldCardinality Cardinality { get; }
        public ElementArray Options { get; }
        public ElementType ElementType => ElementType.Field;
        public Field(string name, int number, string type, ElementArray elements)
        {
            Name = name;
            Number = number;
            Type = type;
            Options = elements;
            Cardinality = FieldCardinality.NONE;
        }

        public Field(string name, int number, string type, FieldCardinality cardinality,ElementArray elements)
        {
            Name = name;
            Number = number;
            Type = type;
            Options = elements;
            Cardinality = cardinality;
        }
    }

    public class MapField : IElement
    {
        public string Name { get; }
        public int Number { get; }
        public string KeyType { get; }
        public string ValueType { get; }
        public ElementArray Options { get; }

        public ElementType ElementType => ElementType.MapField;

        public MapField(string name, int number, string keyType, string valueType, ElementArray options)
        {
            Name = name;
            Number = number;
            KeyType = keyType;
            ValueType = valueType;
            Options = options;
        }
    }

    public class Group : IElement
    {
        public string Name { get; }
        public int Number { get; }
        public FieldCardinality Cardinality { get; }
        public ElementArray Elements { get; }
        public ElementType ElementType => ElementType.Group;
        public Group(string name, int number, FieldCardinality cardinality, ElementArray elements)
        {
            Name = name;
            Number = number;
            Cardinality = cardinality;
            Elements = elements;
        }

        public Group(string name, int number, ElementArray elements)
        {
            Name = name;
            Number = number;
            Elements = elements;
            Cardinality = FieldCardinality.NONE;
        }
    }

    public class File : IElement
    {
        public string Syntax { get; }
        public List<IElement> Elements { get; }
        public ElementType ElementType => ElementType.File;

        public File(string syntax)
        {
            Syntax = syntax;
            Elements = new List<IElement>();
        }

    }

    public class ElementArray : IElement
    {
        public IElement[] Elements { get; }

        public ElementType ElementType => ElementType.ElementArray;

        public ElementArray(IElement[] elements)
        {
            Elements = elements;
        }
    }

    public class OneOf : IElement
    {
        public string Name { get; }
        public ElementType ElementType => ElementType.OneOf;
        public ElementArray Elements { get; }

        public OneOf(string name, ElementArray elements)
        {
            Name = name;
            Elements = elements;
        }
    }

    public class ExtensionRange : IElement
    {
        public string[] Ranges { get; }
        public ElementArray Options { get; }
        public ElementType ElementType => ElementType.ExtensionRange;

        public ExtensionRange(string[] ranges, ElementArray options)
        {
            Ranges = ranges;
            Options = options;
        }
    }

    public class Reserved : IElement
    {
        public string[] Ranges { get; }
        public string[] Name { get; }

        public ElementType ElementType => ElementType.Reserved;

        public Reserved(string[] ranges, string[] names)
        {
            Ranges = ranges;
            Name = names;
        }
    }

    public class Enum : IElement
    {
        public string Name { get; }
        public ElementArray Elements { get; }
        public ElementType ElementType => ElementType.Enum;

        public Enum(string name, ElementArray elementType)
        {
            Name = name;
            Elements = elementType;
        }
    }

    public class EnumValue : IElement
    {
        public string Name { get; }
        public int Number { get; }
        public ElementArray Options { get; }
        public ElementType ElementType => ElementType.EnumValue;
        
        public EnumValue(string name, int number, ElementArray elements)
        {
            Name = name;
            Number = number;
            Options = elements;
        }
    }

    public class EnumReserved : IElement
    {
        public string[] Names { get; }
        public string[] Ranges { get; }

        public ElementType ElementType => ElementType.EnumReserved;

        public EnumReserved(string[] ranges, string[] names)
        {

            Ranges = ranges;
            Names = names;
        }
    }

    public class Extension : IElement
    {
        public string TypeName { get; }
        public ElementArray Elements { get; }

        public ElementType ElementType => ElementType.Extension;

        public Extension(string typeName, ElementArray elements)
        {
            TypeName = typeName;
            Elements = elements;
        }
    }

    public class Service : IElement
    {
        public string Name { get; }
        public ElementArray Elements { get; }

        public ElementType ElementType => ElementType.Service;
        public Service(string name, ElementArray elements)
        {
            Name = name;
            Elements = elements;
        }
    }

    public class Method : IElement
    {
        public string Name { get; }
        public MessageType InputType { get; }
        public MessageType OutputType { get; }
        public ElementArray Elements { get; }
        public ElementType ElementType => ElementType.Method;
        public Method(string name, MessageType inputType, MessageType outputType, ElementArray elements)
        {
            Name = name;
            InputType = inputType;
            OutputType = outputType;
            Elements = elements;
        }
    }

    public class MessageType : IElement
    {
        public string Type { get; }
        public bool Stream { get; }

        public ElementType ElementType => ElementType.MessageType;

        public MessageType(string type, bool stream = false)
        {
            Type = type;
            Stream = stream;
        }
    }

    public class MessageLiteralField : IElement
    {
        public string Name { get; }
        public string Value { get; }
        public ElementType ElementType => ElementType.MessageLiteralField;

        public MessageLiteralField(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class MessageLiteralWithBraces : IElement
    {
        public string Name { get; }
        public ElementArray Fields { get; }

        public ElementType ElementType => ElementType.MessageLiteralWithBraces;

        public MessageLiteralWithBraces(string name, ElementArray fields)
        {
            Name = name;
            Fields = fields;
        }
    }
}