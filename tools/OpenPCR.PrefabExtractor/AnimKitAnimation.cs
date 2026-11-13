using global::OpenPCR.PrefabExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pcr.Prefab;

namespace OpenPCR.PrefabExtractor;

/// <summary>One animation: its timelines in the shared protobuf schema, and the property identities of the skill-effect projection.</summary>
internal sealed record AnimKitAnimation(double Duration, List<object?> Properties, SpineAnimation Timelines, List<double> Events)
{
	public const uint FormatVersion = 1;
	public static AnimKitAnimation Read(AnimKitReader r)
	{
		double duration = 0; var properties = new List<object?>(); var timelines = new SpineAnimation { FormatVersion = FormatVersion }; var events = new List<double>();
		SpineTimeline Timeline(SpineTimelineKind kind, string name, int index, params object?[] rest)
		{
			properties.Add(new object?[] { name, index }.Concat(rest).ToArray());
			var timeline = new SpineTimeline { Kind = kind, Index = index }; timelines.Timelines.Add(timeline); return timeline;
		}
		// Frame times are float32 in the resource; the last frame's time bounds the duration.
		void Time(SpineTimeline timeline, int i, int n) { double t = r.Float(); if (i == n - 1 && t > duration) duration = t; timeline.Times.Add((float)t); }
		// Every frame but the last carries a curve: 0 linear, 1 stepped, 2 bezier with four parameters.
		void Curves(SpineTimeline timeline, List<byte> types, int i, int n)
		{
			if (i == n - 1) { timeline.CurveTypes = Google.Protobuf.ByteString.CopyFrom(types.ToArray()); return; }
			int type = r.Byte();
			if (type is not (0 or 1 or 2)) throw new InvalidDataException("Unknown Spine curve: " + type);
			types.Add((byte)type);
			if (type == 2) for (int k = 0; k < 4; k++) timeline.CurveParams.Add((float)r.Float());
		}
		void FloatFrames(SpineTimeline timeline, int n, int width)
		{
			var types = new List<byte>();
			for (int i = 0; i < n; i++) { Time(timeline, i, n); for (int k = 0; k < width; k++) timeline.Values.Add((float)r.Float()); Curves(timeline, types, i, n); }
		}
		void Name(SpineTimeline timeline, string? text) { timeline.Names.Add(text ?? ""); timeline.NamesAbsent.Add(text is null); }
		for (int a = 0, ac = r.Var(); a < ac; a++)
		{
			int slot = r.Var();
			for (int b = 0, bc = r.Var(); b < bc; b++)
			{
				int type = r.Byte(), n = r.Var();
				var timeline = type switch
				{
					0 => Timeline(SpineTimelineKind.Attachment, "Attachment", slot),
					1 => Timeline(SpineTimelineKind.Color, "Color", slot),
					2 => Timeline(SpineTimelineKind.TwoColor, "TwoColor", slot),
					_ => throw new InvalidDataException("Unknown slot timeline: " + type)
				};
				var types = new List<byte>();
				for (int i = 0; i < n; i++)
				{
					Time(timeline, i, n);
					if (type == 0) { Name(timeline, r.String()); continue; }
					timeline.Colors.Add(r.Int()); if (type == 2) timeline.Colors.Add(r.Int());
					Curves(timeline, types, i, n);
				}
			}
		}
		for (int a = 0, ac = r.Var(); a < ac; a++)
		{
			int bone = r.Var();
			for (int b = 0, bc = r.Var(); b < bc; b++)
			{
				int type = r.Byte(), n = r.Var();
				var timeline = type switch
				{
					0 => Timeline(SpineTimelineKind.Rotate, "Rotate", bone),
					1 => Timeline(SpineTimelineKind.Translate, "Translate", bone),
					2 => Timeline(SpineTimelineKind.Scale, "Scale", bone),
					3 => Timeline(SpineTimelineKind.Shear, "Shear", bone),
					_ => throw new InvalidDataException("Unknown bone timeline: " + type)
				};
				FloatFrames(timeline, n, type == 0 ? 1 : 2);
			}
		}
		for (int a = 0, ac = r.Var(); a < ac; a++)
		{
			int index = r.Var(), n = r.Var(); var timeline = Timeline(SpineTimelineKind.IkConstraint, "IkConstraint", index); var types = new List<byte>();
			for (int i = 0; i < n; i++) { Time(timeline, i, n); timeline.Values.Add((float)r.Float()); timeline.Integers.Add(r.SignedByte()); Curves(timeline, types, i, n); }
		}
		for (int a = 0, ac = r.Var(); a < ac; a++) { int index = r.Var(), n = r.Var(); FloatFrames(Timeline(SpineTimelineKind.TransformConstraint, "TransformConstraint", index), n, 4); }
		for (int a = 0, ac = r.Var(); a < ac; a++)
		{
			int index = r.Var();
			for (int b = 0, bc = r.Var(); b < bc; b++)
			{
				int type = r.SignedByte(), n = r.Var();
				var timeline = type switch
				{
					0 => Timeline(SpineTimelineKind.PathConstraintPosition, "PathConstraintPosition", index),
					1 => Timeline(SpineTimelineKind.PathConstraintSpacing, "PathConstraintSpacing", index),
					2 => Timeline(SpineTimelineKind.PathConstraintMix, "PathConstraintMix", index),
					_ => throw new InvalidDataException("Unknown path timeline: " + type)
				};
				FloatFrames(timeline, n, type == 2 ? 2 : 1);
			}
		}
		for (int a = 0, ac = r.Var(); a < ac; a++)
		{
			int skin = r.Var();
			for (int b = 0, bc = r.Var(); b < bc; b++)
			{
				int slot = r.Var();
				for (int c = 0, cc = r.Var(); c < cc; c++)
				{
					string attachment = r.String() ?? throw new InvalidDataException("Null Deform attachment name");
					var timeline = Timeline(SpineTimelineKind.Deform, "Deform", skin, slot, attachment); timeline.DeformSlot = slot; timeline.DeformAttachment = attachment;
					int n = r.Var(); var types = new List<byte>();
					for (int i = 0; i < n; i++)
					{
						Time(timeline, i, n); int end = r.Var(), start = 0;
						if (end != 0) { start = r.Var(); for (int k = 0; k < end; k++) timeline.Vertices.Add((float)r.Float()); }
						timeline.Integers.Add(start); timeline.Counts.Add(end);
						Curves(timeline, types, i, n);
					}
				}
			}
		}
		int count = r.Var(); var draw = count == 0 ? null : Timeline(SpineTimelineKind.DrawOrder, "DrawOrder", 0);
		for (int i = 0; i < count; i++)
		{
			Time(draw!, i, count); int pairs = r.Var(); draw!.Counts.Add(pairs);
			// Positive varints stay uint32, including wrapped negative draw offsets.
			for (int j = 0; j < pairs; j++) { draw.DrawOffsets.Add(unchecked((uint)r.UnsignedVar())); draw.DrawOffsets.Add(unchecked((uint)r.UnsignedVar())); }
		}
		count = r.Var(); var fired = count == 0 ? null : Timeline(SpineTimelineKind.Event, "Event", 0);
		for (int i = 0; i < count; i++)
		{
			Time(fired!, i, count); events.Add(fired!.Times[i]);
			fired.Integers.Add(r.Var()); fired.Integers.Add(r.Var(false)); fired.Values.Add((float)r.Float()); Name(fired, r.Bool() ? r.String() : null);
		}
		return new AnimKitAnimation(duration, properties, timelines, events);
	}
}
