using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace OpenPCR.Foundation
{
	public class SavedataManager : MonoBehaviour, IManager
	{
		public SavedataManager()
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::.ctor(): runtime capability is unavailable.");
		}

		public void Save()
		{
			throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::Save():System.Void: runtime capability is unavailable.");
		}
		public int GetInt(eSaveKey _eKey, int _defaultValue = 0, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::GetInt(Common.eSaveKey,System.Int32,System.Boolean):System.Int32: runtime capability is unavailable."); }
		public int GetIntForStringKey(string _key, int _defaultValue = 0, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::GetIntForStringKey(System.String,System.Int32,System.Boolean):System.Int32: runtime capability is unavailable."); }
		public void SetInt(eSaveKey _eKey, int _value, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::SetInt(Common.eSaveKey,System.Int32,System.Boolean):System.Void: runtime capability is unavailable."); }
		public void SetIntForStringKey(string _key, int _value, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::SetIntForStringKey(System.String,System.Int32,System.Boolean):System.Void: runtime capability is unavailable."); }
		public bool GetBool(eSaveKey _eKey, bool _defaultValue = false, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::GetBool(Common.eSaveKey,System.Boolean,System.Boolean):System.Boolean: runtime capability is unavailable."); }
		public bool SetBool(eSaveKey _eKey, bool _value, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::SetBool(Common.eSaveKey,System.Boolean,System.Boolean):System.Boolean: runtime capability is unavailable."); }
		public string GetString(eSaveKey _eKey, string _defaultValue = "", bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::GetString(Common.eSaveKey,System.String,System.Boolean):System.String: runtime capability is unavailable."); }
		public void SetString(eSaveKey _eKey, string _value, bool withOutViewID = false) { throw new OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::SetString(Common.eSaveKey,System.String,System.Boolean):System.Void: runtime capability is unavailable."); }
		public bool SetIntArray(eSaveKey _eKey, int[] _intArray)
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.SavedataManager::SetIntArray(Common.eSaveKey,System.Int32[]):System.Boolean: runtime capability is unavailable.");
		}
	}
}
