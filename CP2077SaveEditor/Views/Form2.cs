using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using CP2077SaveEditor.Utils;
using CP2077SaveEditor.Views.Controls;
using WolvenKit.Common.Services;
using WolvenKit.Core.Compression;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Save.IO;

namespace CP2077SaveEditor.Views;

public partial class Form2 : Form, INotifyPropertyChanged
{
    private static readonly string s_saveGamesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "CD Projekt Red", "Cyberpunk 2077");

    internal bool IsLoaded;

    private SaveFileHelper _activeSaveFile;

    private bool _isSaving;

    public Form2()
    {
        InitializeComponent();

        Text = $"Cyberpunk 2077 Save Editor v{Application.ProductVersion} for Patch 2.5 (unofficial fork of CyberCAT-SimpleGUI)";
        tssl_Info.Text = $"v{Application.ProductVersion} (Patch 2.5) - fork of CyberCAT-SimpleGUI by Deweh - click for credits";

        CompressionSettings.Get().UseOodle = false;

        RegisterControl(new PlayerStatsControl(this), true);
        RegisterControl(new AppearanceControl(this));
        RegisterControl(new InventoryControl(this));
        RegisterControl(new VehiclesControl(this));
        RegisterControl(new QuestFactsControl(this));
        RegisterControl(new ExtrasControl(this));

        LoadConfig();
    }

    private static void LoadConfig()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        if (!File.Exists(configPath))
        {
            return;
        }

        try
        {
            var config = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(configPath));
            if (config.TryGetValue("EnableWIPFeatures", out var enableWipFeatures) && enableWipFeatures == 1)
            {
                Global.IsDebug = true;
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Unable to read config.json", "Notice");
        }
    }

    private async void Form2_Load(object sender, EventArgs e)
    {
        SetStatus("Init...");

        // Both are slow one-time initializations that don't depend on each other. HashService fills static name pools.
        await Task.WhenAll(
            Task.Run(() => new HashService()),
            Task.Run(ModManager.LoadTypes));

        openSaveButton.Enabled = true;

        SetStatus("Idle");
    }

    internal SaveFileHelper ActiveSaveFile
    {
        get => _activeSaveFile;
        private set => SetField(ref _activeSaveFile, value);
    }

    #region Controls

    private void RegisterControl(UserControl userControl, bool isLandingPage = false)
    {
        if (userControl is not IGameControl gc)
        {
            throw new ArgumentException($"{userControl.GetType().Name} doesn't implement {nameof(IGameControl)}", nameof(userControl));
        }

        userControl.Dock = DockStyle.Fill;

        sm_Menu.AddButton(gc.GameControlName, (_, _) => { SwitchPanel(userControl); });
        if (isLandingPage)
        {
            SwitchPanel(userControl);
        }
    }

    private void SwitchPanel(UserControl userControl)
    {
        if (userControl == null)
        {
            return;
        }

        this.InvokeIfRequired(() =>
        {
            pnl_Content.Controls.Clear();
            pnl_Content.Controls.Add(userControl);
        });
    }

    #endregion Controls

    private void openSaveButton_Click(object sender, EventArgs e)
    {
        using var fileWindow = new OpenFileDialog
        {
            InitialDirectory = s_saveGamesDirectory,
            Filter = "Cyberpunk 2077 Save File|*.dat"
        };

        if (fileWindow.ShowDialog() == DialogResult.OK)
        {
            LoadSave(fileWindow.FileName);
        }
    }

    private static (EFileReadErrorCodes Status, CyberpunkSaveFile Save, uint GameVersion) ReadSave(string savePath)
    {
        // Read-only, so saves can also be opened while the game is running
        using var fs = File.OpenRead(savePath);
        using var reader = new CyberpunkSaveReader(fs);

        var status = reader.ReadFile(out var save);
        if (status != EFileReadErrorCodes.UnsupportedVersion)
        {
            return (status, save, 0);
        }

        fs.Position = 0;
        reader.ReadFileInfo(out var info);
        return (status, null, info?.GameVersion ?? 0);
    }

    private static byte[] ReadAllBytesIfExists(string path) => File.Exists(path) ? File.ReadAllBytes(path) : null;

    private async void LoadSave(string savePath)
    {
        IsLoaded = false;
        pnl_Content.Enabled = false;
        openSaveButton.Enabled = false;
        saveChangesButton.Enabled = false;

        SetStatus("Loading save...");

        try
        {
            var (status, save, gameVersion) = await Task.Run(() => ReadSave(savePath));
            switch (status)
            {
                case EFileReadErrorCodes.NoError:
                    var saveDirectory = Path.GetDirectoryName(savePath);
                    ActiveSaveFile = new SaveFileHelper
                    {
                        SaveFile = save,
                        Metadata = ReadAllBytesIfExists(Path.Combine(saveDirectory, "metadata.9.json")),
                        ImageData = ReadAllBytesIfExists(Path.Combine(saveDirectory, "screenshot.png"))
                    };

                    filePathLabel.Text = Path.GetFileName(saveDirectory);
                    saveChangesButton.Enabled = true;
                    pnl_Content.Enabled = true;
                    IsLoaded = true;

                    // Release the previously loaded save right away, they are large
                    GC.Collect();
                    break;
                case EFileReadErrorCodes.NoCSav:
                    MessageBox.Show("Failed to parse save file: File contains invalid data");
                    break;
                case EFileReadErrorCodes.UnsupportedVersion:
                    MessageBox.Show($"Failed to parse save file: Game version {gameVersion} is not supported!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
        catch (Exception e)
        {
            ShowError("Failed to parse save file", e);
        }

        SetStatus("Idle");
        openSaveButton.Enabled = true;
    }

    private static void ShowError(string message, Exception e)
    {
        try
        {
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "error.txt"), e.ToString());
            MessageBox.Show($"{message}:{Environment.NewLine}{e.Message}{Environment.NewLine}An error.txt file has been generated with additional information.");
        }
        catch (Exception)
        {
            MessageBox.Show($"{message}:{Environment.NewLine}{e}");
        }
    }

    internal void SetStatus(string message)
    {
        this.InvokeIfRequired(() => { tssl_Status.Text = message; });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private async void saveChangesButton_Click(object sender, EventArgs e)
    {
        using var saveWindow = new FolderBrowserDialog { InitialDirectory = s_saveGamesDirectory };
        if (saveWindow.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var fileDirectory = saveWindow.SelectedPath;

        if (string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(fileDirectory)), s_saveGamesDirectory, StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Saving into the root folder isn't allowed");
            return;
        }

        pnl_Content.Enabled = false;
        openSaveButton.Enabled = false;
        saveChangesButton.Enabled = false;

        SetStatus("Saving...");
        _isSaving = true;

        var saved = false;
        try
        {
            var fileName = Path.Combine(fileDirectory, "sav.dat");
            var backupName = Path.Combine(fileDirectory, "sav.old");
            if (File.Exists(fileName) && !File.Exists(backupName))
            {
                File.Copy(fileName, backupName);
            }

            var warnings = ActiveSaveFile.PrepareForSave();
            if (warnings.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, warnings), "Warning");
            }

            await Task.Run(() => WriteSave(ActiveSaveFile.SaveFile, fileName));

            await WriteIfMissing(Path.Combine(fileDirectory, "metadata.9.json"), ActiveSaveFile.Metadata);
            await WriteIfMissing(Path.Combine(fileDirectory, "screenshot.png"), ActiveSaveFile.ImageData);

            saved = true;
        }
        catch (Exception err)
        {
            ShowError("Failed to save changes", err);
        }
        finally
        {
            _isSaving = false;
        }

        pnl_Content.Enabled = true;
        openSaveButton.Enabled = true;
        saveChangesButton.Enabled = true;

        SetStatus(saved ? "File saved." : "Saving failed.");
    }

    private static void WriteSave(CyberpunkSaveFile save, string fileName)
    {
        // Write to a temporary file first, so a failed or interrupted save never leaves a broken sav.dat behind
        var tempName = fileName + ".tmp";
        try
        {
            using (var fs = File.Create(tempName))
            using (var writer = new CyberpunkSaveWriter(fs, Encoding.UTF8, true))
            {
                writer.WriteFile(save, true);
            }

            File.Move(tempName, fileName, true);
        }
        finally
        {
            File.Delete(tempName);
        }
    }

    private static async Task WriteIfMissing(string path, byte[] data)
    {
        if (File.Exists(path))
        {
            return;
        }

        if (data == null)
        {
            MessageBox.Show($"Error while saving file: {Path.GetFileName(path)} could not be found.");
            return;
        }

        await File.WriteAllBytesAsync(path, data);
    }

    private void tssl_Info_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            $"""
            CyberCAT-SimpleGUI v{Application.ProductVersion} - Patch 2.5 update

            Original editor: CyberCAT-SimpleGUI by Deweh (DeweySalt) and contributors
            https://github.com/Deweh/CyberCAT-SimpleGUI

            Based on CyberCAT by SirBitesalot and contributors
            Save parsing by WolvenKit (GPL-3.0) by the WolvenKit team

            Patch 2.5 update: written by Claude (Anthropic's AI model), directed and tested by topagae.
            This fork is unofficial and not endorsed by the original authors.

            Always back up your saves. See README.md and NOTICE.md for details.
            """,
            "Credits");
    }

    private void Form2_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isSaving && MessageBox.Show("Saving in progress. Closing now will cancel it and your changes won't be saved.\r\nAre you sure you want to exit?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
        {
            e.Cancel = true;
        }
    }
}
