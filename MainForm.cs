using System.ComponentModel;

namespace CheatEngineExternalGui;

public sealed class MainForm : Form
{
    private readonly FlowLayoutPanel _togglePanel = new()
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        Padding = new Padding(10)
    };

    private readonly Label _statusLabel = new()
    {
        Dock = DockStyle.Bottom,
        Height = 28,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(10, 0, 0, 0),
        Text = "Load the target game and use toggles below."
    };

    private readonly MemoryPatcher _patcher = new();
    private AppConfig _config = new();

    public MainForm()
    {
        Text = "External Cheat Toggle GUI";
        Width = 560;
        Height = 420;
        Controls.Add(_togglePanel);
        Controls.Add(_statusLabel);
        Load += OnLoad;
    }

    private void OnLoad(object? sender, EventArgs e)
    {
        try
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
            _config = ConfigLoader.Load(configPath);
            BuildToggleControls();
            SetStatus($"Loaded {_config.Toggles.Count} toggles for process '{_config.ProcessName}'.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Failed to load config. See error dialog.");
        }
    }

    private void BuildToggleControls()
    {
        _togglePanel.Controls.Clear();

        foreach (var toggle in _config.Toggles)
        {
            var row = new Panel
            {
                Width = _togglePanel.ClientSize.Width - 30,
                Height = 58,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(10)
            };

            var nameLabel = new Label
            {
                Text = toggle.Name,
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Top = 8,
                Left = 10
            };

            var descriptionLabel = new Label
            {
                Text = toggle.Description,
                AutoSize = true,
                Top = 30,
                Left = 10
            };

            var checkbox = new CheckBox
            {
                Text = "Enabled",
                AutoSize = true,
                Top = 18,
                Left = row.Width - 100,
                Tag = toggle
            };

            checkbox.CheckedChanged += OnToggleChanged;

            row.Controls.Add(nameLabel);
            row.Controls.Add(descriptionLabel);
            row.Controls.Add(checkbox);
            _togglePanel.Controls.Add(row);
        }
    }

    private void OnToggleChanged(object? sender, EventArgs e)
    {
        if (sender is not CheckBox checkbox || checkbox.Tag is not ToggleDefinition toggle)
        {
            return;
        }

        try
        {
            _patcher.ApplyPatch(_config.ProcessName, toggle, checkbox.Checked);
            SetStatus($"{toggle.Name}: {(checkbox.Checked ? "ON" : "OFF")}");
        }
        catch (Win32Exception ex)
        {
            checkbox.CheckedChanged -= OnToggleChanged;
            checkbox.Checked = !checkbox.Checked;
            checkbox.CheckedChanged += OnToggleChanged;
            SetStatus($"Failed ({toggle.Name}): {ex.Message}");
        }
        catch (Exception ex)
        {
            checkbox.CheckedChanged -= OnToggleChanged;
            checkbox.Checked = !checkbox.Checked;
            checkbox.CheckedChanged += OnToggleChanged;
            SetStatus($"Error ({toggle.Name}): {ex.Message}");
        }
    }

    private void SetStatus(string message) => _statusLabel.Text = message;
}
