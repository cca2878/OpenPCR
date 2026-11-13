using System;
using System.Collections.Concurrent;

namespace OpenPCR;

/// <summary>
/// One shared box per (type, bit pattern) for immutable cells of process-wide masterdata caches. Bit patterns keep -0.0 and
/// NaN payloads distinct; other values, strings included, are returned unchanged.
/// </summary>
public static class SharedBoxes
{
	private static readonly ConcurrentDictionary<(Type, long), object> boxes = new();
	private static readonly object True = true, False = false;
	public static object? Share(object? value) => value switch
	{
		long v => boxes.GetOrAdd((typeof(long), v), static (_, box) => box, (object)v),
		int v => boxes.GetOrAdd((typeof(int), v), static (_, box) => box, (object)v),
		double v => boxes.GetOrAdd((typeof(double), BitConverter.DoubleToInt64Bits(v)), static (_, box) => box, (object)v),
		float v => boxes.GetOrAdd((typeof(float), BitConverter.SingleToInt32Bits(v)), static (_, box) => box, (object)v),
		bool v => v ? True : False,
		_ => value,
	};
}
