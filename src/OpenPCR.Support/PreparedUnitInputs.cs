using System;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Prepared growth results. These are inputs, never runtime HP/ATK or passive results.</summary>
public sealed class PreparedUnitInputs
{
	private readonly long[] attributes;
	public long DamagedEnergyHp { get; }
	public long DamagedEnergyDef { get; }
	public long DamagedEnergyMagicDef { get; }
	public PreparedUnitInputs(ReadOnlySpan<long> attributesByParamType, long damagedEnergyHp, long damagedEnergyDef, long damagedEnergyMagicDef)
	{
		if (attributesByParamType.Length != 17) throw new ArgumentException("All seventeen prepared attributes are required.", nameof(attributesByParamType));
		attributes = attributesByParamType.ToArray();
		for (int i = 1; i < attributes.Length; i++)
			if (attributes[i] < int.MinValue || attributes[i] > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(attributesByParamType), "Only HP is int64; other prepared attributes are int32.");
		DamagedEnergyHp = damagedEnergyHp; DamagedEnergyDef = damagedEnergyDef; DamagedEnergyMagicDef = damagedEnergyMagicDef;
	}
	public long Attribute(int paramType) => paramType is >= 1 and <= 17 ? attributes[paramType - 1] : throw new ArgumentOutOfRangeException(nameof(paramType));
	public long DamagedEnergy(int paramType) => paramType switch
	{
		1 => DamagedEnergyHp,
		3 => DamagedEnergyDef,
		5 => DamagedEnergyMagicDef,
		_ => throw new ArgumentOutOfRangeException(nameof(paramType), "Damaged-energy input requires HP, DEF or MAGIC_DEF.")
	};
}

/// <summary>Per-UnitData identity binding; no new field or convenience member in the mirrored type.</summary>
public static class PreparedUnitInputBinding
{
	private static readonly ConditionalWeakTable<object, PreparedUnitInputs> inputs = new();
	public static void Bind(object unitData, PreparedUnitInputs input)
	{
		ArgumentNullException.ThrowIfNull(unitData); ArgumentNullException.ThrowIfNull(input);
		lock (inputs)
		{
			if (inputs.TryGetValue(unitData, out var current))
			{
				if (!ReferenceEquals(current, input)) throw new InvalidOperationException("Prepared unit input cannot be replaced.");
				return;
			}
			inputs.Add(unitData, input);
		}
	}
	public static PreparedUnitInputs Require(object unitData) => inputs.TryGetValue(unitData, out var input) ? input :
		throw new InvalidOperationException("UnitData has no prepared attribute/damaged-energy input; battle-time growth is not permitted.");
	public static void Copy(object source, object copy) => Bind(copy, Require(source));
	public static void Release(object unitData) => inputs.Remove(unitData);
}
