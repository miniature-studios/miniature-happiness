#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace DA_Assets.Shared
{
    internal class DAJson
    {
#if JSONNET_EXISTS
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Error = (sender, error) => error.ErrorContext.Handled = true,
            Formatting = Formatting.Indented
        };
#endif
        public static string ToJson(object obj)
        {
#if JSONNET_EXISTS
            return JsonConvert.SerializeObject(obj, settings);
#else
            return "";
#endif
        }

        public static T FromJson<T>(string json)
        {
#if JSONNET_EXISTS
            return JsonConvert.DeserializeObject<T>(json, settings);
#else
            return default(T);
#endif
        }

        public static IEnumerator FromJson<T>(string json, Return<T> callback)
        {
            DAResult<T> @return = new DAResult<T>();

            bool endThread = false;

            new Thread(() =>
            {
                try
                {
#if JSONNET_EXISTS == false
                    throw new MissingComponentException("Json.NET packaghe is not installed.");
#endif
                    JFResult jfr = DAFormatter.Format<T>(json);

                    if (jfr.IsValid == false)
                    {
                        throw new Exception("Not valid json.");
                    }

                    if (jfr.MatchTargetType == false)
                    {
                        throw new InvalidCastException("The input json does not match the target type.");
                    }
#if JSONNET_EXISTS
                    @return.Object = JsonConvert.DeserializeObject<T>(json, settings);
#endif
                    @return.Success = true;
                    endThread = true;
                }
                catch (MissingComponentException ex)
                {
                    @return.Success = false;
                    @return.Error = new IDAError(455, null, ex);
                    endThread = true;
                }
                catch (ThreadAbortException ex)
                {
                    @return.Success = false;
                    @return.Error = new IDAError(-1, null, ex);
                    endThread = true;
                }
                catch (Exception ex)
                {
                    @return.Success = false;
                    @return.Error = new IDAError(422, null, ex);
                    endThread = true;
                }
            }).Start();

            while (endThread == false)
            {
                yield return WaitFor.Delay01();
            }

            callback.Invoke(@return);
        }
    }
}