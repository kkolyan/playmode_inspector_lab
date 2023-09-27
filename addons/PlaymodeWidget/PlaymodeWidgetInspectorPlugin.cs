#if TOOLS
using System.Linq;
using Godot;

namespace playmode_inspector_lab.addons.PlaymodeWidget;

[Tool]
public partial class PlaymodeWidgetInspectorPlugin : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject obj)
    {
        return obj.Get(PlaymodeWidgetHelper.IsPlaymodeInspectorWidget).AsBool();
    }

    public override bool _ParseProperty(GodotObject obj, Variant.Type type, string name, PropertyHint hintType,
        string hintString,
        PropertyUsageFlags usageFlags, bool wide)
    {
        switch (name)
        {
            case PlaymodeWidgetHelper.IsPlaymodeInspectorWidget:
            {
                if (obj.GetClass() == "EditorDebuggerRemoteObject")
                {
                    AddPropertyEditor(name, new PlaymodeWidgetInspector(obj));
                }

                return true;
            }
            case PlaymodeWidgetHelper.GetWidgetContent: return true;
            case PlaymodeWidgetHelper.PushEvent: return true;
            default:
                return obj.GetClass() == "EditorDebuggerRemoteObject" &&
                       obj.Get(PlaymodeWidgetHelper.VirtualProperties).AsStringArray().Contains(name);
        }
    }
}
#endif