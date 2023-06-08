using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityCodeGen;
using UnityEngine;

[Generator]
public class InterfaceEditorGenerator : ICodeGenerator
{
    public void Execute(GeneratorContext context)
    {
        IEnumerable<Type> interface_types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(
                a => a.GetTypes().Where(t => t.IsDefined(typeof(InterfaceEditorAttribute)))
            );

        foreach (Type interface_type in interface_types)
        {
            GenerateSerializedInterface(context, interface_type);
            GenerateSerializedInterfaceEditor(context, interface_type);
        }
    }

    private void GenerateSerializedInterface(GeneratorContext context, Type interface_type)
    {
        string interface_name = interface_type.Name;

        string[] implemented_for_types = ClassNamesImplementingInterface(interface_type);

        string fields = implemented_for_types
            .Select(ty => $"    [SerializeField]\r\n    private {ty} {PascalToCamelCase(ty)};")
            .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : "\r\n\r\n") + y);

        string switch_variants = implemented_for_types
            .Select(ty => $"            \"{ty}\" => {PascalToCamelCase(ty)},")
            .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : "\r\n") + y);

        string template_path =
            Application.dataPath + "/Scripts/Editor/SerializedInterfaceTemplate.txt";
        string code = File.ReadAllText(template_path);
        code = code.Replace("|=INTERFACE_NAME=|", interface_name);
        code = code.Replace("|=INTERFACE_CLASS_NAME=|", interface_name[1..]);
        code = code.Replace("|=FIELDS=|", fields);
        code = code.Replace("|=SWITCH_VARIANTS=|", switch_variants);

        context.OverrideFolderPath("Assets/Scripts/Generated");
        context.AddCode($"Serialized{interface_name[1..]}.cs", code);
    }

    private void GenerateSerializedInterfaceEditor(GeneratorContext context, Type interface_type)
    {
        string interface_name = interface_type.Name;

        string[] implemented_for_types = ClassNamesImplementingInterface(interface_type);

        string implementing_type_names = implemented_for_types
            .Select(t => '"' + t + '"')
            .Aggregate("", (x, y) => x + (x.Length == 0 ? "" : ", ") + y);

        string template_path =
            Application.dataPath + "/Scripts/Editor/SerializedInterfaceEditorTemplate.txt";
        string code = File.ReadAllText(template_path);
        code = code.Replace("|=INTERFACE_CLASS_NAME=|", interface_name[1..]);
        code = code.Replace("|=IMPLEMENTING_TYPE_NAMES=|", implementing_type_names);

        context.OverrideFolderPath("Assets/Scripts/Generated");
        context.AddCode($"Serialized{interface_name[1..]}Editor.cs", code);
    }

    private string[] ClassNamesImplementingInterface(Type interface_type)
    {
        return Assembly
            .GetAssembly(interface_type)
            .GetTypes()
            .Where(type => interface_type.IsAssignableFrom(type) && !type.IsInterface)
            .Select(t => t.Name)
            .ToArray();
    }

    private string PascalToCamelCase(string pascal)
    {
        return pascal[..1].ToLower() + pascal[1..];
    }
}
