#if TOOLS
using Godot;

namespace playmode_inspector_lab.addons.PlaymodeWidget;

[Tool]
public partial class PlaymodeWidgetPlugin : EditorPlugin
{
    private EditorInspectorPlugin _plugin;

    public override void _EnterTree()
    {
        _plugin = new PlaymodeWidgetInspectorPlugin();
        AddInspectorPlugin(_plugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_plugin);
    }
}


#endif