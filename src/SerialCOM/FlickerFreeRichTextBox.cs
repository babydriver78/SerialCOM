using System.Windows.Forms;

namespace SerialCOM;

/// <summary>RichTextBox with double buffering to cut repaint flicker.</summary>
public sealed class FlickerFreeRichTextBox : RichTextBox
{
    public FlickerFreeRichTextBox()
    {
        DoubleBuffered = true;
    }
}
