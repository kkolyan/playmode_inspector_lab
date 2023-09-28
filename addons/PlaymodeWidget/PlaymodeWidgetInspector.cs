#if TOOLS
using Godot;
using Godot.Collections;

namespace playmode_inspector_lab.addons.PlaymodeWidget;

[Tool]
public partial class PlaymodeWidgetInspector : EditorProperty
{
    private readonly GodotObject _remoteWidget;
    private readonly bool _playMode;
    private Node _widgetContent;

    public PlaymodeWidgetInspector(GodotObject remoteWidget, bool playMode)
    {
        _remoteWidget = remoteWidget;
        _playMode = playMode;
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
        EnhanceButtons(_widgetContent, new NodePath("0"));
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
                request["type"] = PlaymodeWidgetHelper.EventTypeButtonPressed;
                _remoteWidget.Set(PlaymodeWidgetHelper.PushEvent, request);
            };
        }

        if (control is TextEdit text)
        {
            text.TextChanged += () =>
            {
                var request = new Dictionary();
                request["id"] = Time.GetTicksMsec();
                request["path"] = nodePath;
                request["type"] = PlaymodeWidgetHelper.EventTypeTextChanged;
                request["text"] = text.Text;
                _remoteWidget.Set(PlaymodeWidgetHelper.PushEvent, request);
            };
        }

        var nodes = control.GetChildren();
        for (var i = 0; i < nodes.Count; i++)
        {
            var child = nodes[i];
            EnhanceButtons(child, nodePath + "/" + i);
        }
    }
}
#endif