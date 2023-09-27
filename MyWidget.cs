﻿using Godot;
using Godot.Collections;
using playmode_inspector_lab.addons.PlaymodeWidget;

namespace playmode_inspector_lab;

public partial class MyWidget : Node
{
    private PlaymodeWidgetHelper.State _state;

    public override Array<Dictionary> _GetPropertyList() => PlaymodeWidgetHelper.GetPropertyList(
        new[] { "X", "Y" },
        ref _state
    );

    public override Variant _Get(StringName property) =>
        PlaymodeWidgetHelper.Get(property, CreateWidgetContent, ref _state);

    public override bool _Set(StringName property, Variant value) =>
        PlaymodeWidgetHelper.Set(property, value, ref _state);

    private Control CreateWidgetContent(string virtualPropertyName)
    {
        var textEdit = new TextEdit { CustomMinimumSize = new Vector2(380, 40) };
        textEdit.Name = "Text1";
        textEdit.TextChanged += () => { GD.Print($"TextChanged ('{textEdit.Name}'): {textEdit.Text}"); };
        return CreateBox(true, "Box", new Control[]
        {
            CreateButton("A"),
            CreateButton("B"),
            textEdit,
        });
    }

    private static Button CreateButton(string name)
    {
        var button = new Button();
        button.Name = name;
        button.Size = new Vector2(40, 40);
        button.Text = name;
        button.Pressed += () => { GD.Print($"Backend Button Pressed: {name}"); };
        return button;
    }

    private static Container CreateBox(bool horizontal, string name, Control[] children)
    {
        Container box = horizontal ? new HBoxContainer() : new VBoxContainer();
        box.Name = name;
        foreach (var child in children)
        {
            box.AddChild(child);
        }

        return box;
    }
}