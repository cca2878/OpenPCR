using global::OpenPCR.Runtime.Api;
using Elements.Battle;

namespace Elements
{
	public class PartsHatsuneDefenseTimeGauge : MonoBehaviour, ISingletonField
	{
		private IBattleManagerForPartsBossGauge battleManager;


		private void Awake()
		{
			battleManager = this.CreateSingletonTree().Get<BattleManager>();
		}
	}
}
