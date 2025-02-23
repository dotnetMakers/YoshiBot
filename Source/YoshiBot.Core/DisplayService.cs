using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;

namespace YoshiBot.Core;

public class DisplayService
{
    private AbsoluteLayout _rootLayout;
    private IFont _font;
    private Label _label;

    private DisplayScreen Screen { get; }

    public DisplayService(IPixelDisplay display)
    {
        Screen = new DisplayScreen(display, RotationType.Normal);

        BuildContent();
    }

    private void BuildContent()
    {
        _font = new Font16x24();

        _rootLayout = new AbsoluteLayout(Screen.Width, Screen.Height);

        _label = new Label(
            left: 0,
            top: 0,
            width: Screen.Width,
            height: 30
            )
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = _font,
            TextColor = Color.White
        };

        _rootLayout.Controls.Add(_label);

        Screen.Controls.Add(_rootLayout);
    }

    public void UpdateTick(ulong tick)
    {
        _label.Text = tick.ToString();
    }
}