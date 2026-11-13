namespace Elements
{
	public class LabyrinthBattleFinishReceiveParam : BaseReceiveParam
	{
		[global::OpenPCR.Migration("22430a9b-2754-4b5d-b008-9791dbe9b6bc")]
		public LabyrinthBattleFinishReceiveParam()
		{ throw new global::OpenPCR.OutOfBoundaryException("Labyrinth server response is outside a single battle simulation."); }
	}
}
