using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace playmode_inspector_lab.addons.PlaymodeWidget;

using Array = Godot.Collections.Array;

public static class PlaymodeWidgetHelper
{
    internal const string IsPlaymodeInspectorWidget = "IsPlaymodeInspectorWidget";
    internal const string PushEvent = "PushEvent";
    internal const string EventTypeTextChanged = "TextChanged";
    internal const string EventTypeButtonPressed = "ButtonPressed";
    internal const string GetWidgetContent = "GetWidgetContent";

    public struct State
    {
        internal string DataCache;
        internal Control PrevUi;
    }

    public static Array<Dictionary> GetPropertyList()
    {
        var properties = new Array<Dictionary>();
        AddProperty(properties, IsPlaymodeInspectorWidget, Variant.Type.Bool);
        AddProperty(properties, PushEvent, Variant.Type.Dictionary);
        AddProperty(properties, GetWidgetContent, Variant.Type.String);
        return properties;
    }

    public static Variant Get(string property, Func<Control> createWidgetContent, ref State state)
    {
        switch (property)
        {
            case IsPlaymodeInspectorWidget: return true;
            case GetWidgetContent:
            {
                var data = state.DataCache;
                if (data != null)
                {
                    return data;
                }

                var ui = createWidgetContent.Invoke();
                var tree = ToTree(ui);
                data = GD.VarToStr(tree);
                state.PrevUi = ui;

                return data;
            }
            default: return default;
        }
    }

    public static bool Set(string property, Variant value, ref State state)
    {
        if (property == PushEvent)
        {
            state.DataCache = null;
            var command = GD.StrToVar(value.AsString()).AsGodotDictionary();
            var path = command["path"].AsString();
            var node = new Node();
            node.AddChild(state.PrevUi);
            GD.Print($"handling command to press '{path}'");
            foreach (var segment in path.Split("/"))
            {
                node = node.GetNode(segment);
            }

            switch (command["type"].AsString())
            {
                case EventTypeButtonPressed:

                    node.EmitSignal("pressed");
                    break;
                case EventTypeTextChanged:

                    ((TextEdit)node).Text = command["text"].AsString();
                    node.EmitSignal("text_changed");
                    break;
                    
            }

            return true;
        }

        return false;
    }

    private static void AddProperty(ICollection<Dictionary> properties, string name, Variant.Type type)
    {
        properties.Add(new Dictionary()
        {
            { "name", name },
            { "type", (int)type },
            { "usage", (int)PropertyUsageFlags.Default }, // See above assignment.
            { "hint", 0 },
            { "hint_string", "" }
        });
    }

    internal static Node ToNode(Variant tree)
    {
        var node = tree.AsGodotDictionary()["node"].As<Node>();
        var children = tree.AsGodotDictionary()["children"].As<Array<Variant>>();
        var name = tree.AsGodotDictionary()["name"].AsString();
        node.Name = name;
        foreach (var child in children)
        {
            var childNode = ToNode(child);
            node.AddChild(childNode);
        }

        return node;
    }

    private static Variant ToTree(Node node)
    {
        var dict = new Dictionary();
        dict["node"] = node;
        dict["name"] = node.Name;
        dict["children"] = new Array(node.GetChildren().Select(ToTree));
        return dict;
    }
}