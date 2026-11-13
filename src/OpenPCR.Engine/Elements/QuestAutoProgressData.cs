using Elements.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public partial class QuestAutoProgressData
	{
		public class ResultData
		{
			public ResultData()
			{
				throw new global::OpenPCR.OutOfBoundaryException("Elements.QuestAutoProgressData+ResultData::.ctor(): Native quest progression result production is outside the prepared battle input boundary.");
			}
		}

		private Dictionary<int, ResultData> resultDataDic;
		public bool IsEnable { get; set; }
		public bool IsStopped { get; set; }

		public QuestAutoProgressData()
		{
			resultDataDic = new Dictionary<int, ResultData>();
		}
	}
}
