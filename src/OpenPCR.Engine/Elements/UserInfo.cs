using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class UserInfo
	{
		public global::System.Int64 ViewerId { get; private set; }
		public global::System.String UserName { get; private set; }
		public global::System.String UserComment { get; private set; }
		public EmblemData Emblem { get; private set; }
		public global::System.Int32 TeamLevel { get; private set; }
		public global::System.Int32 UserStamina { get; private set; }
		public global::System.Int32 TeamExp { get; private set; }
		public global::System.Int32 FavoriteUnitId { get; private set; }
		public global::System.Int32 TutorialFlag { get; private set; }
		public global::System.Int32 UserBirth { get; private set; }
		public global::System.Int64 StaminaFullRecoveryTime { get; private set; }
		public global::System.Int32 ArenaRank { get; private set; }
		public global::System.Int32 InviteAcceptFlag { get; private set; }

		public UserInfo()
		{
			initializeUserInfo();
		}

		private void initializeUserInfo()
		{
			ViewerId = 0;
			UserName = null;
			UserComment = null;
			Emblem = null;
			TeamLevel = 0;
			UserStamina = 0;
			TeamExp = 0;
			FavoriteUnitId = 0;
			TutorialFlag = 0;
			UserBirth = 0;
			StaminaFullRecoveryTime = 0L;
			ArenaRank = 0;
			InviteAcceptFlag = 0;
		}
	}
}
