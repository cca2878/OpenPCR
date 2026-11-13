using global::OpenPCR.Runtime.Impl;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Elements;
using Elements.Battle;
using Elements.Battle.Core;
using Elements.Data;
using OpenPCR;

internal static class PrincessMovieTests
{
	internal static void Run(Action<bool, string> check)
	{
		var random = ScriptingRandom.Current;
		const eMovieType type = eMovieType.PRINCESS_FORM_SKILL_NORMAL;
		const int skill = 1804001;
		var owner = (MovieManager)RuntimeHelpers.GetUninitializedObject(typeof(MovieManager));
		void Reject(Action action)
		{
			try { action(); }
			catch (InvalidOperationException) { check(true, "Rejected invalid movie input sequence."); return; }
			throw new Exception("Invalid movie input sequence was accepted.");
		}
		Reject(() => owner.GetDurationTime(type, skill, 1));
		PreparedPrincessMoviePlayback.Bind(owner, (movieType, id, index) => movieType == (int)type && id == skill && index is 1 or 2);
		try
		{
			Reject(() => owner.GetDurationTime(type, skill, 1));
			Reject(() => PreparedPrincessMoviePlayback.Start(owner, (int)eMovieType.CUT_IN, skill, 1));
			Reject(() => PreparedPrincessMoviePlayback.Start(owner, (int)type, skill + 1, 1));
			Reject(() => PreparedPrincessMoviePlayback.Start(owner, (int)type, skill, 3));
			PreparedPrincessMoviePlayback.Start(owner, (int)type, skill, 1);
			check(owner.GetDurationTime(type, skill, 1) == 0f, "First query did not preserve the original Unity coroutine yield.");
			Reject(() => owner.GetSeekPositionTime(type, skill, 1));
			Reject(() => owner.SetPlayEndTypeDestroy(type, skill, 1));
			Reject(() => PreparedPrincessMoviePlayback.Start(owner, (int)type, skill, 1));
			float duration = owner.GetDurationTime(type, skill, 1), seek = owner.GetSeekPositionTime(type, skill, 1);
			check(duration > 0f && seek >= duration / 2f && seek + 1f / 30f + (1f / 30f) * 0.5f >= duration, "Completed decoder input did not satisfy original midpoint/end predicates.");
			owner.SetPlayEndTypeDestroy(type, skill, 1);
			Reject(() => owner.SetPlayEndTypeDestroy(type, skill, 1));
			PreparedPrincessMoviePlayback.Start(owner, (int)type, skill, 1);
			check(owner.GetDurationTime(type, skill, 1) == 0f, "Reused segment kept old readiness.");
			PreparedPrincessMoviePlayback.Start(owner, (int)type, skill, 2);
			check(owner.GetDurationTime(type, skill, 2) == 0f, "Independent segment inherited readiness.");
			Reject(() => owner.GetDurationTime(type, skill, long.MaxValue));

			// The actual mixed Core method must retain the gameplay write before
			// projecting render-only movie preparation; no owner/movie resources needed.
			var manager = new BattleManager();
			var temp = (TempData)RuntimeHelpers.GetUninitializedObject(typeof(TempData));
			var processor = new PrincessFormProcessor();
			const BindingFlags fields = BindingFlags.Instance | BindingFlags.NonPublic;
			typeof(BattleManager).GetField("tempData", fields)!.SetValue(manager, temp);
			typeof(PrincessFormProcessor).GetField("battleManager", fields)!.SetValue(processor, manager);
			typeof(PrincessFormProcessor).GetField("movieManager", fields)!.SetValue(processor, owner);
			processor.Prepare();
			check(manager.IsPrincessSkillRunning, "Prepared movie output erased the original princess skill flag.");
		}
		finally { PreparedPrincessMoviePlayback.Release(owner); }
		Reject(() => owner.GetDurationTime(type, skill, 1));
		check(random.Equals(ScriptingRandom.Current), "Movie boundary consumed battle RNG.");
	}
}
