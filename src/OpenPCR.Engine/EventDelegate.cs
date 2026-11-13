using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenPCR.UICompat
{

	[Serializable]
	[global::OpenPCR.ImplicitMigration("a285c925-07b2-4aa7-842c-45cc702220ca")]
	public class EventDelegate
	{
		private static int s_Hash = "EventDelegate".GetHashCode();

		[global::OpenPCR.Migration("2eac01db-38d3-48e4-a685-bb6d3c79d996")]
		public static EventDelegate Set(List<EventDelegate> list, Callback callback)
		{
			if (list == null)
				return null;
			EventDelegate ev = new EventDelegate(callback);
			list.Clear();
			list.Add(ev);
			return ev;
		}

		public static void Set(List<EventDelegate> list, EventDelegate del)
		{
			if (list != null)
			{
				list.Clear();
				list.Add(del);
			}
		}

		[Serializable]
		public class Parameter
		{
			public global::OpenPCR.Runtime.Api.Object obj;
			public string field;
			[NonSerialized]
			private object mValue;
			[NonSerialized]
			public Type expectedType;
			[NonSerialized]
			public bool cached;
			[NonSerialized]
			public PropertyInfo propInfo;
			[NonSerialized]
			public FieldInfo fieldInfo;
			public object value
			{
				get
				{
					if (mValue != null)
					{
						return mValue;
					}

					if (!cached)
					{
						cached = true;
						fieldInfo = null;
						propInfo = null;
						if (obj != null && !string.IsNullOrEmpty(field))
						{
							Type type = obj.GetType();
							propInfo = type.GetProperty(field);
							if (propInfo == null)
							{
								fieldInfo = type.GetField(field);
							}
						}
					}

					if (propInfo != null)
					{
						return propInfo.GetValue(obj, null);
					}

					if (fieldInfo != null)
					{
						return fieldInfo.GetValue(obj);
					}

					if (obj != null)
					{
						return obj;
					}

					if (expectedType != null && expectedType.IsValueType)
					{
						return null;
					}

					return Convert.ChangeType(null, expectedType);
				}

				set
				{
					mValue = value;
				}
			}

			public Type type
			{
				get
				{
					if (mValue != null)
					{
						return mValue.GetType();
					}

					if (obj == null)
					{
						return typeof(void);
					}

					return obj.GetType();
				}
			}

			public Parameter(object val)
			{
				mValue = val;
			}

			public Parameter()
			{
			}
		}

		public delegate void Callback();
		[SerializeField]
		private MonoBehaviour mTarget;
		[SerializeField]
		private string mMethodName;
		[SerializeField]
		private Parameter[] mParameters;
		public bool oneShot;
		[NonSerialized]
		private Callback mCachedCallback;
		[NonSerialized]
		private bool mRawDelegate;
		[NonSerialized]
		private bool mCached;
		[NonSerialized]
		private MethodInfo mMethod;
		[NonSerialized]
		private ParameterInfo[] mParameterInfos;
		[NonSerialized]
		private object[] mArgs;
		public MonoBehaviour target
		{
			get
			{
				return mTarget;
			}

			set
			{
				mTarget = value;
				mCachedCallback = null;
				mRawDelegate = false;
				mCached = false;
				mMethod = null;
				mParameterInfos = null;
				mParameters = null;
			}
		}

		public string methodName
		{
			get
			{
				return mMethodName;
			}

			set
			{
				mMethodName = value;
				mCachedCallback = null;
				mRawDelegate = false;
				mCached = false;
				mMethod = null;
				mParameterInfos = null;
				mParameters = null;
			}
		}

		public Parameter[] parameters
		{
			get
			{
				if (!mCached)
					Cache();
				return mParameters;
			}
		}

		public bool isValid
		{
			get
			{
				if (!mCached)
				{
					Cache();
				}

				if (!mRawDelegate || mCachedCallback == null)
				{
					if (mTarget != null)
					{
						return !string.IsNullOrEmpty(mMethodName);
					}

					return false;
				}

				return true;
			}
		}

		public EventDelegate()
		{
		}

		public EventDelegate(Callback call)
		{
			Set(call);
		}

		public EventDelegate(MonoBehaviour target, string methodName)
		{
			Set(target, methodName);
		}

		private static string GetMethodName(Callback callback)
		{
			return callback.Method.Name;
		}

		private static bool IsValid(Callback callback)
		{
			if (callback != null)
			{
				return callback.Method != null;
			}

			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return !isValid;
			}

			if (obj is Callback)
			{
				Callback callback = obj as Callback;
				if (callback.Equals(mCachedCallback))
				{
					return true;
				}

				MonoBehaviour monoBehaviour = callback.Target as MonoBehaviour;
				if (mTarget == monoBehaviour)
				{
					return string.Equals(mMethodName, GetMethodName(callback));
				}

				return false;
			}

			if (obj is EventDelegate)
			{
				EventDelegate eventDelegate = obj as EventDelegate;
				if (mTarget == eventDelegate.mTarget)
				{
					return string.Equals(mMethodName, eventDelegate.mMethodName);
				}

				return false;
			}

			return false;
		}

		[global::OpenPCR.Migration("6b8519ec-508c-4e59-986c-af77c52c5812")]
		public override int GetHashCode()
		{ throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.UICompat.EventDelegate::GetHashCode():System.Int32: runtime capability is unavailable."); }

		private void Set(Callback call)
		{
			Clear();
			if (call != null && IsValid(call))
			{
				mTarget = call.Target as MonoBehaviour;
				if (mTarget == null)
				{
					mRawDelegate = true;
					mCachedCallback = call;
					mMethodName = null;
				}
				else
				{
					mMethodName = GetMethodName(call);
					mRawDelegate = false;
				}
			}
		}

		public void Set(MonoBehaviour target, string methodName)
		{
			Clear();
			mTarget = target;
			mMethodName = methodName;
		}

		private void Cache()
		{
			mCached = true;
			if (mRawDelegate || (mCachedCallback != null && !(mCachedCallback.Target as MonoBehaviour != mTarget) && !(GetMethodName(mCachedCallback) != mMethodName)) || !(mTarget != null) || string.IsNullOrEmpty(mMethodName))
			{
				return;
			}

			Type type = mTarget.GetType();
			mMethod = null;
			while (type != null)
			{
				try
				{
					mMethod = type.GetMethod(mMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (mMethod != null)
					{
						break;
					}
				}
				catch (Exception)
				{
				}

				type = type.BaseType;
			}

			if (mMethod == null || mMethod.ReturnType != typeof(void))
			{
				return;
			}

			mParameterInfos = mMethod.GetParameters();
			if (mParameterInfos.Length == 0)
			{
				mCachedCallback = (Callback)Delegate.CreateDelegate(typeof(Callback), mTarget, mMethodName);
				mArgs = null;
				mParameters = null;
				return;
			}

			mCachedCallback = null;
			if (mParameters == null || mParameters.Length != mParameterInfos.Length)
			{
				mParameters = new Parameter[mParameterInfos.Length];
				int i = 0;
				for (int num = mParameters.Length; i < num; i++)
				{
					mParameters[i] = new Parameter();
				}
			}

			int j = 0;
			for (int num2 = mParameters.Length; j < num2; j++)
			{
				mParameters[j].expectedType = mParameterInfos[j].ParameterType;
			}
		}

		public bool Execute()
		{
			if (!mCached)
			{
				Cache();
			}

			if (mCachedCallback != null)
			{
				mCachedCallback();
				return true;
			}

			if (mMethod != null)
			{
				if (mParameters == null || mParameters.Length == 0)
				{
					mMethod.Invoke(mTarget, null);
				}
				else
				{
					if (mArgs == null || mArgs.Length != mParameters.Length)
					{
						mArgs = new object[mParameters.Length];
					}

					int i = 0;
					for (int num = mParameters.Length; i < num; i++)
					{
						mArgs[i] = mParameters[i].value;
					}

					try
					{
						mMethod.Invoke(mTarget, mArgs);
					}
					catch (ArgumentException ex)
					{
						string text = "Error calling ";
						text = ((!(mTarget == null)) ? string.Concat(text, mTarget.GetType(), ".", mMethod.Name) : (text + mMethod.Name));
						text = text + ": " + ex.Message;
						text += "\n  Expected: ";
						if (mParameterInfos.Length == 0)
						{
							text += "no arguments";
						}
						else
						{
							text += mParameterInfos[0];
							for (int j = 1; j < mParameterInfos.Length; j++)
							{
								text = text + ", " + mParameterInfos[j].ParameterType;
							}
						}

						text += "\n  Received: ";
						if (mParameters.Length == 0)
						{
							text += "no arguments";
						}
						else
						{
							text += mParameters[0].type;
							for (int k = 1; k < mParameters.Length; k++)
							{
								text = text + ", " + mParameters[k].type;
							}
						}

						text += "\n";
					}

					int l = 0;
					for (int num2 = mArgs.Length; l < num2; l++)
					{
						if (mParameterInfos[l].IsIn || mParameterInfos[l].IsOut)
						{
							mParameters[l].value = mArgs[l];
						}

						mArgs[l] = null;
					}
				}

				return true;
			}

			return false;
		}

		public void Clear()
		{
			mTarget = null;
			mMethodName = null;
			mRawDelegate = false;
			mCachedCallback = null;
			mParameters = null;
			mCached = false;
			mMethod = null;
			mParameterInfos = null;
			mArgs = null;
		}

		[global::OpenPCR.Migration("940b2642-c369-4d46-9619-e68dcef480d6")]
		public static void Execute(List<EventDelegate> list)
		{
			if (list == null)
			{
				return;
			}

			int num = 0;
			while (num < list.Count)
			{
				EventDelegate eventDelegate = list[num];
				if (eventDelegate != null)
				{
					try
					{
						eventDelegate.Execute();
					}
					catch (Exception ex)
					{
						_ = ex.InnerException;
					}

					if (num >= list.Count)
					{
						break;
					}

					if (list[num] != eventDelegate)
					{
						continue;
					}

					if (eventDelegate.oneShot)
					{
						list.RemoveAt(num);
						continue;
					}
				}

				num++;
			}
		}

		public static EventDelegate Add(List<EventDelegate> list, Callback callback, bool oneShot)
		{
			if (list != null)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					EventDelegate eventDelegate = list[i];
					if (eventDelegate != null && eventDelegate.Equals(callback))
					{
						return eventDelegate;
					}
				}

				EventDelegate eventDelegate2 = new EventDelegate(callback);
				eventDelegate2.oneShot = oneShot;
				list.Add(eventDelegate2);
				return eventDelegate2;
			}

			return null;
		}

		public static void Add(List<EventDelegate> list, EventDelegate ev, bool oneShot)
		{
			if (ev.mRawDelegate || ev.target == null || string.IsNullOrEmpty(ev.methodName))
			{
				Add(list, ev.mCachedCallback, oneShot);
			}
			else
			{
				if (list == null)
				{
					return;
				}

				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					EventDelegate eventDelegate = list[i];
					if (eventDelegate != null && eventDelegate.Equals(ev))
					{
						return;
					}
				}

				EventDelegate eventDelegate2 = new EventDelegate(ev.target, ev.methodName);
				eventDelegate2.oneShot = oneShot;
				if (ev.mParameters != null && ev.mParameters.Length != 0)
				{
					eventDelegate2.mParameters = new Parameter[ev.mParameters.Length];
					for (int j = 0; j < ev.mParameters.Length; j++)
					{
						eventDelegate2.mParameters[j] = ev.mParameters[j];
					}
				}

				list.Add(eventDelegate2);
			}
		}
	}

}
