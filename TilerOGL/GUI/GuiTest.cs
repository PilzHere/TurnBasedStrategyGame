using ImGuiHandler;
using ImGuiNET;

namespace TilerOGL.GUI
{
    public class GuiTest : ImGuiElement
    {
        protected override void CustomRender()
        {
            ImGui.Begin("THIS IS A TEST");
            
            ImGui.End();
        }
    }
}