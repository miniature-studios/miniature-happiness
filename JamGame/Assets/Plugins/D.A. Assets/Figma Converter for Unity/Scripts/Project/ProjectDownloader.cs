using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DA_Assets.FCU
{
    [Serializable]
    public class ProjectDownloader : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator DownloadProject()
        {
            monoBeh.InspectorDrawer.SelectableDocument.Childs.Clear();

            if (monoBeh.FigmaSession.IsAuthed() == false)
            {
                DALogger.LogError(FcuLocKey.log_need_auth.Localize());
                monoBeh.Events.OnProjectDownloadFail?.Invoke(monoBeh);
                yield break;
            }

            monoBeh.Events.OnProjectDownloadStart?.Invoke(monoBeh);

            DAResult<FigmaProject> result = default;

            yield return DownloadProject(monoBeh.Settings.MainSettings.ProjectUrl, x => result = x);

            if (result.Success)
            {
                monoBeh.CurrentProject.FigmaProject = result.Object;
                monoBeh.InspectorDrawer.FillSelectableFramesArray();

                DALogger.Log(FcuLocKey.log_project_downloaded.Localize());

                monoBeh.Events.OnProjectDownloaded?.Invoke(monoBeh);
            }
            else
            {
                switch (result.Error.Status)
                {
                    case 403:
                        DALogger.LogError(FcuLocKey.log_need_auth.Localize());
                        break;
                    case 404:
                        DALogger.LogError(FcuLocKey.log_project_not_found.Localize());
                        break;
                    default:
                        DALogger.LogError(FcuLocKey.log_unknown_error.Localize(result.Error.Message, result.Error.Status, result.Error.Exception));
                        break;
                }

                monoBeh.Events.OnProjectDownloadFail?.Invoke(monoBeh);
                monoBeh.AssetTools.StopImport();
            }
        }

        public IEnumerator DownloadProject(string projectUrl, Return<FigmaProject> @return)
        {
            DARequest projectRequest = RequestCreator.CreateProjectRequest(
                monoBeh.FigmaSession.Token,
                projectUrl);

            yield return monoBeh.RequestSender.SendRequest(projectRequest, @return);
        }

        public IEnumerator DownloadAllNodes(List<string> selectedIds, Return<List<FObject>> @return)
        {
            List<List<string>> idChunks = selectedIds.Split(FcuConfig.Instance.ChunkSizeGetNodes);
            List<List<FObject>> nodeChunks = new List<List<FObject>>();

            foreach (List<string> chunk in idChunks)
            {
                string ids = string.Join(",", chunk);

                DARequest projectRequest = RequestCreator.CreateNodeRequest(
                    monoBeh.FigmaSession.Token,
                    monoBeh.Settings.MainSettings.ProjectUrl,
                    ids);

                monoBeh.RequestSender.SendRequest<FigmaProject>(projectRequest, result =>
                {
                    if (result.Success)
                    {
                        List<FObject> docs = new List<FObject>();

                        if (!result.Object.IsDefault())
                        {
                            if (!result.Object.Nodes.IsEmpty())
                            {
                                foreach (var item in result.Object.Nodes)
                                {
                                    if (item.Value.IsDefault())
                                        continue;

                                    docs.Add(item.Value.Document);
                                }
                            }
                        }

                        nodeChunks.Add(docs);
                    }
                    else
                    {
                        nodeChunks.Add(default);

                        switch (result.Error.Status)
                        {
                            default:
                                DALogger.LogError(FcuLocKey.log_cant_get_part_of_frames.Localize(result.Error.Message, result.Error.Status));
                                break;
                        }
                    }
                }).StartDARoutine(monoBeh);
            }


            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(nodeChunks, idChunks, FcuLocKey.log_getting_frames, nodeChunks.CountAll(), selectedIds.Count(), ref tempCount))
            {
                yield return WaitFor.Delay1();
            }

            @return.Invoke(new DAResult<List<FObject>>
            {
                Success = true,
                Object = nodeChunks.FromChunks()
            });
        }
    }
}
