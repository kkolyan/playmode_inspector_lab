#if TOOLS
using Godot;
using Godot.Collections;

namespace playmode_inspector_lab.addons.PlaymodeWidget;

[Tool]
public partial class PlaymodeWidgetInspector : EditorProperty
{
    private readonly GodotObject _remoteWidget;
    private Node _widgetContent;

    public PlaymodeWidgetInspector(GodotObject remoteWidget)
    {
        _remoteWidget = remoteWidget;
    }

    public override void _UpdateProperty()
    {
        if (_widgetContent != null)
        {
            RemoveChild(_widgetContent);
        }

        var s = _remoteWidget.Get(PlaymodeWidgetHelper.GetWidgetContent).AsString();
        var uiVariant = GD.StrToVar(s);
        _widgetContent = PlaymodeWidgetHelper.ToNode(uiVariant);
        EnhanceButtons(_widgetContent, new NodePath(_widgetContent.Name));
        AddChild(_widgetContent);
    }

    private void EnhanceButtons(Node control, string nodePath)
    {
        if (control is Button button)
        {
            button.Pressed += () =>
            {
                var request = new Dictionary();
                request["id"] = Time.GetTicksMsec();
                request["path"] = nodePath;
                _remoteWidget.Set(PlaymodeWidgetHelper.PushButtonPress, request);
            };
        }

        foreach (var child in control.GetChildren())
        {
            EnhanceButtons(child, nodePath + "/" + child.Name);
        }
    }
}
#endif