using global::OpenPCR.Runtime.Api;
using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForAbromalStateIconController
	{
		Coroutine StartCoroutine(IEnumerator _routine);

		void StopCoroutine(IEnumerator _routine);

		void OpenBonusDetailDialog();
	}
}
