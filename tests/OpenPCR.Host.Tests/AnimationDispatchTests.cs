using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;

internal static class AnimationDispatchTests
{
	internal static void Run(Action<bool, string> check)
	{
		var data = new SkeletonData();
		((ExposedList<BoneData>)typeof(SkeletonData).GetField("bones", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(data)!).Add(new BoneData(0, "root", null!));
		var timeline = new TranslateTimeline(1, 0);
		timeline.SetFrame(0, 0f, 2f, 3f);
		((Dictionary<string, Animation>)typeof(SkeletonData).GetField("animations", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(data)!).Add("move", new Animation("move", new ExposedList<Timeline> { timeline }, 1f));
		BattleAnimKitController controller = (BattleAnimKitController)RuntimeHelpers.GetUninitializedObject(typeof(BattleAnimKitController));
		controller.skeleton = new Skeleton(data);
		controller.state = new AnimationState(new AnimationStateData(data));
		controller.valid = true;
		controller.timeScale = 1f;
		controller.AnimationName = "move";
		var entry = controller.state.GetCurrent(0);
		var bone = controller.skeleton.FindBone("root");
		check(entry.time == 0f && bone.WorldY == 0f, "AnimationName dispatched to inherited Update and advanced the battle pose.");
		((global::OpenPCR.AnimKit.Scene.SkeletonAnimation)controller).Update();
		((global::OpenPCR.AnimKit.Scene.SkeletonRenderer)controller).LateUpdate();
		check(entry.time == 0f && bone.WorldY == 0f, "Automatic virtual Update/LateUpdate bypassed Native empty overrides.");
		controller.Update(0.25f);
		check(entry.time == 0.25f && bone.WorldY == 3f, "Native empty Update suppressed the explicit battle animation driver.");
	}
}
