using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

// Added in v0.29a (2026-10) for patch 2.5 support. Written by Claude (Anthropic) with topagae, see NOTICE.md.

namespace CP2077SaveEditor.Utils;

/// <summary>
/// Patch 2.5 replaced the per entity stats (keys / values) with deduplicated tables (valuesOptimized + shared*).
/// Entries are exposed as gameSavedStatsData so the editor can keep working with them. On save, only the entries
/// that were changed are written back, as new slices. Shared slices are never modified in place.
/// </summary>
public class OptimizedStatsHelper
{
    // Upper two bits of gameBufferSlice.combined are the buffer kind, the rest is the length
    private const int KindShift = 30;
    private const uint LengthMask = (1u << KindShift) - 1;
    private const uint ModifiersKind = 0;
    private const uint ForcedModifiersKind = 1;

    private readonly gameStatsStateMapStructure _statsMap;
    private readonly Dictionary<(ulong, gameStatIDType), gameSavedStatsDataOptimized> _entries = new();
    private readonly Dictionary<gameSavedStatsDataOptimized, TrackedView> _views = new();

    private class TrackedView
    {
        public gameSavedStatsData View { get; init; }

        // null for new entries, so they are always written
        public byte[] Fingerprint { get; set; }
    }

    public OptimizedStatsHelper(gameStatsStateMapStructure statsMap)
    {
        _statsMap = statsMap;

        foreach (var entry in statsMap.ValuesOptimized)
        {
            _entries[(entry.EntityHash, entry.IdType)] = entry;
        }
    }

    public static bool IsOptimized(gameStatsStateMapStructure statsMap) => statsMap.ValuesOptimized.Count > 0;

    public bool ContainsSeed(uint seed) => _statsMap.ValuesOptimized.Any(x => x.Seed == seed);

    public gameSavedStatsData Get(gameStatsObjectID id)
    {
        if (!_entries.TryGetValue((id.EntityHash, id.IdType), out var entry))
        {
            return null;
        }

        if (!_views.TryGetValue(entry, out var tracked))
        {
            var view = CreateView(entry);
            tracked = new TrackedView { View = view, Fingerprint = GetFingerprint(view) };
            _views.Add(entry, tracked);
        }

        return tracked.View;
    }

    public void Add(gameStatsObjectID id, gameSavedStatsData statsData)
    {
        var entry = new gameSavedStatsDataOptimized
        {
            EntityHash = id.EntityHash,
            IdType = id.IdType,
            Seed = statsData.Seed
        };

        statsData.ModifiersBuffer ??= new DataBuffer { Data = new ModifiersBuffer() };
        statsData.ForcedModifiersBuffer ??= new DataBuffer { Data = new ModifiersBuffer() };

        _statsMap.ValuesOptimized.Add(entry);
        _entries[(id.EntityHash, id.IdType)] = entry;
        _views[entry] = new TrackedView { View = statsData };
    }

    /// <summary>
    /// Writes all changed entries back into the optimized tables. Returns warnings about edits that couldn't be saved.
    /// </summary>
    public List<string> Commit()
    {
        var warnings = new List<string>();

        var data = _statsMap.SharedDataBuffer.Buffer.GetBytes();
        var dataEnd = _statsMap.SharedBufferSlices.Select(x => (int)((uint)x.Offset + ((uint)x.Combined & LengthMask))).DefaultIfEmpty(0).Max();
        var dataChanged = false;

        foreach (var (entry, tracked) in _views)
        {
            var view = tracked.View;

            var fingerprint = GetFingerprint(view);
            if (tracked.Fingerprint != null && fingerprint.AsSpan().SequenceEqual(tracked.Fingerprint))
            {
                continue;
            }

            if (view.SavedModifierGroupStatTypesBuffer?.Data is SavedModifierGroupStatTypesBuffer { Entries.Count: > 0 })
            {
                warnings.Add($"Modifier group changes for \"{view.RecordID.GetResolvedText() ?? view.RecordID.ToString()}\" can't be saved in this game version and were skipped.");
            }

            var sliceIndices = new CArray<CUInt16>();
            AddSlice(Encode(view.ModifiersBuffer), ModifiersKind);
            AddSlice(Encode(view.ForcedModifiersBuffer), ForcedModifiersKind);

            // Keep slices of unknown kinds as they are
            foreach (var index in entry.BufferSliceIndices)
            {
                if (GetKind(_statsMap.SharedBufferSlices[index]) is not (ModifiersKind or ForcedModifiersKind))
                {
                    sliceIndices.Add(index);
                }
            }

            entry.BufferSliceIndices = sliceIndices;
            entry.InactiveStatsIndices = new CArray<CUInt16>(view.InactiveStats.Select(x => (CUInt16)GetOrAdd(_statsMap.SharedInactiveStats, x)).ToList());
            entry.RecordIDIndex = (ushort)GetOrAdd(_statsMap.SharedRecordId, view.RecordID);
            entry.Seed = view.Seed;

            tracked.Fingerprint = fingerprint;

            void AddSlice(byte[] bytes, uint kind)
            {
                if (bytes.Length == 0)
                {
                    return;
                }

                // Reuse an identical slice if there is one
                for (var i = 0; i < _statsMap.SharedBufferSlices.Count; i++)
                {
                    var slice = _statsMap.SharedBufferSlices[i];
                    if (GetKind(slice) == kind && (slice.Combined & LengthMask) == bytes.Length &&
                        data.AsSpan((int)(uint)slice.Offset, bytes.Length).SequenceEqual(bytes))
                    {
                        sliceIndices.Add((ushort)i);
                        return;
                    }
                }

                if (dataEnd + bytes.Length > data.Length)
                {
                    var newSize = Math.Max(data.Length, 1);
                    while (newSize < dataEnd + bytes.Length)
                    {
                        newSize *= 2;
                    }
                    Array.Resize(ref data, newSize);
                }

                bytes.CopyTo(data, dataEnd);

                _statsMap.SharedBufferSlices.Add(new gameBufferSlice
                {
                    Offset = (uint)dataEnd,
                    Combined = (uint)bytes.Length | (kind << KindShift)
                });
                sliceIndices.Add((ushort)(_statsMap.SharedBufferSlices.Count - 1));

                dataEnd += bytes.Length;
                dataChanged = true;
            }
        }

        if (_statsMap.SharedBufferSlices.Count > ushort.MaxValue + 1 || _statsMap.SharedRecordId.Count > ushort.MaxValue + 1 || _statsMap.SharedInactiveStats.Count > ushort.MaxValue + 1)
        {
            throw new InvalidOperationException("Too many stats entries to save.");
        }

        if (dataChanged)
        {
            _statsMap.SharedDataBuffer.Buffer.SetBytes(data);
        }

        return warnings;
    }

    private gameSavedStatsData CreateView(gameSavedStatsDataOptimized entry)
    {
        var view = new gameSavedStatsData
        {
            Seed = entry.Seed,
            ModifiersBuffer = ReadSlices(entry, ModifiersKind),
            ForcedModifiersBuffer = ReadSlices(entry, ForcedModifiersKind)
        };

        if (entry.RecordIDIndex < _statsMap.SharedRecordId.Count)
        {
            view.RecordID = _statsMap.SharedRecordId[entry.RecordIDIndex];
        }

        foreach (var index in entry.InactiveStatsIndices)
        {
            view.InactiveStats.Add(_statsMap.SharedInactiveStats[index]);
        }

        return view;
    }

    private DataBuffer ReadSlices(gameSavedStatsDataOptimized entry, uint kind)
    {
        var data = _statsMap.SharedDataBuffer.Buffer.GetBytes();

        using var ms = new MemoryStream();
        foreach (var index in entry.BufferSliceIndices)
        {
            var slice = _statsMap.SharedBufferSlices[index];
            if (GetKind(slice) == kind)
            {
                ms.Write(data, (int)(uint)slice.Offset, (int)(slice.Combined & LengthMask));
            }
        }

        // ModifiersBufferReader keeps the raw bytes (IsParsed = false) if it can't parse them
        var buffer = new DataBuffer(ms.ToArray());
        using var reader = new ModifiersBufferReader(new MemoryStream(buffer.Buffer.GetBytes()));
        reader.ReadBuffer(buffer.Buffer);

        return buffer;
    }

    private static uint GetKind(gameBufferSlice slice) => (uint)slice.Combined >> KindShift;

    private static byte[] Encode(DataBuffer buffer)
    {
        if (buffer?.Data is ModifiersBuffer { IsParsed: true } modifiersBuffer)
        {
            var ms = new MemoryStream();
            using (var writer = new ModifiersBufferWriter(ms))
            {
                writer.WriteBuffer(modifiersBuffer);
            }
            return ms.ToArray();
        }

        return buffer?.Buffer.GetBytes() ?? Array.Empty<byte>();
    }

    private static byte[] GetFingerprint(gameSavedStatsData view)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        var modifiers = Encode(view.ModifiersBuffer);
        bw.Write(modifiers.Length);
        bw.Write(modifiers);

        var forcedModifiers = Encode(view.ForcedModifiersBuffer);
        bw.Write(forcedModifiers.Length);
        bw.Write(forcedModifiers);

        bw.Write(view.InactiveStats.Count);
        foreach (var inactiveStat in view.InactiveStats)
        {
            bw.Write(Convert.ToInt64((gamedataStatType)inactiveStat));
        }

        bw.Write((ulong)view.RecordID);
        bw.Write((uint)view.Seed);

        return ms.ToArray();
    }

    private static int GetOrAdd<T>(CArray<T> array, T value) where T : IRedType
    {
        var index = array.IndexOf(value);
        if (index == -1)
        {
            array.Add(value);
            index = array.Count - 1;
        }
        return index;
    }
}
