using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using global::OpenPCR.Foundation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public static class ExtensionMethods
	{

		[global::OpenPCR.Migration("7462bb0b-3892-4eda-b216-6107eefe6a35")]
		public static void SetLocalPosX(this global::OpenPCR.Runtime.Api.Transform _self, float _x)
		{
			Vector3 position = _self.localPosition;
			_self.localPosition = new Vector3(_x, position.y, position.z);
		}

		[global::OpenPCR.Migration("687481d9-b11e-4908-abab-a0f92da799d3")]
		public static void SetLocalPosY(this global::OpenPCR.Runtime.Api.Transform _self, float _y)
		{
			Vector3 position = _self.localPosition;
			_self.localPosition = new Vector3(position.x, _y, position.z);
		}

		[global::OpenPCR.Migration("3a2f4c10-96fb-49eb-a488-b2783bd53ea8")]
		public static bool IsNullOrEmpty(this string _self)
		{
			return string.IsNullOrEmpty(_self);
		}

		public static string Format(this string _self, params object[] _args)
		{
			return string.Format(_self, _args);
		}

		[global::OpenPCR.Migration("2753b905-ce9c-42dc-94e1-e9cde44f1c53")]
		public static void SetWorldPosX(this MonoBehaviour _self, float _x)
		{
			Vector3 position = _self.transform.position;
			_self.transform.position = new Vector3(_x, position.y, position.z);
		}

		[global::OpenPCR.Migration("0f22b07e-e52e-4351-918d-d58f4dedfe10")]
		public static void SetWorldPosY(this MonoBehaviour _self, float _y)
		{
			Vector3 position = _self.transform.position;
			_self.transform.position = new Vector3(position.x, _y, position.z);
		}

		public static void SetWorldPosX(this Transform _self, float _x)
		{
			Vector3 position = _self.position;
			_self.position = new Vector3(_x, position.y, position.z);
		}

		public static readonly WaitForEndOfFrame WAIT_FOR_END_OF_FRAME = new WaitForEndOfFrame();
		[global::OpenPCR.Migration("fd17151b-601c-4a8d-9f7f-c96d3683dc59")]
		public static void SetActive(this MonoBehaviour _self, bool _isActive)
		{
			_self.gameObject.SetActive(_isActive);
		}

		public static void SetActiveWithCheck(this GameObject _self, bool _isActive)
		{
			if (_self != null)
				_self.SetActive(_isActive);
		}

		[global::OpenPCR.Migration("16fd5129-689b-4f8e-a5fa-973da110fb52")]
		public static void SetActiveWithCheck(this MonoBehaviour _self, bool _isActive)
		{
			if (_self != null)
				_self.gameObject.SetActive(_isActive);
		}

		[global::OpenPCR.Migration("00f54643-4135-4d1b-9357-b8654c62a5db")]
		public static void ChangeChildObjectLayer(this Transform _self, int _layer)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.ExtensionMethods::ChangeChildObjectLayer(OpenPCR.Runtime.Api.Transform,System.Int32):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("33f3266b-e538-41ed-8ada-5e641038d700")]
		public static IEnumerator Timer(this MonoBehaviour obj, Action onComplete)
		{
			IEnumerator enumerator = timerFrameCoroutine(obj, onComplete);
			obj.StartCoroutine(enumerator);
			return enumerator;
		}

		[global::OpenPCR.Migration("76445ee0-c1b5-4a8d-b934-890f15011d26")]
		public static IEnumerator Timer(this MonoBehaviour obj, float time, Action onComplete)
		{
			return obj.Timer(time, 0f, onComplete);
		}

		[global::OpenPCR.Migration("0088cd12-51c9-4f95-8e04-c0da30e2d189")]
		public static IEnumerator Timer(this MonoBehaviour obj, float time, float delay, Action onComplete)
		{
			IEnumerator enumerator = timerCoroutine(obj, time, delay, onComplete);
			obj.StartCoroutine(enumerator);
			return enumerator;
		}

		[global::OpenPCR.Migration("c4ae652a-d080-4df8-ad6f-9ca70a11f507")]
		private static IEnumerator timerFrameCoroutine(MonoBehaviour obj, Action onComplete)
		{
			yield return WAIT_FOR_END_OF_FRAME;
			onComplete.Call();
		}

		[global::OpenPCR.Migration("d11886ed-85d1-4b44-b585-046ce88c46b7")]
		private static IEnumerator timerCoroutine(MonoBehaviour obj, float time, float delay, Action onComplete)
		{
			if (delay > 0f)
			{
				yield return new WaitForSeconds(delay);
			}

			yield return new WaitForSeconds(time);
			onComplete.Call();
		}

		[global::OpenPCR.Migration("27b8a189-9bfb-4a8c-9eca-a7b65fe8977f")]
		public static void AddIfNoContains<TKey, TValue>(this Dictionary<TKey, TValue> _dictionary, TKey _key, TValue _value)
		{
			if (!_dictionary.ContainsKey(_key))
			{
				_dictionary.Add(_key, _value);
			}
		}

		public static void AddIfNoContains<T>(this List<T> _list, T _value)
		{
			if (!_list.Contains(_value))
				_list.Add(_value);
		}

		public static void AddRangeIfNoContains<T>(this List<T> _list, List<T> _add)
		{
			for (int i = 0; i < _add.Count; i++)
				_list.AddIfNoContains(_add[i]);
		}
	}
}
