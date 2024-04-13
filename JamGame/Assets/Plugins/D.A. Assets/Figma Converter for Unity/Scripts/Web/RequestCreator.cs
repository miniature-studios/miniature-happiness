using DA_Assets.FCU.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    public class RequestCreator
    {
        public static DARequest CreateImageLinksRequest(string projectUrl, List<string> chunk, FigmaConverterUnity fcu)
        {
            string query = CreateImagesQuery(
                    chunk,
                    projectUrl,
                    fcu.Settings.MainSettings.ImageFormat.GetImageFormat(),
                    fcu.Settings.MainSettings.ImageScale);

            DARequest request = new DARequest
            {
                Query = query,
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {fcu.FigmaSession.Token}"
                }
            };

            return request;
        }

        public static string CreateImagesQuery(List<string> chunk, string projectId, string extension, float scale)
        {
            string joinedIds = string.Join(",", chunk);

            if (joinedIds[0] == ',')
                joinedIds = joinedIds.Remove(0, 1);

            string query = $"https://api.figma.com/v1/images/{projectId}?ids={joinedIds}&format={extension}&scale={scale.ToDotString()}";
            return query;
        }

        public static DARequest CreateTokenRequest(string code)
        {
            string tokenQueryLink = string.Format(FcuConfig.AuthUrl, FcuConfig.ClientId, FcuConfig.ClientSecret, FcuConfig.RedirectUri, code);

            DARequest request = new DARequest
            {
                Query = tokenQueryLink,
                RequestType = RequestType.Post,
                WWWForm = new WWWForm()
            };

            return request;
        }

        public static DARequest CreateProjectRequest(string token, string projectId)
        {
            string query = string.Format("https://api.figma.com/v1/files/{0}?depth={1}&plugin_data=shared", projectId, FcuConfig.Instance.InspectorDepth);

            DARequest request = new DARequest
            {
                Name = RequestName.Project,
                Query = query,
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {token}"
                }
            };

            return request;
        }

        public static DARequest CreateNodeRequest(string token, string projectId, string nodeIds)
        {
            string query = string.Format("https://api.figma.com/v1/files/{0}/nodes?ids={1}&geometry=paths&plugin_data=shared", projectId, nodeIds);

            DARequest request = new DARequest
            {
                Query = query,
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {token}"
                }
            };

            return request;
        }
    }
}