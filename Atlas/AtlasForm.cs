using System;
using System.Drawing;
using System.Windows.Forms;

using Atlas.Colors;
using Atlas.Editor;
using Atlas.World;

namespace Atlas
{
	public partial class AtlasForm : Form
	{
		private WorldDocument _document;
		private Header? _selectedHeader;

		// Controls
		private MenuStrip? _menuStrip;
		private TabControl? _tabControl;
		private TabPage? _headerTab;
		private TabPage? _matrixTab;
		private TabPage? _mapTab;

		public AtlasForm()
		{
			InitializeComponent();

			_document = new WorldDocument();
			Serializer.Load(_document);

			this.BackColor = Color.FromArgb(30, 30, 30);
			this.ForeColor = Color.White;

			BuildUI();
			PopulateMapControls();

			for (int i = 0; i < _document.Headers.Count; i++)
			{
				AddHeaderToList(_document.Headers[i]);
			}
		}

		private void BuildUI()
		{
			Text = "Atlas - World Editor";
			Size = new Size(1280, 720);

			BuildMenu();
			BuildTabs();
		}

		private void BuildMenu()
		{
			_menuStrip = new MenuStrip();

			var fileMenu = new ToolStripMenuItem("File");
			fileMenu.DropDownItems.Add("Save", null, OnSave);
			fileMenu.DropDownItems.Add(new ToolStripSeparator());
			fileMenu.DropDownItems.Add("Exit", null, (s, e) => Close());

			_menuStrip.Items.Add(fileMenu);

			MainMenuStrip = _menuStrip;
			Controls.Add(_menuStrip);

			_menuStrip.Renderer = new ToolStripProfessionalRenderer(new MenuStripColor());
			_menuStrip.BackColor = Color.FromArgb(45, 45, 48);

			ApplyDarkThemeToMenu(_menuStrip.Items);
		}

		private void ApplyDarkThemeToMenu(ToolStripItemCollection items)
		{
			foreach (ToolStripItem item in items)
			{
				item.ForeColor = Color.White;

				if (item is ToolStripMenuItem menuItem)
				{
					ApplyDarkThemeToMenu(menuItem.DropDownItems);
				}
			}
		}

		private void BuildTabs()
		{
			var root = new Panel
			{
				Dock = DockStyle.Fill,
				BorderStyle = BorderStyle.None,
				BackColor = Color.FromArgb(30, 30, 30),
			};

			_tabControl = new DarkTabControl
			{
				Dock = DockStyle.Fill,
			};

			_headerTab = new TabPage("Header Editor")
			{
				BackColor = Color.FromArgb(45, 45, 48)
			};

			_matrixTab = new TabPage("Matrix Editor")
			{
				BackColor = Color.FromArgb(45, 45, 48)
			};

			_mapTab = new TabPage("Map Editor")
			{
				BackColor = Color.FromArgb(45, 45, 48)
			};

			BuildHeaderTab();
			BuildMatrixTab();
			BuildMapTab();

			_tabControl.TabPages.Add(_headerTab);
			_tabControl.TabPages.Add(_matrixTab);
			_tabControl.TabPages.Add(_mapTab);

			root.Controls.Add(_tabControl);

			Controls.Add(root);

			root.BringToFront();
		}

		#region Header Tab
		private ListView? _headerList;
		private NumericUpDown _headerIdBox;
		private NumericUpDown _verticalOffsetBox;

		private void BuildHeaderTab()
		{
			var layout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 3,
				RowCount = 1
			};

			// 3 columns: Left (250px) | Center (flex) | Right (250px)
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));

			layout.Controls.Add(BuildHeaderListPanel(), 0, 0);
			layout.Controls.Add(BuildHeaderPropertiesPanel(), 1, 0);
			layout.Controls.Add(BuildHeaderPreviewPanel(), 2, 0);

			_headerTab.Controls.Add(layout);
		}

		private Control BuildHeaderListPanel()
		{
			var panel = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
				RowCount = 4
			};

			panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Search box
			panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // List
			panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // Add button
			panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));  // Remove button

			// Search box
			var searchGroup = new GroupBox
			{
				Text = "Search Header:",
				Dock = DockStyle.Fill,
				ForeColor = Color.White,
				Margin = new Padding(3, 0, 3, 3)
			};

			var searchLayout = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
			};

			var searchBox = new TextBox { Width = 150 };
			var goBtn = new Button
			{	Text = "Go",
				Width = 40,
				Margin = new Padding(4, 3, 4, 3),
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
			};

			goBtn.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
			goBtn.FlatAppearance.BorderSize = 1;

			var resetBtn = new Button
			{	Text = "Reset",
				Width = 60,
				Margin = new Padding(4, 3, 4, 3),
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
			};

			resetBtn.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
			resetBtn.FlatAppearance.BorderSize = 1;

			searchLayout.Controls.Add(searchBox);
			searchLayout.Controls.Add(goBtn);
			searchLayout.Controls.Add(resetBtn);
			searchGroup.Controls.Add(searchLayout);

			// Header list
			_headerList = new ListView
			{
				Dock = DockStyle.Fill,
				View = View.Details,
				FullRowSelect = true,
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.White,
				HeaderStyle = ColumnHeaderStyle.Nonclickable
			};

			_headerList.Columns.Add("ID", 50, HorizontalAlignment.Left);
			_headerList.Columns.Add("Layout", 234, HorizontalAlignment.Left);

			_headerList.SelectedIndexChanged += (_, __) =>
			{
				if (_headerList.SelectedItems.Count == 0)
				{
					SelectHeader(null);
					return;
				}

				var item = _headerList.SelectedItems[0];
				SelectHeader(item.Tag as Header);
			};

			// Buttons
			var addBtn = new Button
			{
				Text = "Add Header",
				Dock = DockStyle.Fill,
				Margin = new Padding(4),
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
			};

			addBtn.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
			addBtn.FlatAppearance.BorderSize = 1;

			addBtn.Click += (_, __) =>
			{
				var header = _document.CreateHeader();
				AddHeaderToList(header);
			};

			var removeBtn = new Button
			{
				Text = "Remove Header",
				Dock = DockStyle.Fill,
				Margin = new Padding(4),
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
			};

			removeBtn.FlatAppearance.BorderColor = Color.FromArgb(30, 30, 30);
			removeBtn.FlatAppearance.BorderSize = 1;

			panel.Controls.Add(searchGroup, 0, 0);
			panel.Controls.Add(_headerList, 0, 1);
			panel.Controls.Add(addBtn, 0, 2);
			panel.Controls.Add(removeBtn, 0, 3);

			return panel;
		}

		private void SelectHeader(Header? header)
		{
			_selectedHeader = header;

			if (_headerIdBox == null || _verticalOffsetBox == null)
				return;

			if (header == null)
			{
				_headerIdBox.Value = 0;
				_verticalOffsetBox.Value = 0;
				return;
			}

			_headerIdBox.Value = header.HeaderId;
			_verticalOffsetBox.Value = header.VerticalOffset;

			PopulateMapFromHeader(header);
		}

		private void AddHeaderToList(Header header)
		{
			if (_headerList == null)
				return;

			var item = new ListViewItem(header.HeaderId.ToString());
			item.SubItems.Add("Unassigned");
			item.Tag = header;

			_headerList.Items.Add(item);
			RefreshMatrixHeaderList();
		}

		private Control BuildHeaderPropertiesPanel()
		{
			var layout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				RowCount = 5,
				Padding = new Padding(0)
			};

			layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
			layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));

			var identityGroup = new GroupBox
			{
				Text = "Identity",
				Dock = DockStyle.Fill,
				ForeColor = Color.White,
			};

			var identityLayout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 2,
				Padding = new Padding(10)
			};

			identityLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			identityLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			// Header ID
			var idLabel = new Label { Text = "Header ID:", Dock = DockStyle.Fill };
			_headerIdBox = new NumericUpDown { Dock = DockStyle.Fill, Maximum = ushort.MaxValue, ReadOnly = true };
			_headerIdBox.Controls[0].Enabled = false;

			var heightLabel = new Label { Text = "Vertical Offset:", Dock = DockStyle.Fill };
			_verticalOffsetBox = new NumericUpDown
			{
				Dock = DockStyle.Fill,
				Minimum = short.MinValue,
				Maximum = short.MaxValue,
				Increment = 1
			};

			_verticalOffsetBox.ValueChanged += (_, __) =>
			{
				if (_selectedHeader == null)
					return;

				_selectedHeader.VerticalOffset = (short)_verticalOffsetBox.Value;
			};

			identityLayout.Controls.Add(idLabel, 0, 0);
			identityLayout.Controls.Add(_headerIdBox, 0, 1);
			identityLayout.Controls.Add(heightLabel, 1, 0);
			identityLayout.Controls.Add(_verticalOffsetBox, 1, 1);

			identityGroup.Controls.Add(identityLayout);

			layout.Controls.Add(identityGroup);

			var behaviorGroup = new GroupBox
			{
				Text = "Behavior",
				Dock = DockStyle.Fill,
				ForeColor = Color.White,
			};

			var behaviorLayout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 4,
				Padding = new Padding(10)
			};

			behaviorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			behaviorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			var scriptsLabel = new Label { Text = "Script ID:", Dock = DockStyle.Fill };
			var scriptCmb = new ComboBox { Dock = DockStyle.Fill };

			var eventsLabel = new Label { Text = "Events ID:", Dock = DockStyle.Fill };
			var eventsCmb = new ComboBox { Dock = DockStyle.Fill };

			var textLabel = new Label { Text = "Text ID:", Dock = DockStyle.Fill };
			var textCmb = new ComboBox { Dock = DockStyle.Fill };

			behaviorLayout.Controls.Add(scriptsLabel, 0, 0);
			behaviorLayout.Controls.Add(scriptCmb, 0, 1);
			behaviorLayout.Controls.Add(eventsLabel, 1, 0);
			behaviorLayout.Controls.Add(eventsCmb, 1, 1);
			behaviorLayout.Controls.Add(textLabel, 0, 2);
			behaviorLayout.Controls.Add(textCmb, 0, 3);

			behaviorGroup.Controls.Add(behaviorLayout);

			layout.Controls.Add(behaviorGroup);

			return layout;
		}

		private Control BuildHeaderPreviewPanel()
		{
			var panel = new Panel
			{
				Dock = DockStyle.Fill,
				BorderStyle = BorderStyle.FixedSingle
			};

			// TODO: Add preview/info panel content
			// Could show: identityLayout preview, tileset info, scripts count, etc.

			return panel;
		}
		#endregion

		#region Matrix Tab
		private ListBox? _matrixHeaderList;
		private ushort _selectedHeaderId = 0;
		private Panel? _matrixGrid;
		private const int CellSize = 32;

		private void BuildMatrixTab()
		{
			var layout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 1
			};

			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

			layout.Controls.Add(BuldMatrixPalettePanel(), 0, 0);
			layout.Controls.Add(BuldMatrixGridPanel(), 1, 0);

			_matrixTab!.Controls.Add(layout);
		}

		private Control BuldMatrixPalettePanel()
		{
			var panel = new Panel
			{
				Dock = DockStyle.Fill,
				BackColor = Color.FromArgb(30, 30, 30)
			};

			_matrixHeaderList = new ListBox
			{
				Dock = DockStyle.Fill,
				BackColor = Color.FromArgb(30, 30, 30),
				ForeColor = Color.White
			};

			_matrixHeaderList.SelectedIndexChanged += (_, __) =>
			{
				if (_matrixHeaderList.SelectedItem is Header h)
					_selectedHeaderId = h.HeaderId;
			};

			panel.Controls.Add(_matrixHeaderList);
			return panel;
		}

		private Control BuldMatrixGridPanel()
		{
			_matrixGrid = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				BackColor = Color.FromArgb(30, 30, 30)
			};

			RebuildMatrixGrid();
			return _matrixGrid;
		}

		private void RebuildMatrixGrid()
		{
			_matrixGrid!.Controls.Clear();

			for (int y = 0; y < _document.Matrix.Height; y++)
			{
				for (int x = 0; x < _document.Matrix.Width; x++)
				{
					int cx = x;
					int cy = y;

					var btn = new Button
					{
						Width = CellSize,
						Height = CellSize,
						Left = x * CellSize,
						Top = y * CellSize,
						FlatStyle = FlatStyle.Flat,
						BackColor = Color.FromArgb(30, 30, 30),
						ForeColor = Color.White,
						Text = _document.Matrix.Cells[x, y].ToString()
					};

					btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);

					btn.Click += (s, e) =>
					{
						_document.Matrix.Cells[cx, cy] = _selectedHeaderId;
						btn.Text = _selectedHeaderId.ToString();
					};

					_matrixGrid.Controls.Add(btn);
				}
			}
		}

		private void RefreshMatrixHeaderList()
		{
			_matrixHeaderList!.Items.Clear();

			foreach (var h in _document.Headers)
				_matrixHeaderList.Items.Add(h);
		}
		#endregion

		#region Map Tab
		private ComboBox _geometryIdBox;
		private ComboBox _collisionIdBox;
		private ComboBox _regionalIdBox;
		private ComboBox _localIdBox;
		private ComboBox _interiorIdBox;

		private void BuildMapTab()
		{
			var layout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 1
			};

			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

			layout.Controls.Add(BuildMapHeaders(), 0, 0);
			layout.Controls.Add(BuildMapPreview(), 1, 0);

			_mapTab!.Controls.Add(layout);
		}

		private Control BuildMapHeaders()
		{
			var layout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				RowCount = 2,
			};

			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));

			var geoColGroup = new GroupBox
			{
				Text = "Geometry and Collision",
				Dock = DockStyle.Fill,
				ForeColor = Color.White
			};

			var geoColLayout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 2,
				Padding = new Padding(10)
			};

			geoColLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
			geoColLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

			geoColLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
			geoColLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

			var geoLabel = new Label { Text = "Geometry:", Dock = DockStyle.Fill };
			_geometryIdBox = new ComboBox { Dock = DockStyle.Fill };

			var colLabel = new Label { Text = "Collision:", Dock = DockStyle.Fill };
			_collisionIdBox = new ComboBox { Dock = DockStyle.Fill };

			geoColLayout.Controls.Add(geoLabel, 0, 0);
			geoColLayout.Controls.Add(_geometryIdBox, 1, 0);
			geoColLayout.Controls.Add(colLabel, 0, 1);
			geoColLayout.Controls.Add(_collisionIdBox, 1, 1);

			geoColGroup.Controls.Add(geoColLayout);

			var tilesetsGroup = new GroupBox
			{
				Text = "Tilesets",
				Dock = DockStyle.Fill,
				ForeColor = Color.White
			};

			var tilesetsLayout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 3,
				Padding = new Padding(10)
			};

			tilesetsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
			tilesetsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

			var regionalLabel = new Label { Text = "Regional:",	Dock = DockStyle.Fill };
			_regionalIdBox = new ComboBox { Dock = DockStyle.Fill };

			var localLabel = new Label { Text = "Local:", Dock = DockStyle.Fill };
			_localIdBox = new ComboBox { Dock = DockStyle.Fill };

			var interiorLabel = new Label { Text = "Interior:", Dock = DockStyle.Fill };
			_interiorIdBox = new ComboBox { Dock = DockStyle.Fill };

			tilesetsLayout.Controls.Add(regionalLabel, 0, 0);
			tilesetsLayout.Controls.Add(_regionalIdBox, 1, 0);
			tilesetsLayout.Controls.Add(localLabel, 0, 1);
			tilesetsLayout.Controls.Add(_localIdBox, 1, 1);
			tilesetsLayout.Controls.Add(interiorLabel, 0, 2);
			tilesetsLayout.Controls.Add(_interiorIdBox, 1, 2);

			tilesetsGroup.Controls.Add(tilesetsLayout);

			layout.Controls.Add(geoColGroup, 0, 0);
			layout.Controls.Add(tilesetsGroup, 0, 1);

			return layout;
		}

		private Control BuildMapPreview()
		{
			var panel = new Panel();

			return panel;
		}

		private void PopulateMapControls()
		{
			PopulateCombo(_geometryIdBox, _document.GeometryRegistry);
			PopulateCombo(_collisionIdBox, _document.CollisionRegistry);
			PopulateCombo(_regionalIdBox, _document.RegionalRegistry);
			PopulateCombo(_localIdBox, _document.LocalRegistry);
			PopulateCombo(_interiorIdBox, _document.InteriorRegistry);

			_geometryIdBox.SelectedIndexChanged += (_, __) =>
			{
				if (_selectedHeader == null) return;
				_selectedHeader.GeometryId = (ushort) _geometryIdBox.SelectedIndex;
			};

			_collisionIdBox.SelectedIndexChanged += (_, __) =>
			{
				if (_selectedHeader == null) return;
				_selectedHeader.CollisionId = (ushort) _collisionIdBox.SelectedIndex;
			};

			_regionalIdBox.SelectedIndexChanged += (_, __) =>
			{
				if (_selectedHeader == null) return;
				_selectedHeader.RegionalTilesetId = (ushort) _regionalIdBox.SelectedIndex;
			};

			_localIdBox.SelectedIndexChanged += (_, __) =>
			{
				if (_selectedHeader == null) return;
				_selectedHeader.LocalTilesetId = (ushort) _localIdBox.SelectedIndex;
			};

			_interiorIdBox.SelectedIndexChanged += (_, __) =>
			{
				if (_selectedHeader == null) return;
				_selectedHeader.InteriorTilesetId = (ushort) _interiorIdBox.SelectedIndex;
			};
		}

		private void PopulateCombo(ComboBox box, DataRegistry registry)
		{
			box.Items.Clear();

			for (int i = 0; i < registry.Entries.Count; i++)
			{
				var entry = registry.Entries[i];

				box.Items.Add(entry.ToString());
			}
		}

		private void PopulateMapFromHeader(Header header)
		{
			SetComboIndex(_geometryIdBox, header.GeometryId);
			SetComboIndex(_collisionIdBox, header.CollisionId);
			SetComboIndex(_regionalIdBox, header.RegionalTilesetId);
			SetComboIndex(_localIdBox, header.LocalTilesetId);
			SetComboIndex(_interiorIdBox, header.InteriorTilesetId);
		}

		private void SetComboIndex(ComboBox box, int index)
		{
			if (index >= 0 && index < box.Items.Count)
				box.SelectedIndex = index;
		}
		#endregion

		private void OnSave(object? sender, EventArgs e)
		{
			Serializer.Save(_document);
		}
	}
}
