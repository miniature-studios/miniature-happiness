using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable IDE0052

namespace DA_Assets.FCU
{
    [Serializable]
    public class RequestSender : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] float pbarProgress;
        public float PbarProgress => pbarProgress;

        [SerializeField] float pbarBytes;
        public float PbarBytes => pbarBytes;

        [SerializeField] int _requestCount;
        [SerializeField] bool _timeoutActive;
        [SerializeField] int _remainingTime;

        private static int requestCount = 0;
        private static bool timeoutActive = false;
        private static int remainingTime = 0;

        public void RefreshLimiterData()
        {
            _requestCount = requestCount;
            _timeoutActive = timeoutActive;
            _remainingTime = remainingTime;
        }

        private IEnumerator CheckRateLimit()
        {
            while (requestCount >= FcuConfig.Instance.ApiRequestsCountLimit)
            {
                if (timeoutActive == false)
                {
                    timeoutActive = true;
                    remainingTime = FcuConfig.Instance.ApiTimeoutSec;
                    LogRemainingTime().StartDARoutine(monoBeh);
                }

                yield return WaitFor.Delay1();
            }

            requestCount++;
        }

        private IEnumerator LogRemainingTime()
        {
            while (remainingTime > 0)
            {
                DALogger.Log(FcuLocKey.log_api_waiting.Localize(remainingTime));
                RefreshLimiterData();
                yield return WaitFor.Delay1();
                remainingTime--;
            }

            requestCount = 0;
            timeoutActive = false;
        }

        public IEnumerator SendRequest<T>(DARequest request, Return<T> @return)
        {
            yield return CheckRateLimit();

            UnityWebRequest webRequest;

            switch (request.RequestType)
            {
                case RequestType.Post:
                    webRequest = UnityWebRequest.Post(request.Query, request.WWWForm);
                    break;
                default:
                    webRequest = UnityWebRequest.Get(request.Query);
                    break;
            }


            using (webRequest)
            {
                if (request.RequestHeader.IsDefault() == false)
                {
                    webRequest.SetRequestHeader(request.RequestHeader.Name, request.RequestHeader.Value);
                }

                try
                {
                    webRequest.SendWebRequest();
                }
                catch (InvalidOperationException)
                {
                    DALogger.LogError(FcuLocKey.log_enable_http_project_settings.Localize());
                    monoBeh.AssetTools.StopImport();
                    yield break;
                }
                catch (Exception ex)
                {
                    DALogger.LogException(ex);
                }

                yield return UpdateRequestProgressBar(webRequest);
                yield return MoveRequestProgressBarToEnd();

                DAResult<T> result = new DAResult<T>();

                if (request.RequestType == RequestType.GetFile)
                {
                    result.Success = true;
                    result.Object = (T)(object)webRequest.downloadHandler.data;
                }
                else
                {
                    string text = webRequest.downloadHandler.text;

                    request.WriteLog(text).StartDARoutine(monoBeh);

                    if (typeof(T) == typeof(string))
                    {
                        result.Success = true;
                        result.Object = (T)(object)text;
                    }
                    else
                    {
                        yield return TryParseResponse<T>(text, request, webRequest, x => result = x);
                    }
                }

                @return.Invoke(result);
            }
        }

        private IEnumerator TryParseResponse<T>(string text, DARequest request, UnityWebRequest webRequest, Return<T> @return)
        {
            DAResult<T> finalResult = new DAResult<T>();

            bool isRequestError;
#if UNITY_2020_1_OR_NEWER
            isRequestError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
            isRequestError = webRequest.isNetworkError || webRequest.isHttpError;
#endif

            monoBeh.Log($"TryParseResponse | 0");

            if (isRequestError)
            {
                finalResult.Success = false;

                if (webRequest.error.Contains("SSL"))
                {
                    monoBeh.Log($"TryParseResponse | 1");

                    finalResult.Error = new IDAError(909, text);
                }
                else
                {
                    monoBeh.Log($"TryParseResponse | 2");

                    finalResult.Error = new IDAError((int)webRequest.responseCode, webRequest.error);
                }
            }
            else if (text.Contains("<pre>Cannot GET "))
            {
                finalResult.Error = new IDAError(404, text);
            }
            else
            {
                monoBeh.Log($"TryParseResponse | 3");

                DAResult<T> result1 = default;
                yield return DAJson.FromJson<T>(text, x => result1 = x);

                if (result1.Success)
                {
                    monoBeh.Log($"TryParseResponse | 4");

                    finalResult.Success = true;
                    finalResult.Object = result1.Object;

                    if (request.Name == RequestName.Project)
                    {
                        monoBeh.ProjectCacher.Cache(text, result1.Object, monoBeh.Settings.MainSettings.ProjectUrl).StartDARoutine(monoBeh);
                    }
                }
                else
                {
                    monoBeh.Log($"TryParseResponse | 5 | {result1.Error.Status} | {result1.Error.Message} | {result1.Error.Exception}");

                    DAResult<IDAError> result2 = default;
                    yield return DAJson.FromJson<IDAError>(text, x => result2 = x);

                    if (result2.Success)
                    {
                        monoBeh.Log($"TryParseResponse | 6");

                        finalResult.Success = false;
                        finalResult.Error = result2.Object;
                    }
                    else
                    {
                        monoBeh.Log($"TryParseResponse | 7");

                        finalResult.Success = false;
                        finalResult.Error = result1.Error;
                    }
                }
            }

            if (finalResult.Success == false)
            {
                //errorCount++
            }

            @return.Invoke(finalResult);
        }

        private IEnumerator UpdateRequestProgressBar(UnityWebRequest webRequest)
        {
            while (webRequest.isDone == false)
            {
                if (pbarProgress < 1f)
                {
                    pbarProgress += WaitFor.Delay001().WaitTimeF;
                }
                else
                {
                    pbarProgress = 0;
                }

                if (webRequest.downloadedBytes == 0)
                {
                    pbarBytes += 100;
                }
                else
                {
                    pbarBytes = webRequest.downloadedBytes;
                }

                yield return WaitFor.Iterations(1);
            }
        }

        private IEnumerator MoveRequestProgressBarToEnd()
        {
            while (true)
            {
                if (pbarProgress < 1f)
                {
                    pbarProgress += WaitFor.Delay001().WaitTimeF;
                    yield return null;
                }
                else
                {
                    pbarProgress = 0f;
                    pbarBytes = 0f;
                    break;
                }
            }
        }
    }

    public struct DARequest
    {
        public RequestName Name;
        public string Query;
        public RequestType RequestType;
        public RequestHeader RequestHeader;
        public WWWForm WWWForm;
    }

    public struct RequestHeader
    {
        public string Name;
        public string Value;
    }

    public enum RequestType
    {
        Get,
        Post,
        GetFile,
    }

    public enum RequestName
    {
        None,
        Project,
    }
}
