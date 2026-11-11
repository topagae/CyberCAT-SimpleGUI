using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CP2077SaveEditor.ModSupport;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Views.Controls;

public partial class ExtrasControl : UserControl, IGameControl
{
    private readonly Form2 _parentForm;

    public ExtrasControl(Form2 parentForm)
    {
        InitializeComponent();

        _parentForm = parentForm;
        _parentForm.PropertyChanged += OnParentFormPropertyChanged;
    }

    public string GameControlName => "Extras";

    private void OnParentFormPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_parentForm.ActiveSaveFile))
        {
            if (_parentForm.ActiveSaveFile != null)
            {
                this.InvokeIfRequired(Init);
            }
        }
    }

    private void Init()
    {
        gb_Player.Enabled = true;
        gb_WardrobeExtra.Enabled = _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>() != null;
    }

    private void btn_UnlockAll_Click(object sender, EventArgs e)
    {
        var fts = _parentForm.ActiveSaveFile.GetScriptableSystem<FastTravelSystem>();
        if (fts != null)
        {
            // Only add missing points. Clearing the list first dropped unlocked points that FastTravel.json
            // doesn't know about, like Dogtown ones or points added by mods.
            var known = new HashSet<ulong>();
            foreach (var node in fts.FastTravelNodes)
            {
                if (node.Chunk is { } point)
                {
                    known.Add(point.PointRecord);
                }
            }

            foreach (var record in ResourceHelper.FastTravelRecords)
            {
                if (!known.Add((TweakDBID)record.PointRecord))
                {
                    continue;
                }

                fts.FastTravelNodes.Add(new gameFastTravelPointData
                {
                    MarkerRef = record.MarkerRef,
                    PointRecord = record.PointRecord
                });
            }
        }
    }

    private void btn_MakeVulnerable_Click(object sender, EventArgs e)
    {
        var godModeSystem = _parentForm.ActiveSaveFile.GetGodModeSystem();
        if (godModeSystem != null)
        {
            var data = (gameGodModeSaveData)godModeSystem.RootChunk;
            foreach (var entityData in data.Gods)
            {
                if (entityData.EntityId.Hash != 1)
                {
                    continue;
                }

                for (var i = entityData.Data.Overrides.Count - 1; i >= 0; i--)
                {
                    if (entityData.Data.Overrides[i].Type == Enums.gameGodModeType.Invulnerable)
                    {
                        entityData.Data.Overrides.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void btn_ClearBlacklist_Click(object sender, EventArgs e)
    {
        _parentForm.ActiveSaveFile.GetScriptableSystem<WardrobeSystemExtra>().Blacklist = null;
    }
}
