using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Atlas.Colors
{
	public class DarkTabControl : TabControl
	{
		public DarkTabControl()
		{
			DrawMode = TabDrawMode.OwnerDrawFixed;
			SizeMode = TabSizeMode.Fixed;
			ItemSize = new Size(120, 28);
			Padding = new Point(0, 0);
			SetStyle(ControlStyles.UserPaint, true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			for (int i = 0; i < TabCount; i++)
			{
				Rectangle tabRect = GetTabRect(i);
				bool selected = SelectedIndex == i;

				Color back = selected ? Color.FromArgb(45, 45, 48) : Color.FromArgb(62, 62, 64);
				Color text = Color.White;

				//using (var b = new LinearGradientBrush(
				//	tabRect,
				//	selected ? Color.FromArgb(50, 50, 52) : Color.FromArgb(65, 65, 67),
				//	selected ? Color.FromArgb(35, 35, 37) : Color.FromArgb(55, 55, 57),
				//	LinearGradientMode.Vertical))
				using (var b = new SolidBrush(Color.FromArgb(45, 45, 48)))
				{
					e.Graphics.FillRectangle(b, new Rectangle(tabRect.Left + 1, tabRect.Top + 1, tabRect.Width - 2, tabRect.Height - 1));
				}

				if (selected)
				{
					// Bottom shadow to press it into the surface
					using (var b = new SolidBrush(Color.FromArgb(30, 30, 30)))
					{
						// Top Line
						e.Graphics.DrawLine(new Pen(b, 2), tabRect.Left + 4, tabRect.Top + 4, tabRect.Right - 4, tabRect.Top + 4);

						// Left Line
						e.Graphics.DrawLine(new Pen(b, 2), tabRect.Left + 4, tabRect.Top + 4, tabRect.Left + 4, tabRect.Bottom - 4);
					}

					using (var b = new SolidBrush(Color.FromArgb(60, 60, 65)))
					{
						// Bottom Line
						e.Graphics.DrawLine(new Pen(b, 2), tabRect.Left + 4, tabRect.Bottom - 4, tabRect.Right - 4, tabRect.Bottom - 4);

						// Right Line
						e.Graphics.DrawLine(new Pen(b, 2), tabRect.Right - 4, tabRect.Top + 4, tabRect.Right - 4, tabRect.Bottom - 4);
					}
				}
				else
				{
					using (var b = new SolidBrush(Color.FromArgb(60, 60, 65)))
					{
						// Bottom Line
						e.Graphics.DrawLine(new Pen(b, 2), tabRect.Left + 2, tabRect.Bottom, tabRect.Right - 2, tabRect.Bottom);
					}
				}

					TextRenderer.DrawText(
						e.Graphics,
						TabPages[i].Text,
						Font,
						tabRect,
						text,
						TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
					);
			}

			// Skip drawing the default content border
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			using (var b = new SolidBrush(Color.FromArgb(30, 30, 30)))
				e.Graphics.FillRectangle(b, ClientRectangle);
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle r = base.DisplayRectangle;
				r.Inflate(2, 2); // expand to cover the default border
				return r;
			}
		}
	}

}
