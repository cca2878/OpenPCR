namespace Elements
{
	public class ChangeColorEffectEx : ChangeColorEffect
	{
		[global::OpenPCR.Migration("4351e384-bc52-4678-b189-e7d7d8e64be5")]
		public override void SetStartTime(float _starttime)
		{
			base.startTime = _starttime;
		}

		[global::OpenPCR.Migration("1a7c276b-ec40-4aca-b3d5-5f506e87b930")]
		public override ePauseType GetPauseType()
		{
			return ePauseType.NO_DIALOG;
		}
	}
}
